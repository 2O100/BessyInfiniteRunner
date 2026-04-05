using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nécessaire pour le texte du score
using UnityEngine.SceneManagement; // Nécessaire pour charger le Game Over

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Paramètres Santé")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    [Header("UI References (Icônes)")]
    public Image[] healthIcons;
    public Sprite fullBellSprite;
    public Sprite emptyBellSprite;

    [Header("Système de Score")]
    public TextMeshProUGUI scoreText; // Glisse ton texte UI ici dans l'inspecteur
    private float _distance = 0f;
    public float gameSpeedMultiplier = 1f; // Par défaut à 1, passera à 1.5 en boss

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        UpdateBellUI();
    }

    private void Update()
    {
        // Calcul du score en temps réel
        // Time.deltaTime * 1 = 1m par seconde / Time.deltaTime * 1.5 = 1.5m par seconde
        _distance += Time.deltaTime * gameSpeedMultiplier;

        // Affichage du score (on arrondit à l'entier le plus proche)
        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(_distance).ToString() + " m";
        }
    }

    public void TakeDamage()
    {
        if (_currentHealth > 0)
        {
            _currentHealth--;
            Debug.Log("<color=magenta>Santé actuelle = </color>" + _currentHealth);
            UpdateBellUI();

            if (_currentHealth <= 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateBellUI()
    {
        if (healthIcons == null) return;
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
            {
                healthIcons[i].sprite = (i < _currentHealth) ? fullBellSprite : emptyBellSprite;
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        // On sauvegarde le score dans la mémoire du jeu avant de quitter la scène
        PlayerPrefs.SetInt("FinalScore", Mathf.FloorToInt(_distance));

        // Charge la scène Game Over (vérifie bien le nom exact de ta scène)
        SceneManager.LoadScene("GameOver");
    }
}