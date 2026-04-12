using UnityEngine;

public class BeeAnimation : MonoBehaviour
{
    [Header("Cibles de mouvement")]
    [SerializeField] private Transform targetLeft;  
    [SerializeField] private Transform targetRight; 

    [Header("Réglages")]
    [SerializeField] private float speed = 2f;      
    [SerializeField] private float smoothness = 0.1f; 

    private Vector3 _currentTarget;
    private Quaternion _lookRight = Quaternion.Euler(0, 180, 0);
    private Quaternion _lookLeft = Quaternion.Euler(0, 0, 0);

    private void Start()
    {
        // Left
        if (targetLeft != null)
        {
            _currentTarget = targetLeft.position;
            transform.rotation = _lookLeft;
        }
    }

    private void Update()
    {
        if (targetLeft == null || targetRight == null) return;

        // 1. Déplacement vers la cible actuelle (uniquement sur l'axe X local du chunk)
        float step = speed * Time.deltaTime;

        // On déplace l'abeille vers la cible
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, step);

        // 2. Vérification : est-on arrivé à destination ?
        if (Vector3.Distance(transform.position, _currentTarget) < 0.1f)
        {
            // On change de cible et de rotation
            if (_currentTarget == targetLeft.position)
            {
                _currentTarget = targetRight.position;
                transform.rotation = _lookRight;
            }
            else
            {
                _currentTarget = targetLeft.position;
                transform.rotation = _lookLeft;
            }
        }
    }
}