using UnityEngine;

public class DungBallRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed on X axis (Horizontal). Use negative values to reverse direction.")]
    [SerializeField] private float _rotationSpeedX = 90f;

    [Tooltip("Rotation speed on Y axis (Vertical). Use negative values to reverse direction.")]
    [SerializeField] private float _rotationSpeedY = 180f;

    [Tooltip("Rotation speed on Z axis (Depth).")]
    [SerializeField] private float _rotationSpeedZ = 0f;

    [Header("Bounce Settings")]
    [SerializeField] private float _minY = 0.2f;            // Minimum bounce height
    [SerializeField] private float _maxY = 0.5f;            // Maximum bounce height
    [SerializeField] private float _bounceFrequency = 4f;   // How fast it bounces up and down

    private void Update()
    {
        // 1. MULTI-AXIS ROTATION
        // Create a rotation vector based on the Inspector settings
        Vector3 rotationVector = new Vector3(_rotationSpeedX, _rotationSpeedY, _rotationSpeedZ);

        // Apply the rotation over time (Space.Self ensures rotation is relative to the object)
        transform.Rotate(rotationVector * Time.deltaTime, Space.Self);

        // 2. SMOOTH BOUNCE LOGIC
        // Create a sine wave oscillation
        float sinus = Mathf.Sin(Time.time * _bounceFrequency);

        // Normalize the sine wave from [-1, 1] to [0, 1]
        float normalizedSin = (sinus + 1f) / 2f;

        // Interpolate between min and max height based on the normalized sine value
        float newY = Mathf.Lerp(_minY, _maxY, normalizedSin);

        // Apply the calculated height to the local position
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
}