using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState { Waiting, Attacking, Victory }

    [Header("Configuration des Phases")]
    [Tooltip("Durée de la phase d'attente en secondes (ex: 120 pour 2 minutes)")]
    public float waitingDuration = 120f;

    [Header("État Actuel")]
    public BossState currentState = BossState.Waiting;

    [Header("Références")]
    public GameObject targetLaser; // L'objet LaserTarget avec le script LaserTargetMovement

    private float _timer = 0f;

    void Start()
    {
        // Au lancement, on s'assure d'être en attente et que la cible est éteinte
        currentState = BossState.Waiting;
        _timer = 0f;
        if (targetLaser != null) targetLaser.SetActive(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Waiting:
                UpdateWaitingState();
                break;

            case BossState.Attacking:
                UpdateAttackingState();
                break;

            case BossState.Victory:
                // Logique optionnelle ici (ex: le boss s'arrête de bouger)
                if (targetLaser != null) targetLaser.SetActive(false);
                break;
        }
    }

    private void UpdateWaitingState()
    {
        // On incrémente le timer
        _timer += Time.deltaTime;

        // Si on dépasse la durée configurée (modifiable dans l'inspecteur)
        if (_timer >= waitingDuration)
        {
            StartAttacking();
        }
    }

    private void UpdateAttackingState()
    {
        // Ici, l'attaque tourne via le script LaserTargetMovement
        // On pourrait ajouter une condition pour arrêter l'attaque si besoin
    }

    public void StartAttacking()
    {
        currentState = BossState.Attacking;
        _timer = 0f;
        if (targetLaser != null)
        {
            targetLaser.SetActive(true);
        }
        Debug.Log("<color=orange>BOSS : Fin de l'attente, début de l'attaque !</color>");
    }

    public void SetVictory()
    {
        currentState = BossState.Victory;
        if (targetLaser != null) targetLaser.SetActive(false);
    }
}