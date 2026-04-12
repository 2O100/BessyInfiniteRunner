using UnityEngine;

public class HealthBonus : CollidableObject
{
    [Header("Bonus Settings")]
    [SerializeField] private int healthAmount = 1;
    [SerializeField] private float rotationSpeed = 100f; // Vitesse de rotation

    // L'Update tourne à chaque frame, indépendamment de la collision
    private void Update()
    {
        // Fait tourner l'objet sur son axe Y (vertical)
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }

    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // On demande au GameManager de soigner le joueur
        if (GameManager.Instance != null)
        {
            // Astuce : TakeDamage(-1) fonctionne, mais s'assurer que ton GameManager 
            // n'empêche pas la vie de remonter au-delà du max !
            GameManager.Instance.TakeDamage(-healthAmount);
            Debug.Log("<color=green>BONUS : +1 PV !</color>");
        }

        // On détruit l'objet bonus après la collision
        Destroy(gameObject);
    }
}