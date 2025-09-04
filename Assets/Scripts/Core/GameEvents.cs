using System;

public static class GameEvents
{
    public static System.Action<CardController> OnCardFlipped;
    public static System.Action<CardController, CardController> OnCardsMatched;
    public static System.Action<CardController, CardController> OnCardsMismatched;

    public static Action OnGameStarted;
    public static Action<GameStats> OnGameCompleted;

    public static Action<int> OnScoreUpdated;
    public static Action<int> OnComboUpdated;
    public static Action<int> OnTurnsUpdated;
}

[Serializable]
public struct GameStats
{
    public int finalScore;
    public int totalTurns;
    public int maxCombo;
    public float completionTime;
}