using UnityEngine;

public class LaserTargetMovement : MonoBehaviour
{
    public enum TargetState { Inactive, Targeting, Fire }

    [Header("Current State")]
    public TargetState currentState = TargetState.Inactive;

    [Header("Settings")]
    public float targetingDuration = 5f; // Duration of the blinking phase
    public float fireDuration = 2f;      // Max window to shoot before auto-reset
    public float flashSpeed = 0.2f;      // Speed of the sprite flashing

    [Header("References")]
    public UFOLaserShooter ufo;          // Reference to the UFO shooter script
    public Transform[] targetSlides;     // Possible spawn positions (lanes)
    public AudioSource alertAudio;       // Sound played during targeting

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
        // Subscribe to the reset signal from the UFO via EventSystem
        if (EventSystem.EventSystemInstance != null)
            EventSystem.EventSystemInstance.OnTargetReset += ForceExternalReset;

        // Start targeting as soon as the object is enabled
        TransitionToState(TargetState.Targeting);
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks or errors
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
            // Randomly teleport to one of the target lanes
            if (targetSlides != null && targetSlides.Length > 0)
                transform.position = targetSlides[Random.Range(0, targetSlides.Length)].position;

            _collider.enabled = false; // Disable collision during targeting
            if (ufo != null) ufo.SetAttackWindow(false); // UFO cannot shoot yet
            if (alertAudio != null) alertAudio.Play();
        }
        else if (newState == TargetState.Fire)
        {
            // Transition to firing window
            _sprite.enabled = true;
            _collider.enabled = true; // Enable collision so player can trigger the shot
            if (ufo != null) ufo.SetAttackWindow(true); // UFO is now ready to fire
            if (alertAudio != null) alertAudio.Stop();
        }
    }

    private void UpdateTargeting()
    {
        // Handle flashing visual effect
        _flashTimer += Time.deltaTime;
        if (_flashTimer >= flashSpeed)
        {
            _sprite.enabled = !_sprite.enabled;
            _flashTimer = 0;
        }

        // Switch to Fire state once duration is reached
        if (_stateTimer >= targetingDuration)
            TransitionToState(TargetState.Fire);
    }

    private void UpdateFire()
    {
        // AUTO RESET: If the firing window expires without player interaction
        if (_stateTimer >= fireDuration)
            TransitionToState(TargetState.Targeting);
    }

    // Called via EventSystem when the UFO has finished its firing sequence
    public void ForceExternalReset()
    {
        TransitionToState(TargetState.Targeting);
    }
}