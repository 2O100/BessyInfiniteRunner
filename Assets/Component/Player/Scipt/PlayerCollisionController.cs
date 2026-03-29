using UnityEngine;
using UnityEngine.InputSystem; // Pour utiliser ton Input System

namespace Components.Player.Scripts
{
    public class PlayerCollisionController : MonoBehaviour
    {
        [Header("References")]
        // On lie ton script de mouvement pour lire l'action d'attaque
        [SerializeField] private PlayerMovementController _playerMovement;

        [Header("Parameters")]
        [SerializeField] private Vector3 _sphereCenter;
        [SerializeField] private float _sphereRadius;
        [SerializeField] private LayerMask _projectileLayer; // Pour ne cibler que les projectiles

        private bool _isHit;

        // La propriété magique qui calcule la position de ta sphère en temps réel
        private Vector3 PlayerSpherePosition => transform.position + _sphereCenter;

        private void Start()
        {
            // On s'abonne à l'événement de ton prof (si tu as la fonction ShrinkCollider)
            // EventSystem.EventSystem.OnPlayerSlideDown += ShrinkCollider;
        }

        private void Update()
        {
            // 1. SCAN : On crée la sphère de détection (le radar rouge)
            Collider[] hitColliders = Physics.OverlapSphere(PlayerSpherePosition, _sphereRadius, _projectileLayer);

            // 2. DETECTION : Si la sphère touche quelque chose
            if (hitColliders.Length > 0)
            {
                foreach (var hit in hitColliders)
                {
                    // Est-ce que l'objet touché est un projectile du Boss ?
                    BossProjectile proj = hit.GetComponent<BossProjectile>();

                    if (proj != null)
                    {
                        // 3. RENVOI : Si le joueur appuie sur son bouton d'attaque (Input System)
                        // Remplace .attackAction par le nom exact de ta variable dans PlayerMovementController
                        if (_playerMovement.Attack.action.triggered)
                        {
                            proj.Reflect(); // On appelle la fonction de renvoi
                            Debug.Log("PROJECTILE RENVOYÉ !");
                        }
                    }
                }

                // Anti-spam : on marque qu'on a touché quelque chose
                if (!_isHit)
                {
                    Debug.Log("Player hit something!");
                    _isHit = true;
                }
            }
            else
            {
                // Si plus rien dans la sphère, on reset le flag
                _isHit = false;
            }
        }

        // Dessine la sphère rouge dans l'éditeur pour t'aider à régler la zone
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PlayerSpherePosition, _sphereRadius);
        }

    }
}