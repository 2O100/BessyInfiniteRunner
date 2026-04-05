using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Paramètres Santé")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    [Header("UI References (Icônes)")]
    // Glisse tes 3 images d'UI ici dans l'ordre (de gauche à droite)
    public Image[] healthIcons;

    // Glisse ton Sprite de cloche dorée ici
    public Sprite fullBellSprite;

    // Glisse ton Sprite de cloche grise ici
    public Sprite emptyBellSprite;

    private bool _isInvincible = false;
    [SerializeField] private float _invincibilityDuration = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerHit += TakeDamage;
    }

    private void OnDisable()
    {
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerHit -= TakeDamage;
    }

    private void Start()
    {
        // On initialise la santé
        _currentHealth = _maxHealth;

        // Sécurité : on vérifie qu'on a bien 3 icônes pour 3 vies max
        if (healthIcons.Length != _maxHealth)
        {
            Debug.LogError("GameManager : Le nombre d'icônes de santé (healthIcons) ne correspond pas à _maxHealth !");
        }

        UpdateBellUI();
    }

    public void TakeDamage()
    {
        // Sécurité pour ne pas descendre en dessous de 0
        if (_currentHealth > 0)
        {
            _currentHealth--; // On enlève 1 point de vie

            // --- AJOUTE CETTE LIGNE ICI ---
            Debug.Log("<color=magenta>GAMEMANAGER : Dégât reçu ! Santé actuelle = " + _currentHealth + "</color>");
            // ------------------------------

            UpdateBellUI(); // Ta fonction qui change les images

            if (_currentHealth <= 0)
            {
                Debug.Log("PLUS DE VIE ! Direction Game Over...");
                Invoke("LoadGameOver", 1f); // On attend 1 seconde avant de changer pour voir la dernière cloche
            }
        }
    }

    // Le cœur de la logique d'affichage
    private void UpdateBellUI()
    {
        if (healthIcons == null || healthIcons.Length == 0) return;

        // On parcourt les 3 emplacements d'icônes
        for (int i = 0; i < healthIcons.Length; i++)
        {
            // Si l'index i est inférieur à la santé actuelle, on affiche une cloche dorée
            if (i < _currentHealth)
            {
                healthIcons[i].sprite = fullBellSprite;
                // Optionnel : s'assurer que la cloche dorée est opaque (alpha = 1)
                Color c = healthIcons[i].color;
                c.a = 1f;
                healthIcons[i].color = c;
            }
            // Sinon, on affiche une cloche grise
            else
            {
                healthIcons[i].sprite = emptyBellSprite;
                // Optionnel : Tu pourrais aussi rendre la cloche grise un peu transparente
                // Color c = healthIcons[i].color;
                // c.a = 0.5f;
                // healthIcons[i].color = c;
            }
        }
    }

    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        _isInvincible = true;
        // Optionnel : faire clignoter le sprite de la vache ici
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
    }

    private void GameOver()
    {
        Debug.Log("<color=red>GAME OVER</color>");
        // Time.timeScale = 0f; 
        // Afficher l'écran de Game Over ici
    }
    void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}