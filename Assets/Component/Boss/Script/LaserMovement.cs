using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;

    // Sets up the initial parameters for the laser projectile
    public void Initialize(float speed, Vector3 targetPos)
    {
        _speed = speed;

        // Calculate normalized direction vector towards the target position
        _direction = (targetPos - transform.position).normalized;
    }

    void Update()
    {
        // Move the laser forward in the calculated direction
        transform.position += _direction * _speed * Time.deltaTime;

        // Ensure the laser is facing its travel direction
        if (_direction != Vector3.zero)
        {
            // Set the forward vector to match movement direction
            transform.forward = _direction;

            // Adjust rotation by 90 degrees on X axis. 
            // This is often needed if using a Cylinder primitive as a laser.
            transform.Rotate(90, 0, 0);
        }
    }
}