using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;


[System.Serializable]
public class SaveData
{
    public List<string> cardSymbols;
    public List<bool> cardMatched;
    public List<bool> cardFlipped;
    public int score;
    public int combo;
    public int turns;
    public int matchedPairs;
    public float gameTime;
    public string timestamp;
}

public static class SaveSystem
{
    private const string SAVE_KEY = "MemoryGameSave";

    

    public static void SaveGame()
    {
        var gm = GameManager.Instance;
        var saveData = new SaveData
        {
            cardSymbols = gm.allCards.Select(card => card.Symbol).ToList(),
            cardMatched = gm.allCards.Select(card => card.IsMatched).ToList(),
            cardFlipped = gm.allCards.Select(card => card.IsFlipped).ToList(),
            score = gm.CurrentScore,
            combo = gm.CurrentCombo,
            turns = gm.CurrentTurns,
            matchedPairs = gm.MatchedPairs,
            gameTime = Time.time - gm.gameStartTime,
            timestamp = System.DateTime.Now.ToBinary().ToString()
        };

        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public static void LoadGame()
    {
        if (!HasSaveData()) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        GameManager.Instance.LoadFromSave(saveData);
    }

    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }
}