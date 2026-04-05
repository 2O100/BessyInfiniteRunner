using UnityEngine;

public class UFOLaserShooter : MonoBehaviour
{
    [Header("Prefab & Points")]
    public GameObject laserPrefab;
    public Transform shootPoint;

    [Header("Laser Physics")]
    public float laserSpeed = 150f;
    public float laserLifetime = 1.5f;

    private bool _hasShotThisCycle = false;
    private bool _canShootNow = false;

    private void OnEnable()
    {
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerOnTarget += TryShoot;

        _hasShotThisCycle = false;
        _canShootNow = false;
    }

    private void OnDisable()
    {
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerOnTarget -= TryShoot;
    }

    public void SetAttackWindow(bool isOpen)
    {
        _canShootNow = isOpen;
        if (isOpen) _hasShotThisCycle = false;
    }

    public void TryShoot(Vector3 targetPosition)
    {
        if (!_canShootNow || _hasShotThisCycle) return;

        if (laserPrefab != null && shootPoint != null)
        {
            _hasShotThisCycle = true;

            GameObject laser = Instantiate(laserPrefab, shootPoint.position, Quaternion.identity);

            LaserMovement lm = laser.GetComponent<LaserMovement>();
            if (lm != null) lm.Initialize(laserSpeed, targetPosition);

            Destroy(laser, laserLifetime);

            // SIGNAL DE RESET : On prévient la cible qu'elle peut bouger
            if (EventSystem.EventSystemInstance != null)
                EventSystem.EventSystemInstance.TriggerTargetReset();

            Debug.Log("<color=red>UFO : Tir et Reset demandés !</color>");
        }
    }
}