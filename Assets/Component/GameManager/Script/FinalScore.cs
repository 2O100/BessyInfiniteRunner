using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    [Header("UI Display")]
    [Tooltip("Text component to display the total distance traveled.")]
    public TextMeshProUGUI lenghtText;

    [Tooltip("Text component to display the total fireflies collected.")]
    public TextMeshProUGUI fireflyText;

    void Start()
    {
        // Retrieve the saved data from the PlayerPrefs (stored by GameManager during GameOver)
        int finalDistance = PlayerPrefs.GetInt("FinalDistance", 0);
        int finalFireflies = PlayerPrefs.GetInt("FinalFireflies", 0);

        // Display the distance (matching the CM format used in-game)
        if (lenghtText != null)
        {
            lenghtText.text = "TOTAL DISTANCE: " + finalDistance + " CM";
        }

        // Display the total count of fireflies collected
        if (fireflyText != null)
        {
            fireflyText.text = "x" + finalFireflies;
        }
    }
}