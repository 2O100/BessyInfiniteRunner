using UnityEngine;

public class DungBallRotation : MonoBehaviour
{
    [Header("Contrôle de la Rotation")]
    [Tooltip("Vitesse sur X (Horizontal). Valeur négative pour inverser le sens.")]
    [SerializeField] private float _rotationSpeedX = 90f;

    [Tooltip("Vitesse sur Y (Vertical). Valeur négative pour inverser le sens.")]
    [SerializeField] private float _rotationSpeedY = 180f;

    [Tooltip("Vitesse sur Z (Profondeur).")]
    [SerializeField] private float _rotationSpeedZ = 0f;

    [Header("Contrôle du Bounce")]
    [SerializeField] private float _minY = 0.2f;
    [SerializeField] private float _maxY = 0.5f;
    [SerializeField] private float _bounceFrequency = 4f;

    private void Update()
    {
        // 1. ROTATION MULTI-AXES
        // On crée un vecteur de rotation basé sur tes réglages Unity
        Vector3 rotationVector = new Vector3(_rotationSpeedX, _rotationSpeedY, _rotationSpeedZ);

        // On applique la rotation (Space.Self est important pour ne pas s'emmêler)
        transform.Rotate(rotationVector * Time.deltaTime, Space.Self);

        // 2. BOUNCE FLUIDE
        float sinus = Mathf.Sin(Time.time * _bounceFrequency);
        float normalizedSin = (sinus + 1f) / 2f;

        float newY = Mathf.Lerp(_minY, _maxY, normalizedSin);

        // Application de la hauteur locale
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
}