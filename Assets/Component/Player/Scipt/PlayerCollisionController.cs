using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    [SerializeField] private Vector3 _sphereCenter;
    [SerializeField] private float _sphereRadius = 0.5f;

    private Vector3 PlayerSpherePosition => transform.position + _sphereCenter;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(PlayerSpherePosition, _sphereRadius);

        foreach (var hit in hitColliders)
        {
            // Le signal est envoyé en continu, mais l'UFO l'ignorera après le 1er laser
            if (hit.CompareTag("LaserTarget"))
            {
                if (EventSystem.EventSystemInstance != null)
                {
                    EventSystem.EventSystemInstance.TriggerPlayerOnTarget(hit.transform.position);
                }
            }
        }
    }
}