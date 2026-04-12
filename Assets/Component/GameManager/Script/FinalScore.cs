using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    [Header("UI Display")]
    public TextMeshProUGUI lenghtText; 
    public TextMeshProUGUI fireflyText;  

    void Start()
    {
        // On récupère les données sauvegardées dans le GameManager
        int finalDistance = PlayerPrefs.GetInt("FinalDistance", 0);
        int finalFireflies = PlayerPrefs.GetInt("FinalFireflies", 0);

        // On affiche la distance (en cm comme dans le jeu)
        if (lenghtText != null)
        {
            lenghtText.text = "TOTAL DISTANCE: " + finalDistance + " CM";
        }

        // On affiche le nombre de lucioles
        if (fireflyText != null)
        {
            fireflyText.text = "x" + finalFireflies;
        }
    }
}