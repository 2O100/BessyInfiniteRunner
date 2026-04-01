using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;

    // On initialise la direction au moment où l'UFO crée le laser
    public void Initialize(float speed, Vector3 targetPos)
    {
        _speed = speed;
        // On calcule le vecteur entre le point de départ et la cible
        _direction = (targetPos - transform.position).normalized;
    }

    void Update()
    {
        // On déplace le laser vers la cible dans l'espace du monde (World Space)
        transform.position += _direction * _speed * Time.deltaTime;

        // On l'oriente aussi visuellement vers cette direction
        if (_direction != Vector3.zero)
        {
            transform.forward = _direction;
            // Si ton cylindre est vertical, ajoute cette ligne pour le coucher :
            transform.Rotate(90, 0, 0);
        }
    }
}