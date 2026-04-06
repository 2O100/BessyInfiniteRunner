using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance to access GameManager from anywhere

    [Header("Player Settings")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    [Header("Boss Settings")]
    [SerializeField] private int _bossMaxHealth = 10;
    private int _currentBossHealth;
    public TextMeshProUGUI bossPercentText;

    [Header("Boss References")]
    public Transform bossShootPoint; //For CounterObstacle

    [Header("UI References")]
    public Image[] healthIcons;       // Array of UI Images (the bells)
    public Sprite fullBellSprite;     // Sprite for active health
    public Sprite emptyBellSprite;    // Sprite for lost health
    public TextMeshProUGUI scoreText; // UI Text for distance
    public Slider bossHealthSlider;

    [Header("Game Progression")]
    private float _distance = 0f;
    public float gameSpeedMultiplier = 16f; // Controls the scrolling speed of the world

    private void Awake()
    {
        // Setup Singleton pattern
        if (Instance == null) Instance = this;

        // Initialize health values
        _currentHealth = _maxHealth;
        _currentBossHealth = _bossMaxHealth;
        bossHealthSlider.maxValue = _bossMaxHealth;
    }

    private void Start()
    {
        UpdateHealthUI();

    }

    private void Update()
    {
        // Increment distance based on time and current speed
        _distance += Time.deltaTime * gameSpeedMultiplier;

        // Update the UI text (rounded down to nearest meter)
        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(_distance).ToString() + " m";
        }
    }

    public void LevelUpBoss()
    {
        // 1. On augmente le palier
        _bossMaxHealth += 1;

        // 2. On remplit la vie au NOUVEAU max
        _currentBossHealth = _bossMaxHealth;

        // 3. On met à jour le Slider (le contenant)
        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = _bossMaxHealth;
            bossHealthSlider.value = _currentBossHealth;
        }

        // 4. On met à jour le Texte % (le contenu)
        UpdateBossUI();

        Debug.Log("BOSS LEVEL UP : Vie actuelle " + _currentBossHealth + " sur " + _bossMaxHealth);
    }

    public void ShowBossHealthBar(bool isVisible)
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(isVisible);
        }
    }

    // Called when the player hits a standard obstacle
    // On ajoute "int amount" pour que la fonction puisse recevoir un chiffre
    public void TakeDamage(int amount)
    {
        // On soustrait le montant (si amount est -1, ça ajoute +1)
        _currentHealth -= amount;

        // Sécurité : on empêche la vie de dépasser le max ou de descendre sous 0
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            GameOver();
        }
    }

    // Called when a DungBall is successfully countered back at the Boss
    [HideInInspector] public int bossHealth = 10;

    public void TakeBossDamage(int damage)
    {
        bossHealth -= damage;
        // CE LOG EST INDISPENSABLE POUR SAVOIR SI CA MARCHE
        Debug.Log("<color=red>[GameManager] Le Boss a été touché ! Vie restante : </color>" + bossHealth);

        if (_currentBossHealth <= 0) return;

        _currentBossHealth -= damage;
        UpdateBossUI(); // Ton Slider

        if (_currentBossHealth <= 0)
        {
            _currentBossHealth = 0;

            // On prévient l'EventSystem
            if (EventSystem.EventSystemInstance != null)
            {
                EventSystem.EventSystemInstance.TriggerBossDefeated();
            }
        }

    }

    private void UpdateBossUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = _currentBossHealth;
        }

        if (bossPercentText != null)
        {
            // On force le calcul en float pour éviter que 11/12 donne 0
            float percentage = ((float)_currentBossHealth / (float)_bossMaxHealth) * 100f;

            // F2 pour le format 00.00%
            bossPercentText.text = percentage.ToString("F2") + "%";
        }
    }


    // Logic executed when Boss health reaches zero
    private void BossDefeated()
    {
        Debug.Log("<color=yellow>[GameManager] Boss Stunned! Switching to Waiting phase.</color>");

        // Find the Boss state machine and force it back to Waiting
        BossStateMachine boss = Object.FindFirstObjectByType<BossStateMachine>();
        if (boss != null) boss.EndBossAttack();

        // Reset Boss health for the next encounter
        _currentBossHealth = _bossMaxHealth;
    }

    // Updates the bell icons in the UI
    private void UpdateHealthUI()
    {
        if (healthIcons == null) return;
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
                healthIcons[i].sprite = (i < _currentHealth) ? fullBellSprite : emptyBellSprite;
        }
    }

    // Used by external triggers to speed up or slow down the game
    public void SetSpeedMultiplier(float value) => gameSpeedMultiplier = value;

    private void GameOver()
    {
        // Save score and switch to Game Over scene
        PlayerPrefs.SetInt("FinalScore", Mathf.FloorToInt(_distance));
        SceneManager.LoadScene("GameOver");
    }
}