using UnityEngine;
using System.Collections;

public class LaserTargetMovement : MonoBehaviour
{
    [Header("Réglages Temps")]
    public float flashingDuration = 5f;
    public float activeDuration = 1f;
    public float flashSpeed = 0.2f;

    [Header("Références")]
    public UFOLaserShooter ufo;

    private SpriteRenderer _sprite;
    private SphereCollider _collider;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            // 1. PHASE PRÉPARATION (Clignotement)
            if (ufo != null) ufo.SetAttackWindow(false); // UFO interdit de tirer
            _collider.enabled = false;

            float t = 0;
            while (t < flashingDuration)
            {
                _sprite.enabled = !_sprite.enabled;
                yield return new WaitForSeconds(flashSpeed);
                t += flashSpeed;
            }

            // 2. PHASE ACTIVE (Le moment où le tir est possible)
            _sprite.enabled = true;
            _collider.enabled = true;

            if (ufo != null) ufo.SetAttackWindow(true); // ON OUVRE LA FENÊTRE DE TIR

            yield return new WaitForSeconds(activeDuration);

            // 3. PHASE REPOS
            if (ufo != null) ufo.SetAttackWindow(false); // ON REFERME LA FENÊTRE
            _collider.enabled = false;
            _sprite.enabled = false;
            yield return new WaitForSeconds(1f);
        }
    }
}