using UnityEngine;
using System;

public class EventSystem : MonoBehaviour
{
    public static EventSystem EventSystemInstance;

    public event Action<Vector3> OnPlayerOnTarget;
    public event Action OnTargetReset;
    public event Action OnPlayerHit;

    // --- NOUVEAU : Événement centralisé pour l'état du Boss ---
    public static event Action<BossStateMachine.BossState> OnBossStateChanged;

    private void Awake()
    {
        if (EventSystemInstance == null) EventSystemInstance = this;
    }

    public void TriggerPlayerOnTarget(Vector3 targetPos)
    {
        OnPlayerOnTarget?.Invoke(targetPos);
    }

    public void TriggerTargetReset()
    {
        OnTargetReset?.Invoke();
    }

    public void TriggerPlayerHit()
    {
        Debug.Log("<color=yellow>EVENTSYSTEM : J'ai reçu l'alerte du Player, je contacte le GameManager...</color>");
        if (GameManager.Instance != null) GameManager.Instance.TakeDamage(1);
    }

    // --- NOUVELLE MÉTHODE : Pour diffuser le changement d'état ---
    public void TriggerBossStateChanged(BossStateMachine.BossState newState)
    {
        OnBossStateChanged?.Invoke(newState);
    }

    public static event Action OnBossDefeated;
    public void TriggerBossDefeated()
    {
        Debug.Log("<color=cyan>EVENTSYSTEM : Le Boss est vaincu !</color>");
        OnBossDefeated?.Invoke();
    }
}