using UnityEngine;
using UnityEngine.SceneManagement; // Obligatoire pour changer de scène

public class SceneChanger : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Fonction pour lancer le jeu
    public void StartGame()
    {
        // Remplace "MaSceneDeJeu" par le nom EXACT de ta scène de jeu
        SceneManager.LoadScene("InGame");
    }

    // Fonction pour retourner au Menu
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Fonction pour quitter le jeu (ne marche que dans le .exe final)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Le jeu a été fermé");
    }
}