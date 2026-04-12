using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [Header("UFO Animation Settings")]
    [Tooltip("How fast the UFO oscillates from side to side.")]
    [SerializeField] private float _hoverSpeed = 2f;

    [Tooltip("The maximum distance the UFO moves to the left and right.")]
    [SerializeField] private float _hoverAmount = 5f;

    [Tooltip("The speed at which the UFO rotates around its vertical axis.")]
    [SerializeField] private float _rotationSpeed = 100f;

    void Update()
    {
        // 1. CONTINUOUS ROTATION
        // Rotates the object around the Y axis (up) based on rotation speed and frame time
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        // 2. SIDE-TO-SIDE HOVER MOVEMENT
        // Use a Sine wave to create a smooth back-and-forth oscillation on the X axis
        float xMove = Mathf.Sin(Time.time * _hoverSpeed) * _hoverAmount;

        // Apply the new X position while maintaining the current Y and Z positions
        // Note: This assumes the UFO's center is at X = 0.
        transform.position = new Vector3(xMove, transform.position.y, transform.position.z);

        // ... additional update logic (state management, etc.) can be added here
    }
}