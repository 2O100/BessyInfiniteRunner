using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [SerializeField] private Transform _endAnchor;
    public Vector3 EndAnchor => _endAnchor.position;

    public bool IsBehindPlayer()
    {
        return EndAnchor.z <= 0;
    }

    private float speedMultiplier = 1f; // Multiplicateur de vitesse

    // Méthode pour définir le multiplicateur de vitesse
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    void Update()
    {
        // Exemple de déplacement du chunk
        transform.Translate(Vector3.back * Time.deltaTime * speedMultiplier);
    }
}