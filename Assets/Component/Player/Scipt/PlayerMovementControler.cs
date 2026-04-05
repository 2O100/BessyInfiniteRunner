using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Jump parameters")]
    [SerializeField] private float _jumpDuration = 1f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private AnimationCurve _fallCurve;

    [Header("Slide parameters")]
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private AnimationCurve _slideCurve;
    [SerializeField] public Transform[] _sideTarget;

    [Header("Setup")]
    [SerializeField] public int _currentLaneIndex = 2;
    private bool _isSliding;
    private bool _isJumping;
    public bool IsAttacking => _isAttacking;
    private bool _isAttacking;

    [Header("Input References")]
    public InputActionReference Move;
    public InputActionReference Jump;
    public InputActionReference Attack;
    public InputActionReference Mega;


    private void OnEnable()
    {
        // On active chaque action pour que Unity les écoute
        Move.action.Enable();
        Jump.action.Enable();
        Attack.action.Enable();
        Mega.action.Enable();
    }
    private void Update()
    {
        // --- JUMP ---
        // On utilise .action.triggered sur la référence
        if (Jump.action.triggered)
        {
            if (!_isJumping) StartCoroutine(JumpRoutine());
        }

        // --- ATTACK & MEGA ---
        if (Attack.action.triggered && !_isAttacking) 
        {
            Debug.Log("Attack!");
            StartCoroutine(AttackZSequence()); // Ajout de l'appel
        }
        if (Mega.action.triggered) Debug.Log("Mega!");

        // --- MOVEMENTS (SLIDE) ---
        if (_isSliding) return;

        // On lit la valeur Vector2 de l'action Move
        Vector2 moveVec = Move.action.ReadValue<Vector2>();

        if (Move.action.triggered)
        {
            // Vers la gauche (Q / Flèche Gauche)
            if (moveVec.x < -0.1f && _currentLaneIndex > 0)
            {
                _currentLaneIndex--;
                StartCoroutine(SlideRoutine(_sideTarget[_currentLaneIndex]));
            }
            // Vers la droite (D / Flèche Droite)
            else if (moveVec.x > 0.1f && _currentLaneIndex < _sideTarget.Length - 1)
            {
                _currentLaneIndex++;
                StartCoroutine(SlideRoutine(_sideTarget[_currentLaneIndex]));
            }
        }
    }

    private IEnumerator JumpRoutine()
    {
        _isJumping = true;
        float jumpTimer = 0f;
        float halfJumpDuration = _jumpDuration / 2f;
        float startY = transform.position.y;

        // Phase de montée
        while (jumpTimer < halfJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            float normalTime = jumpTimer / halfJumpDuration;
            float height = _jumpCurve.Evaluate(normalTime) * _jumpHeight;
            transform.position = new Vector3(transform.position.x, startY + height, transform.position.z);
            yield return null;
        }

        // Phase de descente
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

    private IEnumerator SlideRoutine(Transform target)
    {
        _isSliding = true;
        float slideTimer = 0f;
        Vector3 startPos = transform.position;

        while (slideTimer < _slideDuration)
        {
            slideTimer += Time.deltaTime;
            float normalTime = slideTimer / _slideDuration;

            // On calcule le mouvement latéral (X)
            float newX = Mathf.Lerp(startPos.x, target.position.x, _slideCurve.Evaluate(normalTime));
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            yield return null;
        }

        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        _isSliding = false;
    }

    private IEnumerator AttackZSequence()
    {
        _isAttacking = true; // On active la protection

        float startZ = transform.position.z;
        float elapsed = 0f;

        // --- PHASE 1 : AVANT ---
        while (elapsed < 0.05f)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.Lerp(startZ, startZ + 0.5f, elapsed / 0.05f);
            // On garde transform.position.x et .y actuels pour ne pas bloquer le Slide/Jump
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return null;
        }

        // --- PHASE 2 : ARRIÈRE ---
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.Lerp(startZ + 0.5f, startZ - 0.5f, elapsed / 0.1f);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return null;
        }

        // --- PHASE 3 : RETOUR ---
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.Lerp(startZ - 0.5f, startZ, elapsed / 0.1f);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return null;
        }

        // On remet le Z pile à sa place d'origine
        transform.position = new Vector3(transform.position.x, transform.position.y, startZ);

        _isAttacking = false; // On désactive la protection
    }
}