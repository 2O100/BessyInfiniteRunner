using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState { Waiting, Attacking }

    [Header("Configuration")]
    public float waitingDuration = 10f; // Temps avant la prochaine attaque
    public BossState currentState = BossState.Waiting;

    [Header("Références")]
    public GameObject targetLaser;
    public ObstacleController obstacleController;

    private float _timer = 0f;

    void Start()
    {
        currentState = BossState.Waiting;
        _timer = 0f;

        if (targetLaser != null) targetLaser.SetActive(false);
        if (obstacleController != null) obstacleController.SetBossSpeedActive(false);
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
        Debug.Log("<color=red>BOSS : PHASE ATTAQUE !</color>");

        if (targetLaser != null) targetLaser.SetActive(true);
        if (obstacleController != null) obstacleController.SetBossSpeedActive(true);
    }

    // Appelé par le GameManager quand le boss n'a plus de vie
    public void EndBossAttack()
    {
        currentState = BossState.Waiting; // On boucle ici
        _timer = 0f; // Reset du chrono pour la prochaine fois

        Debug.Log("<color=blue>BOSS : Retour en phase ATTENTE</color>");

        if (targetLaser != null) targetLaser.SetActive(false);
        if (obstacleController != null) obstacleController.SetBossSpeedActive(false);
    }
}