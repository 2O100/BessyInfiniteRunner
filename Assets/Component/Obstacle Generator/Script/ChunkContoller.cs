using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [SerializeField] private Transform _endAnchor; // Point used to detect when the chunk has passed the player

    // Public property to easily access the end anchor's world position
    public Vector3 EndAnchor => _endAnchor.position;

    // Checks if the chunk has fully passed the player (Z coordinate <= 0)
    public bool IsBehindPlayer()
    {
        return EndAnchor.z <= 0;
    }

    private float speedMultiplier = 1f; // Speed multiplier for world scrolling

    // Sets a new speed multiplier to accelerate or decelerate the movement
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}