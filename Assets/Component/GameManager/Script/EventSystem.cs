using UnityEngine;
using System;

public class EventSystem : MonoBehaviour
{
    public static EventSystem EventSystemInstance;

    public event Action<Vector3> OnPlayerOnTarget;
    public event Action OnTargetReset;

    public event Action OnPlayerHit;

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

    // Appelé par ton PlayerCollisionController
    public void TriggerPlayerHit()
    {
        Debug.Log("<color=yellow>EVENTSYSTEM : J'ai reçu l'alerte du Player, je contacte le GameManager...</color>");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(1);
        }
        else
        {
            Debug.LogError("EVENTSYSTEM : Je ne trouve pas le GameManager ! Vérifie qu'il est bien dans ta scène.");
        }
    }

    public static event Action OnBossDefeated;
    public void TriggerBossDefeated()
    {
        Debug.Log("<color=cyan>EVENTSYSTEM : Le Boss est vaincu, j'alerte les autres systèmes...</color>");
        OnBossDefeated?.Invoke();
    }
}