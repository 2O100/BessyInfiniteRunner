using UnityEngine;

public class ObstacleDamage : CollidableObject
{
    // Implementation of the abstract method from CollidableObject
    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // Call the damage application method on the player controller
        // This is triggered when the player hits a standard static obstacle
        player.ApplyDamageToPlayer();
    }
}