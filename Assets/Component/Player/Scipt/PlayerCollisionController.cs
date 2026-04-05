using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Vector3 _sphereCenter = new Vector3(0, 0.3f, 0);
    [SerializeField] private float _sphereRadius = 1.0f;

    [Header("Invincibilité")]
    [SerializeField] private float _damageCooldown = 1.5f;
    private float _nextDamageTime = 0f;

    // Propriété pour que CounterObstacle vérifie l'attaque
    public PlayerMovementController Movement { get; private set; }

    private void Start()
    {
        Movement = GetComponent<PlayerMovementController>();
    }

    private Vector3 PlayerSpherePosition => transform.position + _sphereCenter;

    void Update()
    {
        // Détection par OverlapSphere (image 3, 5)
        Collider[] hitColliders = Physics.OverlapSphere(PlayerSpherePosition, _sphereRadius, -1, QueryTriggerInteraction.Collide);

        foreach (var hit in hitColliders)
        {
            // Système d'héritage
            CollidableObject collidable = hit.GetComponent<CollidableObject>();
            if (collidable != null)
            {
                collidable.OnPlayerHit(this);
            }

            // Zone LaserTarget (image 5)
            if (hit.CompareTag("LaserTarget"))
            {
                EventSystem.EventSystemInstance?.TriggerPlayerOnTarget(hit.transform.position);
            }
        }
    }

    // Cette fonction règle ton erreur CS1061 précédente
    public void ApplyDamageToPlayer()
    {
        if (Time.time >= _nextDamageTime)
        {
            _nextDamageTime = Time.time + _damageCooldown;
            EventSystem.EventSystemInstance?.TriggerPlayerHit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(PlayerSpherePosition, _sphereRadius);
    }
}