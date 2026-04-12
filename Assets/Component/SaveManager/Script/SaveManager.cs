using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_stats.json");

    // Load helper
    public static SaveData LoadData()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData(); // Returns empty data if no file exists
    }

    // Save logic with cumulative calculations
    public static void SaveRun(int runFireflies, float runDistance)
    {
        // 1. Load existing data
        SaveData data = LoadData();

        // 2. Update Totals (Cumulative)
        data.totalFireflies += runFireflies;
        data.totalDistance += runDistance;

        // 3. Update Bests (Highscores)
        if (runFireflies > data.bestFireflies) data.bestFireflies = runFireflies;
        if (runDistance > data.bestDistance) data.bestDistance = runDistance;

        // 4. Save back to disk
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("<color=green>STATS UPDATED: Totals and Bests synchronized.</color>");
    }
}