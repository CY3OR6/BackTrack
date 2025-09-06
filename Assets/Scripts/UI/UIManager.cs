using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text turnsText;
    [SerializeField] private TMP_Text progressText;

    [Header("Game Over UI")]
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalTurnsText;
    [SerializeField] private TMP_Text completionTimeText;
    [SerializeField] private TMP_Text efficiencyText;

    [Header("Menu UI")]
    [SerializeField] private Button continueButton;

    private void OnEnable()
    {
        GameEvents.OnGameStarted += ShowGamePanel;
        GameEvents.OnScoreUpdated += UpdateScore;
        GameEvents.OnComboUpdated += UpdateCombo;
        GameEvents.OnTurnsUpdated += UpdateTurns;
        GameEvents.OnCardsMatched += (c1, c2) => UpdateProgress();
        GameEvents.OnGameCompleted += ShowGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= ShowGamePanel;
        GameEvents.OnScoreUpdated -= UpdateScore;
        GameEvents.OnComboUpdated -= UpdateCombo;
        GameEvents.OnTurnsUpdated -= UpdateTurns;
        GameEvents.OnCardsMatched -= (c1, c2) => UpdateProgress();
        GameEvents.OnGameCompleted -= ShowGameOver;
    }

    private void Start()
    {
        ShowMenuPanel();
        continueButton.gameObject.SetActive(SaveSystem.HasSaveData());
    }

    public void OnNewGameClicked()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnContinueClicked()
    {
        SaveSystem.LoadGame();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    private void ShowMenuPanel()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void ShowGamePanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);

        UpdateAllUI();
    }

    private void ShowGameOver(GameStats stats)
    {
        StartCoroutine(ShowGameOverDelayed(stats));
    }

    private IEnumerator ShowGameOverDelayed(GameStats stats)
    {
        yield return new WaitForSeconds(1f);

        GameManager gm = GameManager.Instance;

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        finalScoreText.text = $"Final Score:\n{stats.finalScore:N0}";
        completionTimeText.text = $"Time:\n{stats.completionTime:F1}s";
        efficiencyText.text = $"Efficiency:\n{stats.finalScore / stats.totalTurns:F1} pts/turn";
        finalTurnsText.text = $"Turns:\n{gm.CurrentTurns}";

        SaveSystem.ClearSave();
    }

    private void UpdateScore(int score) => scoreText.text = $"Score:\n{score:N0}";
    private void UpdateCombo(int combo) => comboText.text = $"Combo:\nx{combo}";
    private void UpdateTurns(int turns) => turnsText.text = $"Turns:\n{turns}";

    private void UpdateProgress()
    {
        GameManager gm = GameManager.Instance;
        progressText.text = $"Pairs:\n{gm.MatchedPairs}/{gm.config.RequiredPairs}";
    }

    private void UpdateAllUI()
    {
        GameManager gm = GameManager.Instance;
        UpdateScore(gm.CurrentScore);
        UpdateCombo(gm.CurrentCombo);
        UpdateTurns(gm.CurrentTurns);
        progressText.text = $"Pairs: {gm.MatchedPairs}/{gm.config.RequiredPairs}";
    }
}
