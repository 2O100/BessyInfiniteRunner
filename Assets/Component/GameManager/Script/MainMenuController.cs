using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels & Images")]
    public GameObject menuBG;     
    public GameObject rulesPanel; 

    [Header("Buttons")]
    public GameObject startButton;
    public GameObject rulesButton;
    public GameObject backButton; 

    
    /// Switch from Main Menu to Rules screen
   
    public void OpenRules()
    {
        
        menuBG.SetActive(false);
        startButton.SetActive(false);
        rulesButton.SetActive(false);

        
        rulesPanel.SetActive(true);
        backButton.SetActive(true);
    }

    
    /// Switch from Rules screen back to Main Menu
    
    public void CloseRules()
    {
        
        menuBG.SetActive(true);
        startButton.SetActive(true);
        rulesButton.SetActive(true);

       
        rulesPanel.SetActive(false);
        backButton.SetActive(false);
    }
}