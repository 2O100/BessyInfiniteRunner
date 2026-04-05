using UnityEngine;
using TMPro; // Required for TextMeshPro

public class FinalScore : MonoBehaviour
{
    // Reference to your UI Text component
    public TextMeshProUGUI scoreDisplayText;

    void Start()
    {
        // We retrieve the score saved by the GameManager
        // "FinalScore" is the key we used in PlayerPrefs.SetInt
        int distanceReached = PlayerPrefs.GetInt("FinalScore", 0);

        // Display the result in English
        if (scoreDisplayText != null)
        {
            scoreDisplayText.text = "Final Distance: " + distanceReached + " m";
        }
        else
        {
            // Fallback: try to get the component on the same object
            scoreDisplayText = GetComponent<TextMeshProUGUI>();
            if (scoreDisplayText != null)
            {
                scoreDisplayText.text = "Final Distance: " + distanceReached + " m";
            }
        }
    }
}