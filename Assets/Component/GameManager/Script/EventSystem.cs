using UnityEngine;
using System;

public class EventSystem : MonoBehaviour
{
    // Singleton instance for easy access to the EventSystem
    public static EventSystem EventSystemInstance;

    // --- GAMEPLAY EVENTS ---
    public event Action<Vector3> OnPlayerOnTarget;
    public event Action OnTargetReset;
    public event Action OnPlayerHit;

    // --- SCORE EVENTS ---
    // Added: Event for firefly collection to handle scoring via events
    public event Action<int> OnFireflyCollected;

    // --- BOSS EVENTS ---
    // Centralized event for Boss state changes (Attacking, Waiting, etc.)
    public static event Action<BossStateMachine.BossState> OnBossStateChanged;
    // Event triggered when the Boss health reaches zero
    public static event Action OnBossDefeated;

    private void Awake()
    {
        // Initialize the Singleton pattern
        if (EventSystemInstance == null) EventSystemInstance = this;
    }

    // Notifies listeners when the player is correctly aligned with a target
    public void TriggerPlayerOnTarget(Vector3 targetPos)
    {
        OnPlayerOnTarget?.Invoke(targetPos);
    }

    // Notifies listeners to reset target-related logic
    public void TriggerTargetReset()
    {
        OnTargetReset?.Invoke();
    }

    // Broadcasts firefly collection to any listening script (like GameManager)
    public void TriggerFireflyCollected(int amount)
    {
        OnFireflyCollected?.Invoke(amount);
    }

    // Handles the logic when the player hits an obstacle
    public void TriggerPlayerHit()
    {
        Debug.Log("<color=yellow>EVENTSYSTEM: Player hit detected, notifying GameManager...</color>");

        // Directly tells the GameManager to reduce player health
        if (GameManager.Instance != null) GameManager.Instance.TakeDamage(1);
    }

    // Broadcasts the new Boss state to all listening scripts (like ObstacleController)
    public void TriggerBossStateChanged(BossStateMachine.BossState newState)
    {
        OnBossStateChanged?.Invoke(newState);
    }

    // Broadcasts that the boss has been defeated
    public void TriggerBossDefeated()
    {
        Debug.Log("<color=cyan>EVENTSYSTEM: Boss has been defeated!</color>");
        OnBossDefeated?.Invoke();
    }
}