using UnityEngine;

public class UFOLaserShooter : MonoBehaviour
{
    [Header("Prefab & Points")]
    public GameObject laserPrefab; // The laser projectile to spawn
    public Transform shootPoint;   // The origin point of the shot

    [Header("Laser Physics")]
    public float laserSpeed = 150f;   // Travel speed of the laser
    public float laserLifetime = 1.5f; // Time before the laser is destroyed

    private bool _hasShotThisCycle = false; // Prevents multiple shots during one targeting window
    private bool _canShootNow = false;      // Controlled by the LaserTarget (state window)

    private void OnEnable()
    {
        // Subscribe to the event triggered when the player is on the target
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerOnTarget += TryShoot;

        // Reset flags when the UFO is activated
        _hasShotThisCycle = false;
        _canShootNow = false;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent errors when the object is destroyed or disabled
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerOnTarget -= TryShoot;
    }

    // Opens or closes the window during which the UFO is allowed to fire
    public void SetAttackWindow(bool isOpen)
    {
        _canShootNow = isOpen;
        if (isOpen) _hasShotThisCycle = false; // Reset the cycle flag when a new window opens
    }

    // Attempt to fire a laser at the target position
    public void TryShoot(Vector3 targetPosition)
    {
        // Safety check: Don't shoot if the window is closed or if we already shot during this window
        if (!_canShootNow || _hasShotThisCycle) return;

        if (laserPrefab != null && shootPoint != null)
        {
            _hasShotThisCycle = true;

            // Instantiate the laser at the shoot point
            GameObject laser = Instantiate(laserPrefab, shootPoint.position, Quaternion.identity);

            // Initialize the movement script of the laser with speed and direction
            LaserMovement lm = laser.GetComponent<LaserMovement>();
            if (lm != null) lm.Initialize(laserSpeed, targetPosition);

            // Schedule the destruction of the laser
            Destroy(laser, laserLifetime);

            // RESET SIGNAL: Notify the Target system to move or reset its state
            if (EventSystem.EventSystemInstance != null)
                EventSystem.EventSystemInstance.TriggerTargetReset();

            Debug.Log("<color=red>UFO: Shot fired and Reset requested!</color>");
        }
    }
}