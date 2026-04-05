using UnityEngine;

public class LaserTargetMovement : MonoBehaviour
{
    public enum TargetState { Inactive, Targeting, Fire }

    [Header("État Actuel")]
    public TargetState currentState = TargetState.Inactive;

    [Header("Réglages")]
    public float targetingDuration = 5f; // Temps de clignotement
    public float fireDuration = 2f;      // Fenêtre de tir max avant reset auto
    public float flashSpeed = 0.2f;

    [Header("Références")]
    public UFOLaserShooter ufo;
    public Transform[] targetSlides;
    public AudioSource alertAudio;

    private SpriteRenderer _sprite;
    private SphereCollider _collider;
    private float _stateTimer;
    private float _flashTimer;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        // On écoute le signal de reset venant de l'UFO
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnTargetReset += ForceExternalReset;

        TransitionToState(TargetState.Targeting);
    }

    private void OnDisable()
    {
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnTargetReset -= ForceExternalReset;
    }

    private void Update()
    {
        _stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case TargetState.Targeting:
                UpdateTargeting();
                break;
            case TargetState.Fire:
                UpdateFire();
                break;
        }
    }

    private void TransitionToState(TargetState newState)
    {
        currentState = newState;
        _stateTimer = 0f;

        if (newState == TargetState.Targeting)
        {
            // Téléportation aléatoire sur une slide
            if (targetSlides != null && targetSlides.Length > 0)
                transform.position = targetSlides[Random.Range(0, targetSlides.Length)].position;

            _collider.enabled = false;
            if (ufo != null) ufo.SetAttackWindow(false);
            if (alertAudio != null) alertAudio.Play();
        }
        else if (newState == TargetState.Fire)
        {
            _sprite.enabled = true;
            _collider.enabled = true;
            if (ufo != null) ufo.SetAttackWindow(true);
            if (alertAudio != null) alertAudio.Stop();
        }
    }

    private void UpdateTargeting()
    {
        _flashTimer += Time.deltaTime;
        if (_flashTimer >= flashSpeed)
        {
            _sprite.enabled = !_sprite.enabled;
            _flashTimer = 0;
        }

        if (_stateTimer >= targetingDuration)
            TransitionToState(TargetState.Fire);
    }

    private void UpdateFire()
    {
        // RESET AUTO : Si le temps expire sans que le joueur déclenche un tir
        if (_stateTimer >= fireDuration)
            TransitionToState(TargetState.Targeting);
    }

    public void ForceExternalReset()
    {
        // Appelé via l'EventSystem quand l'UFO a fini de tirer
        TransitionToState(TargetState.Targeting);
    }
}