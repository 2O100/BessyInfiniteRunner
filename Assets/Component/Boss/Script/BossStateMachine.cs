using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState { Waiting, Attacking }

    [Header("Configuration")]
    public float waitingDuration = 10f;
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

        // On informe l'EventSystem
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.TriggerBossStateChanged(BossState.Attacking);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelUpBoss();
            GameManager.Instance.ShowBossHealthBar(true);
        }

        if (targetLaser != null) targetLaser.SetActive(true);
        if (obstacleController != null) obstacleController.SetBossSpeedActive(true);
    }

    public void EndBossAttack()
    {
        currentState = BossState.Waiting;
        _timer = 0f;

        // On informe l'EventSystem
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.TriggerBossStateChanged(BossState.Waiting);

        Debug.Log("<color=blue>BOSS : Retour en phase ATTENTE</color>");

        if (GameManager.Instance != null) GameManager.Instance.ShowBossHealthBar(false);
        if (targetLaser != null) targetLaser.SetActive(false);
        if (obstacleController != null) obstacleController.SetBossSpeedActive(false);
    }

    void OnEnable() { EventSystem.OnBossDefeated += EndBossAttack; }
    void OnDisable() { EventSystem.OnBossDefeated -= EndBossAttack; }
}