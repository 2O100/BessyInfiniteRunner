using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Vector3 _sphereCenter = new Vector3(0, 0.2f, 0);
    [SerializeField] private float _sphereRadius = 1.0f;

    [Header("Invincibilité")]
    [SerializeField] private float _damageCooldown = 1.5f; // Temps en secondes avant de pouvoir reprendre un coup
    private float _nextDamageTime = 0f; // Moment où le prochain dégât sera autorisé

    private Vector3 PlayerSpherePosition => transform.position + _sphereCenter;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(PlayerSpherePosition, _sphereRadius, ~0, QueryTriggerInteraction.Collide);

        foreach (var hit in hitColliders)
        {
            // On vérifie le Tag ET si le temps actuel a dépassé le temps d'attente
            if (hit.CompareTag("Obstacle") || hit.CompareTag("Laser"))
            {
                if (Time.time >= _nextDamageTime)
                {
                    Debug.Log("<color=red>DÉGÂT VALIDÉ !</color>");

                    // On enregistre le moment du prochain dégât possible
                    _nextDamageTime = Time.time + _damageCooldown;

                    if (EventSystem.EventSystemInstance != null)
                    {
                        EventSystem.EventSystemInstance.TriggerPlayerHit();
                    }
                }
                else
                {
                    // Optionnel : un log pour vérifier que le cooldown fonctionne
                    // Debug.Log("Collision ignorée (en cours d'invincibilité)");
                }
            }

            if (hit.CompareTag("LaserTarget"))
            {
                if (EventSystem.EventSystemInstance != null)
                {
                    EventSystem.EventSystemInstance.TriggerPlayerOnTarget(hit.transform.position);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(PlayerSpherePosition, _sphereRadius);
    }
}