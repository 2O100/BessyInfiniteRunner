using UnityEngine;
using System;

public class EventSystem : MonoBehaviour
{
    public static EventSystem EventSystemInstance;

    // Événement qui transporte la position de la cible
    public event Action<Vector3> OnPlayerOnTarget;

    private void Awake()
    {
        if (EventSystemInstance == null) EventSystemInstance = this;
    }

    public void TriggerPlayerOnTarget(Vector3 targetPos)
    {
        OnPlayerOnTarget?.Invoke(targetPos);
    }
}