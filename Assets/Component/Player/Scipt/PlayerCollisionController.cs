using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Center offset of the detection sphere relative to the player's pivot.")]
    [SerializeField] private Vector3 _sphereCenter = new Vector3(0, 0.3f, 0);
    [SerializeField] private float _sphereRadius = 1.0f; // Radius of the detection zone

    [Header("Invincibility Settings")]
    [SerializeField] private float _damageCooldown = 1.5f; // Time in seconds between two possible hits
    private float _nextDamageTime = 0f;

    // Property allowing other scripts (like CounterObstacle) to check player movement state
    public PlayerMovementController Movement { get; private set; }

    private void Start()
    {
        // Cache the reference to the movement controller on start
        Movement = GetComponent<PlayerMovementController>();
    }

    // Helper to calculate the sphere's world position
    private Vector3 PlayerSpherePosition => transform.position + _sphereCenter;

    void Update()
    {
        // Detection using OverlapSphere: Checks all colliders within the radius
        // QueryTriggerInteraction.Collide ensures we also detect objects set as "Is Trigger"
        Collider[] hitColliders = Physics.OverlapSphere(PlayerSpherePosition, _sphereRadius, -1, QueryTriggerInteraction.Collide);

        foreach (var hit in hitColliders)
        {
            // 1. COLLIDABLE OBJECT SYSTEM (Inheritance)
            // Checks if the hit object inherits from CollidableObject (Firefly, Health, etc.)
            CollidableObject collidable = hit.GetComponent<CollidableObject>();
            if (collidable != null)
            {
                collidable.OnPlayerHit(this);
            }

            // 2. LASER TARGET ZONE
            // Specifically checks for the LaserTarget tag to trigger UFO fire logic
            if (hit.CompareTag("LaserTarget"))
            {
                EventSystem.EventSystemInstance?.TriggerPlayerOnTarget(hit.transform.position);
            }
        }
    }

    // Centralized method to handle player damage and invincibility frames
    public void ApplyDamageToPlayer()
    {
        // Only take damage if the cooldown has expired
        if (Time.time >= _nextDamageTime)
        {
            _nextDamageTime = Time.time + _damageCooldown;

            // Notify the EventSystem to reduce health and update UI
            EventSystem.EventSystemInstance?.TriggerPlayerHit();

            Debug.Log("<color=red>PLAYER: Damage taken! Invincibility started.</color>");
        }
    }

    // Visual feedback in the Unity Editor to help adjust the detection zone
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(PlayerSpherePosition, _sphereRadius);
    }
}