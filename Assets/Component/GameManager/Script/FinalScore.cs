using UnityEngine;
using TMPro;

/// <summary>
/// Displays the final scores on the GameOver screen using data from the JSON save file.
/// </summary>
public class FinalScore : MonoBehaviour
{
    [Header("Current Run Display")]
    public TextMeshProUGUI currentDistanceText;
    public TextMeshProUGUI currentFireflyText;

    [Header("Best Run Display (Highscores)")]
    public TextMeshProUGUI bestDistanceText;
    public TextMeshProUGUI bestFireflyText;

    [Header("Lifetime Display (Totals)")]
    public TextMeshProUGUI totalDistanceText;
    public TextMeshProUGUI totalFireflyText;

    void Start()
    {
        // 1. Retrieve the stats from the previous session (Still using PlayerPrefs for the current run is fine)
        int runDistance = PlayerPrefs.GetInt("FinalDistance", 0);
        int runFireflies = PlayerPrefs.GetInt("FinalFireflies", 0);

        // 2. Load the persistent data from the JSON file via our SaveManager
        SaveData globalStats = SaveManager.LoadData();

        // 3. DISPLAY CURRENT RUN
        if (currentDistanceText != null) currentDistanceText.text = runDistance + " CM";
        if (currentFireflyText != null) currentFireflyText.text = "x " + runFireflies;

        // 4. DISPLAY BEST RECORDS (From JSON)
        if (bestDistanceText != null)
            bestDistanceText.text = Mathf.FloorToInt(globalStats.bestDistance) + " CM";
        if (bestFireflyText != null) bestFireflyText.text = "x " + globalStats.bestFireflies;

        // 5. DISPLAY LIFETIME TOTALS (From JSON)
        if (totalDistanceText != null)
            totalDistanceText.text = Mathf.FloorToInt(globalStats.totalDistance) + " CM";
        if (totalFireflyText != null) totalFireflyText.text = "x " + globalStats.totalFireflies;

        Debug.Log("<color=cyan>UI: Scores updated from JSON save file.</color>");
    }
}