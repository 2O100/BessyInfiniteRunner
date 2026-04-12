using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Central manager for game logic, UI updates, and data persistence.
/// </summary>
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

    // --- EVENT SUBSCRIPTION ---
    // This connects the GameManager to the EventSystem signals
    private void OnEnable()
    {
        if (EventSystem.EventSystemInstance != null)
        {
            // Subscribe to the firefly collection event
            EventSystem.EventSystemInstance.OnFireflyCollected += AddScore;
        }
    }

    private void OnDisable()
    {
        if (EventSystem.EventSystemInstance != null)
        {
            // Unsubscribe to avoid memory leaks or errors
            EventSystem.EventSystemInstance.OnFireflyCollected -= AddScore;
        }
    }

    private void Update()
    {
        // Continuous distance calculation based on time and speed
        _distance += Time.deltaTime * gameSpeedMultiplier;

        // Update the distance UI (rounded to nearest integer)
        if (scoreText != null)
        {
            scoreText.text = "DISTANCE " + Mathf.FloorToInt(_distance).ToString() + " CM";
        }
    }

    // --- COLLECTIBLE SYSTEM ---

    /// <summary>
    /// Adds fireflies and updates the counter. Called via EventSystem.
    /// </summary>
    public void AddScore(int amount)
    {
        _fireflyCount += amount;
        UpdateFireflyUI();
        Debug.Log($"<color=green>GM: Added {amount} fireflies. Total: {_fireflyCount}</color>");
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

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UpdateHealthUI();

        if (_currentHealth <= 0) GameOver();
    }

    public void TakeBossDamage(int damage)
    {
        if (_currentBossHealth <= 0) return;

        _currentBossHealth -= damage;
        UpdateBossUI();

        if (_currentBossHealth <= 0)
        {
            _currentBossHealth = 0;
            EventSystem.EventSystemInstance?.TriggerBossDefeated();
        }
    }

    private void UpdateBossUI()
    {
        if (bossHealthSlider != null) bossHealthSlider.value = _currentBossHealth;

        if (bossPercentText != null)
        {
            float percentage = ((float)_currentBossHealth / (float)_bossMaxHealth) * 100f;
            bossPercentText.text = percentage.ToString("F2") + "%";
        }
    }

    private void UpdateHealthUI()
    {
        if (healthIcons == null) return;
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
                healthIcons[i].sprite = (i < _currentHealth) ? fullBellSprite : emptyBellSprite;
        }
    }

    public void ShowBossHealthBar(bool isVisible) => bossHealthSlider?.gameObject.SetActive(isVisible);

    public void LevelUpBoss() { /* Level up logic here */ }

    // --- GAME OVER & PERSISTENCE ---

    private void GameOver()
    {
        // 1. Temporary storage for the GameOver scene display
        PlayerPrefs.SetInt("FinalDistance", Mathf.FloorToInt(_distance));
        PlayerPrefs.SetInt("FinalFireflies", _fireflyCount);

        // 2. Persistent storage using JSON (for Highscores and Lifetime totals)
        if (SaveManager.LoadData() != null)
        {
            SaveManager.SaveRun(_fireflyCount, _distance);
        }

        // 3. Change Scene
        SceneManager.LoadScene("GameOver");
    }
}
