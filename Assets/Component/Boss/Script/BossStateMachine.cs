using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState { Waiting, Attacking, Victory }

    [Header("Configuration")]
    public float waitingDuration = 120f;
    public BossState currentState = BossState.Waiting;

    [Header("Références")]
    public GameObject targetLaser;
    public ObstacleController obstacleController;

    private float _timer = 0f;

    void Start()
    {
        currentState = BossState.Waiting;
        _timer = 0f;

        if (targetLaser != null)
            targetLaser.SetActive(false);

        // Vitesse normale au début
        if (obstacleController != null)
            obstacleController.SetBossSpeedActive(false);
    }

    void Update()
    {
        if (currentState == BossState.Waiting)
        {
            _timer += Time.deltaTime;

            if (_timer >= waitingDuration)
            {
                TriggerBossAttack();
            }
        }
    }

    private void TriggerBossAttack()
    {
        currentState = BossState.Attacking;
        Debug.Log("<color=red>BOSS : PHASE ATTACKING !</color>");

        if (targetLaser != null)
            targetLaser.SetActive(true);

        // On active le mode vitesse Boss
        if (obstacleController != null)
            obstacleController.SetBossSpeedActive(true);
    }

    public void EndBossAttack()
    {
        currentState = BossState.Victory;

        if (targetLaser != null)
            targetLaser.SetActive(false);

        // Retour à la vitesse normale
        if (obstacleController != null)
            obstacleController.SetBossSpeedActive(false);
    }
}