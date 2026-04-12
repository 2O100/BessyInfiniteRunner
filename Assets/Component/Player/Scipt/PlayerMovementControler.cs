using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Jump Parameters")]
    [SerializeField] private float _jumpDuration = 1f;   // Total time for the jump cycle
    [SerializeField] private float _jumpHeight = 3f;     // Peak height of the jump
    [SerializeField] private AnimationCurve _jumpCurve;  // Curve for the upward movement
    [SerializeField] private AnimationCurve _fallCurve;  // Curve for the downward movement

    [Header("Slide Parameters")]
    [SerializeField] private float _slideDuration = 0.5f; // Time to move between lanes
    [SerializeField] private AnimationCurve _slideCurve;  // Easing curve for lane switching
    [SerializeField] public Transform[] _sideTarget;      // Positions of the different lanes

    [Header("Setup & State")]
    [SerializeField] public int _currentLaneIndex = 2;    // Current lane (usually starts in the middle)
    private bool _isSliding;                              // Is the player currently switching lanes?
    private bool _isJumping;                              // Is the player currently in the air?
    public bool IsAttacking => _isAttacking;              // Public getter for combat state
    private bool _isAttacking;                            // Internal combat state flag

    [Header("Input References")]
    public InputActionReference Move;   // Input for horizontal movement
    public InputActionReference Jump;   // Input for jumping
    public InputActionReference Attack; // Input for the dash/parry attack
    public InputActionReference Mega;   // Reserved for special ability


    private void OnEnable()
    {
        // Enable each action so Unity starts listening for inputs
        Move.action.Enable();
        Jump.action.Enable();
        Attack.action.Enable();
        Mega.action.Enable();
    }

    private void Update()
    {
        // --- JUMP LOGIC ---
        // Check if the jump action was triggered this frame
        if (Jump.action.triggered)
        {
            if (!_isJumping) StartCoroutine(JumpRoutine());
        }

        // --- ATTACK & MEGA LOGIC ---
        if (Attack.action.triggered && !_isAttacking)
        {
            Debug.Log("Attack dash triggered!");
            StartCoroutine(AttackZSequence());
        }

        if (Mega.action.triggered) Debug.Log("Mega ability triggered!");

        // --- MOVEMENT LOGIC (LANE SWITCHING) ---
        // Prevent starting a new slide if already sliding
        if (_isSliding) return;

        // Read the Vector2 value from the Move action
        Vector2 moveVec = Move.action.ReadValue<Vector2>();

        if (Move.action.triggered)
        {
            // Move Left (Q / Left Arrow)
            if (moveVec.x < -0.1f && _currentLaneIndex > 0)
            {
                _currentLaneIndex--;
                StartCoroutine(SlideRoutine(_sideTarget[_currentLaneIndex]));
            }
            // Move Right (D / Right Arrow)
            else if (moveVec.x > 0.1f && _currentLaneIndex < _sideTarget.Length - 1)
            {
                _currentLaneIndex++;
                StartCoroutine(SlideRoutine(_sideTarget[_currentLaneIndex]));
            }
        }
    }

    // Handles the vertical jump logic using a two-phase Coroutine
    private IEnumerator JumpRoutine()
    {
        _isJumping = true;
        float jumpTimer = 0f;
        float halfJumpDuration = _jumpDuration / 2f;
        float startY = transform.position.y;

        // PHASE 1: Ascending
        while (jumpTimer < halfJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            float normalTime = jumpTimer / halfJumpDuration;
            float height = _jumpCurve.Evaluate(normalTime) * _jumpHeight;
            transform.position = new Vector3(transform.position.x, startY + height, transform.position.z);
            yield return null;
        }

        // PHASE 2: Descending
        jumpTimer = 0f;
        while (jumpTimer < halfJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            float normalTime = jumpTimer / halfJumpDuration;
            float height = _fallCurve.Evaluate(normalTime) * _jumpHeight;
            transform.position = new Vector3(transform.position.x, startY + height, transform.position.z);
            yield return null;
        }

        _isJumping = false;
    }

    // Smoothly interpolates the player position between lanes
    private IEnumerator SlideRoutine(Transform target)
    {
        _isSliding = true;
        float slideTimer = 0f;
        Vector3 startPos = transform.position;

        while (slideTimer < _slideDuration)
        {
            slideTimer += Time.deltaTime;
            float normalTime = slideTimer / _slideDuration;

            // Calculate the lateral movement (X) based on the easing curve
            float newX = Mathf.Lerp(startPos.x, target.position.x, _slideCurve.Evaluate(normalTime));

            // Apply movement while keeping existing Y (for jumps) and Z (for attacks)
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            yield return null;
        }

        // Snapping to final position to prevent precision errors
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        _isSliding = false;
    }

    // Visual and functional dash sequence on the Z axis (Forward/Backward)
    // This allows parrying projectiles without stopping lateral movement
    private IEnumerator AttackZSequence()
    {
        _isAttacking = true; // Enables parry protection

        float startZ = transform.position.z;
        float elapsed = 0f;

        // --- PHASE 1: FORWARD DASH ---
        while (elapsed < 0.05f)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.Lerp(startZ, startZ + 0.5f, elapsed / 0.05f);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return null;
        }

        // --- PHASE 2: BACKWARD RECOIL ---
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.Lerp(startZ + 0.5f, startZ - 0.5f, elapsed / 0.1f);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return null;
        }

        // --- PHASE 3: RETURNING TO IDLE Z ---
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.Lerp(startZ - 0.5f, startZ, elapsed / 0.1f);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return null;
        }

        // Snap back to original Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, startZ);

        _isAttacking = false; // Disables parry protection
    }
}