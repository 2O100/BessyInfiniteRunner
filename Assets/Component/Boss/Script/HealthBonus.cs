using UnityEngine;

public class HealthBonus : CollidableObject
{
    [SerializeField] private int healthAmount = 1;

    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // On demande au GameManager de soigner le joueur
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(-healthAmount);
            // Note : On envoie une valeur négative à TakeDamage pour soigner, 
            // ou tu peux créer une fonction Heal() dans ton GameManager.
        }

        // On détruit l'objet bonus après la collision
        Destroy(gameObject);
    }
}