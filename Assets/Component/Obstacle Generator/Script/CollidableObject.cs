using UnityEngine;

/// <summary>
/// This is the Base Class for all objects that can collide with the player.
/// It uses "Inheritance" so the Player script doesn't need to know exactly 
/// what it's hitting (Obstacle, Bonus, Firefly, etc.) to trigger an effect.
/// </summary>
public abstract class CollidableObject : MonoBehaviour
{
    /// <summary>
    /// This method must be implemented by any script inheriting from CollidableObject.
    /// It defines the specific behavior (damage, heal, score) when the PlayerCollisionController 
    /// interacts with this object.
    /// </summary>
    /// <param name="player">The controller of the player that hit this object.</param>
    public abstract void OnPlayerHit(PlayerCollisionController player);
}