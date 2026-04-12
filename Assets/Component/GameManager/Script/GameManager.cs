using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance to allow easy access from any other script
    public static GameManager Instance;

    [Header("Player Settings")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    [Header("Boss Settings")]
    [SerializeField] private int _bossMaxHealth = 9;
    private int _currentBossHealth;
    public TextMeshProUGUI bossPercentText; // UI Text showing boss health percentage

    [Header("Boss References")]
    public Transform bossShootPoint; // Target point for countered projectiles

    [Header("UI References")]
    public Image[] healthIcons;       // Array of bell icons for player health
    public Sprite fullBellSprite;     // Active health sprite
    public Sprite emptyBellSprite;    // Lost health sprite
    public TextMeshProUGUI scoreText; // Text showing traveled distance
    public TextMeshProUGUI fireflyText; // Text showing collected fireflies
    public Slider bossHealthSlider;   // Visual health bar for the boss

    [Header("Game Progression")]
    private float _distance = 0f;     // Calculated distance in real-time
    private int _fireflyCount = 0;    // Number of fireflies collected
    public float gameSpeedMultiplier = 16f; // Overall speed of the world scrolling

    private void Awake()
    {
        // Initialize the Singleton pattern
        if (Instance == null) Instance = this;

        // Set initial health values
        _currentHealth = _maxHealth;
        _currentBossHealth = _bossMaxHealth;
    }

    private void Start()
    {
        // Initial UI refresh
        UpdateHealthUI();
        UpdateFireflyUI();

        // Setup boss health bar range
        if (bossHealthSlider != null) bossHealthSlider.maxValue = _bossMaxHealth;
    }

    private void Update()
    {
        // Continuous distance calculation based on time and speed
        _distance += Time.deltaTime * gameSpeedMultiplier;

        // Update the distance UI (rounded to nearest integer)
        if (scoreText != null)
        {
            scoreText.text = "LENGHT " + Mathf.FloorToInt(_distance).ToString() + " CM";
        }
    }

    // --- COLLECTIBLE SYSTEM ---

    // Adds fireflies and updates the counter
    public void AddScore(int amount)
    {
        _fireflyCount += amount;
        UpdateFireflyUI();
    }

    // Refreshes the firefly count on the screen
    private void UpdateFireflyUI()
    {
        if (fireflyText != null)
        {
            fireflyText.text = "x" + _fireflyCount.ToString();
        }
    }

    // --- PLAYER & BOSS LOGIC ---

    // Handles player damage and healing (if amount is negative)
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        // Ensure health stays between 0 and max
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        UpdateHealthUI();

        // Trigger Game Over if health reaches zero
        if (_currentHealth <= 0) GameOver();
    }

    // Handles boss damage when a projectile is countered
    public void TakeBossDamage(int damage)
    {
        if (_currentBossHealth <= 0) return;

        _currentBossHealth -= damage;
        UpdateBossUI();

        // Check for boss defeat
        if (_currentBossHealth <= 0)
        {
            _currentBossHealth = 0;
            // Notify the event system that the boss is down
            EventSystem.EventSystemInstance?.TriggerBossDefeated();
        }
    }

    // Updates the boss health bar and percentage text
    private void UpdateBossUI()
    {
        if (bossHealthSlider != null) bossHealthSlider.value = _currentBossHealth;

        if (bossPercentText != null)
        {
            // Cast to float for accurate percentage calculation
            float percentage = ((float)_currentBossHealth / (float)_bossMaxHealth) * 100f;
            bossPercentText.text = percentage.ToString("F2") + "%";
        }
    }

    // Updates the health icons (bells) based on current health
    private void UpdateHealthUI()
    {
        if (healthIcons == null) return;
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
                // If index is less than current health, show full bell; else show empty
                healthIcons[i].sprite = (i < _currentHealth) ? fullBellSprite : emptyBellSprite;
        }
    }

    // Toggles the visibility of the boss HUD
    public void ShowBossHealthBar(bool isVisible) => bossHealthSlider?.gameObject.SetActive(isVisible);

    // Reserved for boss evolution mechanics
    public void LevelUpBoss() { /* Level up logic here */ }

    // Finalizes the game session and saves scores
    private void GameOver()
    {
        // Save final stats to PlayerPrefs to retrieve them in the GameOver scene
        PlayerPrefs.SetInt("FinalDistance", Mathf.FloorToInt(_distance));
        PlayerPrefs.SetInt("FinalFireflies", _fireflyCount);

        // Load the final scene
        SceneManager.LoadScene("GameOver");
    }
}