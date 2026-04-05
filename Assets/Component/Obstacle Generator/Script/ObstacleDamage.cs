public class ObstacleDamage : CollidableObject
{
    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // On appelle la méthode de dégât du joueur
        player.ApplyDamageToPlayer();
    }
}