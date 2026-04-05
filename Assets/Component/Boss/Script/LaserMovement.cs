using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;

    public void Initialize(float speed, Vector3 targetPos)
    {
        _speed = speed;
        // Direction vers la cible
        _direction = (targetPos - transform.position).normalized;
    }

    void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;

        if (_direction != Vector3.zero)
        {
            transform.forward = _direction;
            transform.Rotate(90, 0, 0); // Ajuste selon l'orientation de ton cylindre
        }
    }
}