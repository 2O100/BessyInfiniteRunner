using UnityEngine;

/// <summary>
/// Manages the Boss behavior cycle between 'Waiting' and 'Attacking' states.
/// It uses the EventSystem to notify the rest of the game about state changes.
/// </summary>
public class BossStateMachine : MonoBehaviour
{
    // Possible states for the Boss
    public enum BossState { Waiting, Attacking }

    [Header("Configuration")]
    [Tooltip("Time in seconds the boss waits before launching a new attack.")]
    public float waitingDuration = 10f;
    public BossState currentState = BossState.Waiting;

    [Header("References")]
    [Tooltip("Visual laser target that tracks the player.")]
    public GameObject targetLaser;
    [Tooltip("Reference to the controller managing world scrolling.")]
    public ObstacleController obstacleController;

    private float _timer = 0f;

    void Start()
    {
        // Initializing the boss in a resting state
        currentState = BossState.Waiting;
        _timer = 0f;

        // Hide the targeting laser at the start
        if (targetLaser != null) targetLaser.SetActive(false);
    }

    void Update()
    {
        // Only run the timer if the boss is currently in the 'Waiting' phase
        if (currentState == BossState.Waiting)
        {
            _timer += Time.deltaTime;

            // Transition to attack mode once the duration is reached
            if (_timer >= waitingDuration)
            {
                TriggerBossAttack();
            }
        }
    }

    /// <summary>
    /// Activates the Boss attack phase and notifies all game systems.
    /// </summary>
    private void TriggerBossAttack()
    {
        currentState = BossState.Attacking;

        // Notify systems via EventSystem (e.g., ObstacleController speeds up)
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.TriggerBossStateChanged(BossState.Attacking);

        // Update GameManager logic for combat
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelUpBoss(); // Increases boss difficulty/HP
            GameManager.Instance.ShowBossHealthBar(true); // Display health UI
        }

        // Enable visual combat cues
        if (targetLaser != null) targetLaser.SetActive(true);
    }

    /// <summary>
    /// POINT 3: Safely ends the combat phase when the boss is defeated.
    /// Cleans up visuals and resets the cycle timer.
    /// </summary>
    public void EndBossAttack()
    {
        // 1. Reset Internal State
        // Moving back to Waiting allows the game to return to standard difficulty
        currentState = BossState.Waiting;
        _timer = 0f; // Reset the cooldown so the player gets a full break

        // 2. Notify the world via EventSystem
        // This triggers the ObstacleController to return to normal scrolling speed
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.TriggerBossStateChanged(BossState.Waiting);

        // 3. UI and Visual Cleanup
        if (GameManager.Instance != null)
            GameManager.Instance.ShowBossHealthBar(false);

        // Disable targeting immediately to stop UFO fire
        if (targetLaser != null)
            targetLaser.SetActive(false);

        Debug.Log("<color=green>BOSS: Defeated! Cleaning up and returning to Waiting phase.</color>");
    }

    // --- EVENT SUBSCRIPTION ---
    // Listens for the BossDefeated event (usually triggered by GameManager when HP hits 0)
    void OnEnable() { EventSystem.OnBossDefeated += EndBossAttack; }

    // Unsubscribe to prevent memory leaks or errors when the object is destroyed
    void OnDisable() { EventSystem.OnBossDefeated -= EndBossAttack; }
}