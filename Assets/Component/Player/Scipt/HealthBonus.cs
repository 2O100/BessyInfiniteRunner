using UnityEngine;

public class HealthBonus : CollidableObject
{
    [Header("Bonus Settings")]
    [SerializeField] private int healthAmount = 1; // Amount of health restored to the player
    [SerializeField] private float rotationSpeed = 100f; // Rotation speed in degrees per second

    // Update is called once per frame, handling visual feedback independently of physics
    private void Update()
    {
        // Rotates the object around its vertical (Y) axis for a "pickup" visual effect
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }

    // Triggered when the player collides with this bonus object
    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // Access the GameManager singleton to heal the player
        if (GameManager.Instance != null)
        {
            // Note: Sending a negative value to TakeDamage acts as healing.
            // Ensure the GameManager clamps health so it doesn't exceed the maximum!
            GameManager.Instance.TakeDamage(-healthAmount);

            Debug.Log("<color=green>BONUS: +1 HP restored!</color>");
        }

        // Destroy the bonus object after it has been collected
        Destroy(gameObject);
    }
}