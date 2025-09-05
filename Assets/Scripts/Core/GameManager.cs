using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Setup")]
    public GameConfig config;
    [SerializeField] private CardController cardPrefab;
    [SerializeField] private Transform cardParent;

    [Header("Runtime")]
    public List<CardController> allCards = new List<CardController>();
    [SerializeField] private CardController firstCard;
    [SerializeField] private CardController secondCard;

    public bool CanFlipCards { get; private set; } = true;
    public int CurrentTurns { get; private set; }
    public int CurrentScore { get; private set; }
    public int CurrentCombo { get; private set; }
    public int MatchedPairs { get; private set; }

    public float gameStartTime { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnCardFlipped += HandleCardFlipped;
    }

    private void OnDisable()
    {
        GameEvents.OnCardFlipped -= HandleCardFlipped;
    }

    [ContextMenu("Start New Game")]
    public void StartNewGame()
    {
        ClearExistingCards();
        CreateGameBoard();
        ResetGameState();

        gameStartTime = Time.time;
        GameEvents.OnGameStarted?.Invoke();
    }

    private void CreateGameBoard()
    {
        var symbols = new List<string>();
        int pairsNeeded = config.RequiredPairs;

        for (int i = 0; i < pairsNeeded; i++)
        {
            string symbol = config.cardSymbols[i % config.cardSymbols.Count];
            symbols.Add(symbol);
            symbols.Add(symbol);
        }

        for (int i = 0; i < symbols.Count; i++)
        {
            var temp = symbols[i];
            int randomIndex = Random.Range(i, symbols.Count);
            symbols[i] = symbols[randomIndex];
            symbols[randomIndex] = temp;
        }

        for (int i = 0; i < symbols.Count; i++)
        {
            var card = Instantiate(cardPrefab, cardParent);
            card.Initialize(symbols[i], config);
            allCards.Add(card);
        }
    }

    private void HandleCardFlipped(CardController card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            CurrentTurns++;
            GameEvents.OnTurnsUpdated?.Invoke(CurrentTurns);

            ProcessCardPair();
        }
    }

    private void ProcessCardPair()
    {
        CanFlipCards = false;

        if (firstCard.Symbol == secondCard.Symbol)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();

            CurrentCombo++;
            CurrentScore += config.baseScore * (1 + CurrentCombo * config.comboMultiplier);
            MatchedPairs++;

            GameEvents.OnCardsMatched?.Invoke(firstCard, secondCard);
            GameEvents.OnScoreUpdated?.Invoke(CurrentScore);
            GameEvents.OnComboUpdated?.Invoke(CurrentCombo);

            if (MatchedPairs >= config.RequiredPairs)
            {
                var stats = new GameStats
                {
                    finalScore = CurrentScore,
                    totalTurns = CurrentTurns,
                    maxCombo = CurrentCombo,
                    completionTime = Time.time - gameStartTime
                };
                GameEvents.OnGameCompleted?.Invoke(stats);
            }
        }
        else
        {
            CurrentCombo = 0;
            GameEvents.OnCardsMismatched?.Invoke(firstCard, secondCard);
            GameEvents.OnComboUpdated?.Invoke(CurrentCombo);

            StartCoroutine(firstCard.ResetAfterMismatch());
            StartCoroutine(secondCard.ResetAfterMismatch());
        }

        firstCard = null;
        secondCard = null;
        CanFlipCards = true;
    }

    private void ResetGameState()
    {
        CurrentTurns = 0;
        CurrentScore = 0;
        CurrentCombo = 0;
        MatchedPairs = 0;
        CanFlipCards = true;
    }

    private void ClearExistingCards()
    {
        foreach (var card in allCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        allCards.Clear();
    }

    public void LoadFromSave(SaveData saveData)
    {
        ClearExistingCards();

        CurrentScore = saveData.score;
        CurrentCombo = saveData.combo;
        CurrentTurns = saveData.turns;
        MatchedPairs = saveData.matchedPairs;
        gameStartTime = Time.time - saveData.gameTime;

        for (int i = 0; i < saveData.cardSymbols.Count; i++)
        {
            var card = Instantiate(cardPrefab, cardParent);
            card.Initialize(saveData.cardSymbols[i], config);

            if (saveData.cardMatched[i])
            {
                card.SetMatched();
            }
            else if (saveData.cardFlipped[i])
            {
                card.Flip();
            }

            allCards.Add(card);
        }

        GameEvents.OnGameStarted?.Invoke();
        GameEvents.OnScoreUpdated?.Invoke(CurrentScore);
        GameEvents.OnComboUpdated?.Invoke(CurrentCombo);
        GameEvents.OnTurnsUpdated?.Invoke(CurrentTurns);

        CanFlipCards = true;

        Debug.Log($"Game loaded! Score: {CurrentScore}, Turns: {CurrentTurns}");
    }
}
