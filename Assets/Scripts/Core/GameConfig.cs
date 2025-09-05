using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "GameConfig", menuName = "BackTrack/Config")]
public class GameConfig : ScriptableObject
{
    [Header("Game Rules")]
    public int gridWidth = 4;
    public int gridHeight = 4;
    public List<string> cardSymbols = new List<string>();

    [Header("Timing")]
    public float cardFlipDuration = 0.5f;
    public float previewDuration = 2f;
    public float mismatchResetDelay = 1f;

    [Header("Scoring")]
    public int baseScore = 10;
    public int comboMultiplier = 2;

    [Header("Animation")]
    public Ease flipEase = Ease.OutBack;

    public int TotalCards => gridWidth * gridHeight;
    public int RequiredPairs => TotalCards / 2;
}
