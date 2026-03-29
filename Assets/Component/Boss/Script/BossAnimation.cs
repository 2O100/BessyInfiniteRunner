using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [Header("UFO Animations")]
    [SerializeField] private float _hoverSpeed = 2f;    // Vitesse du balancement
    [SerializeField] private float _hoverAmount = 5f;   // Distance du balancement (gauche/droite)
    [SerializeField] private float _rotationSpeed = 100f; // Vitesse de rotation sur lui-même

    void Update()
    {
        // 1. Rotation continue sur l'axe Y
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        // 2. Mouvement de va-et-vient (gauche à droite) avec un Sinus
        float xMove = Mathf.Sin(Time.time * _hoverSpeed) * _hoverAmount;
        // On applique le mouvement par rapport à sa position d'origine (ou tu peux l'ajuster selon ton setup)
        transform.position = new Vector3(xMove, transform.position.y, transform.position.z);

        // ... reste de ton code d'Update (gestion des états, etc.)
    }
}