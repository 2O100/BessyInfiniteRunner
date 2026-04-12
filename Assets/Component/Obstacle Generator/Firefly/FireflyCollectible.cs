using UnityEngine;

/// <summary>
/// Script for the firefly collectible items.
/// Communicates via the EventSystem to update global score.
/// </summary>
public class FireflyCollectible : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The points value of this specific firefly.")]
    [SerializeField] private int _scoreValue = 1;

    [Header("Visual Feedback")]
    [Tooltip("Optional particle effect to spawn on collection.")]
    [SerializeField] private GameObject _collectEffect;

    /// <summary>
    /// Triggered by the PlayerCollisionController.
    /// Sends the score value to the EventSystem and destroys the object.
    /// </summary>
    public void OnPlayerHit(PlayerCollisionController player)
    {
        // POINT 4: Decoupled scoring logic using the EventSystem
        if (EventSystem.EventSystemInstance != null)
        {
            // Now this method is correctly found in EventSystem
            EventSystem.EventSystemInstance.TriggerFireflyCollected(_scoreValue);
        }

        // Optional: Spawn particles
        if (_collectEffect != null)
        {
            Instantiate(_collectEffect, transform.position, Quaternion.identity);
        }

        // Remove the firefly from the world
        Destroy(gameObject);

        Debug.Log($"<color=yellow>FIREFLY: Collected! Signal sent to EventSystem (+{_scoreValue}).</color>");
    }
}