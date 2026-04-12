using UnityEngine;

[System.Serializable]
public class SaveData
{
    [Header("Best Run (Highscores)")]
    public int bestFireflies;
    public float bestDistance;

    [Header("Lifetime Totals (Cumulative)")]
    public int totalFireflies;
    public float totalDistance;
}