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
        GameEvents.OnGameCompleted += ShowGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= ShowGamePanel;
        GameEvents.OnScoreUpdated -= UpdateScore;
        GameEvents.OnComboUpdated -= UpdateCombo;
        GameEvents.OnTurnsUpdated -= UpdateTurns;
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

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        finalScoreText.text = $"Final Score: {stats.finalScore:N0}";
        completionTimeText.text = $"Time: {stats.completionTime:F1}s";
        efficiencyText.text = $"Efficiency: {stats.finalScore / stats.totalTurns:F1} pts/turn";

        SaveSystem.ClearSave(); // Game completed, clear save
    }

    private void UpdateScore(int score) => scoreText.text = $"Score: {score:N0}";
    private void UpdateCombo(int combo) => comboText.text = $"Combo: x{combo}";
    private void UpdateTurns(int turns) => turnsText.text = $"Turns: {turns}";

    private void UpdateAllUI()
    {
        var gm = GameManager.Instance;
        UpdateScore(gm.CurrentScore);
        UpdateCombo(gm.CurrentCombo);
        UpdateTurns(gm.CurrentTurns);
        progressText.text = $"Pairs: {gm.MatchedPairs}/{gm.config.RequiredPairs}";
    }
}
