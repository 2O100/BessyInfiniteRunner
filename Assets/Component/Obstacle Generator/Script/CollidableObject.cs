using UnityEngine;

// This is the Base Class for all objects that can collide with the player.
// It uses "Inheritance" so the Player script doesn't need to know exactly 
// what it's hitting (Obstacle, Ammo, etc.) to trigger an effect.
public abstract class CollidableObject : MonoBehaviour
{
    /// <summary>
    /// This method must be implemented by any script inheriting from CollidableObject.
    /// It defines what happens when the PlayerCollisionController detects this object.
    public abstract void OnPlayerHit(PlayerCollisionController player);
}