using UnityEngine;

public class UFOLaserShooter : MonoBehaviour
{
    [Header("Prefab & Points")]
    public GameObject laserPrefab;   // Ton cylindre rouge (Prefab)
    public Transform shootPoint;    // L'objet vide placé sous l'UFO

    [Header("Laser Physics")]
    public float laserSpeed = 150f;   // Vitesse rapide pour un effet "éclair"
    public float laserLifetime = 1.5f; // Durée de vie du laser

    private bool _hasShotThisCycle = false; // Empêche les clones multiples
    private bool _canShootNow = false;      // Autorise le tir seulement en phase rouge

    private void OnEnable()
    {
        // On s'abonne au signal du joueur via l'EventSystem
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerOnTarget += TryShoot;

        _hasShotThisCycle = false;
        _canShootNow = false;
    }

    private void OnDisable()
    {
        // On se désabonne pour éviter les erreurs de mémoire
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnPlayerOnTarget -= TryShoot;
    }

    /// <summary>
    /// Appelé par le script LaserTargetMovement pour ouvrir/fermer la fenêtre de tir.
    /// </summary>
    public void SetAttackWindow(bool isOpen)
    {
        _canShootNow = isOpen;

        // Si on ouvre une nouvelle fenêtre, on autorise un nouveau tir
        if (isOpen)
        {
            _hasShotThisCycle = false;
        }
    }

    /// <summary>
    /// Tentative de tir déclenchée par le signal du joueur.
    /// </summary>
    public void TryShoot(Vector3 targetPosition)
    {
        // CONDITIONS : La fenêtre doit être ouverte ET on ne doit pas avoir déjà tiré
        if (!_canShootNow || _hasShotThisCycle) return;

        if (laserPrefab != null && shootPoint != null)
        {
            _hasShotThisCycle = true; // On verrouille immédiatement pour éviter les clones

            // 1. Création du laser au point de sortie
            GameObject laser = Instantiate(laserPrefab, shootPoint.position, Quaternion.identity);

            // 2. Initialisation du mouvement vers la cible
            LaserMovement lm = laser.GetComponent<LaserMovement>();
            if (lm != null)
            {
                // On envoie la vitesse et la position exacte du LaserTarget au sol
                lm.Initialize(laserSpeed, targetPosition);
            }
            else
            {
                Debug.LogError("Le prefab Laser n'a pas le script LaserMovement !");
            }

            // 3. Nettoyage
            Destroy(laser, laserLifetime);

            Debug.Log("<color=red>UFO : Tir directionnel vers la cible effectué !</color>");
        }
    }
}