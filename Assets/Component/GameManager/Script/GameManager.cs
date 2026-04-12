using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    [Header("Boss Settings")]
    [SerializeField] private int _bossMaxHealth = 9;
    private int _currentBossHealth;
    public TextMeshProUGUI bossPercentText;

    [Header("Boss References")]
    public Transform bossShootPoint;

    [Header("UI References")]
    public Image[] healthIcons;
    public Sprite fullBellSprite;
    public Sprite emptyBellSprite;
    public TextMeshProUGUI scoreText;   
    public TextMeshProUGUI fireflyText; 
    public Slider bossHealthSlider;

    [Header("Game Progression")]
    private float _distance = 0f;
    private int _fireflyCount = 0;
    public float gameSpeedMultiplier = 16f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _currentHealth = _maxHealth;
        _currentBossHealth = _bossMaxHealth;
    }

    private void Start()
    {
        UpdateHealthUI();
        UpdateFireflyUI(); // Affiche "x0" dès le lancement
        if (bossHealthSlider != null) bossHealthSlider.maxValue = _bossMaxHealth;
    }

    private void Update()
    {
        _distance += Time.deltaTime * gameSpeedMultiplier;
        if (scoreText != null)
        {
            scoreText.text = "LENGHT " + Mathf.FloorToInt(_distance).ToString() + " CM";
        }
    }

    // --- SYSTÈME DE COLLECTIBLES ---
    public void AddScore(int amount)
    {
        _fireflyCount += amount;
        UpdateFireflyUI();
    }

    private void UpdateFireflyUI()
    {
        if (fireflyText != null)
        {
            fireflyText.text = "x" + _fireflyCount.ToString();
        }
    }

    // --- RESTE DU CODE (BOSS & DAMAGE) ---
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
    public void LevelUpBoss() { /* Ton code de level up */ }

    private void GameOver()
    {
        PlayerPrefs.SetInt("FinalDistance", Mathf.FloorToInt(_distance));
        PlayerPrefs.SetInt("FinalFireflies", _fireflyCount);
        SceneManager.LoadScene("GameOver");
    }
}