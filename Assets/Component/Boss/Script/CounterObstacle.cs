using UnityEngine;

public class CounterObstacle : CollidableObject
{
    [Header("Reflect Settings")]
    [SerializeField] private float _returnSpeed = 40f; // Speed at which the dung ball returns to the boss

    private bool _isCountered = false; // Tracks if the projectile has been successfully parried
    private Transform _target;         // Reference to the boss's hit point

    private void Update()
    {
        // If the dung ball is countered, move it towards the boss target
        if (_isCountered && _target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, _returnSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // IMPORTANT: We only check for the Boss tag if the dung ball has been countered first
        if (_isCountered && other.CompareTag("Boss"))
        {
            Debug.Log("<color=green>[SUCCESS] Impact on Boss!</color>");
            ApplyDamageToBoss();
        }
    }

    // Triggered by the base CollidableObject system when the player touches the dung ball
    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // Ignore further player collisions if the dung ball is already returning
        if (_isCountered) return;

        // Check if the player is currently in an attacking state (Dash/Attack)
        if (player.Movement != null && player.Movement.IsAttacking)
        {
            HandleCounter();
        }
        else
        {
            // If player is not attacking, apply normal damage and destroy projectile
            Debug.Log("<color=red>[FAIL] Player hit by dung ball!</color>");
            player.ApplyDamageToPlayer();
            Destroy(gameObject);
        }
    }

    // Logic to flip the projectile's direction towards the boss
    private void HandleCounter()
    {
        // Retrieve the target point from the GameManager singleton
        if (GameManager.Instance != null && GameManager.Instance.bossShootPoint != null)
        {
            _target = GameManager.Instance.bossShootPoint;
            _isCountered = true;

            Debug.Log("<color=yellow>[Counter] Dung ball returned towards: </color>" + _target.name);

            // Disable visual rotation script to give visual impact feedback
            if (TryGetComponent(out DungBallRotation rot)) rot.enabled = false;

            // Briefly disable the collider and re-enable it to prevent instant re-collision with player
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
                Invoke("EnableCollider", 0.1f);
            }
        }
    }

    // Helper method for the Invoke call
    private void EnableCollider() => GetComponent<Collider>().enabled = true;

    // Logic to communicate the successful hit to the game loop
    private void ApplyDamageToBoss()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("<color=cyan>[Counter] Sending damage point to GameManager...</color>");
            GameManager.Instance.TakeBossDamage(1);
        }
        else
        {
            // Error handling if the singleton is missing in the scene
            Debug.LogError("[Counter] ERROR: GameManager.Instance not found!");
        }

        // Note: Destroy(gameObject) is usually called here after impact
    }

    // Debugging tool to track lifecycle of countered projectiles
    private void OnDestroy()
    {
        if (_isCountered)
        {
            Debug.Log("<color=orange>[DEBUG] Countered dung ball has been destroyed!</color>");
        }
    }
}