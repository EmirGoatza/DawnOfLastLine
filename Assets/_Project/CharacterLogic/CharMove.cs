using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharStam))] 
public class CharMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Jump & Dash Physics")]
    public float jumpHeight = 1.5f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    
    [Header("Stamina Costs")]
    public float jumpCost = 1f;
    public float dashCost = 1f;

    [Header("State")]
    public bool IsRunning { get; private set; }
    [HideInInspector] public bool canMove = true;

    [Header("References")]
    public Animator animator;

    private CharacterController controller;
    private CharStam charStam;
    private Health health;
    private EnemyTarget enemyTarget;
    private Transform cam;

    private float verticalVelocity;
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity; 
    private float smoothInputSpeed = 0.1f;

    // Logique Dash/Sprint
    private float sprintButtonDownTime;
    private bool sprintButtonHeld;
    private float lastDashTime;
    private bool isDashing;
    private const float SPRINT_THRESHOLD = 0.25f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        charStam = GetComponent<CharStam>();
        enemyTarget = GetComponent<EnemyTarget>();

        if (animator == null) animator = GetComponent<Animator>();
        if (Camera.main != null) cam = Camera.main.transform;
        
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    void Update()
    {
        if(health != null && health.IsDead) return;
        if (isDashing) return;

        ApplyGravity();

        if (!canMove)
        {
            controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
            UpdateAnimatorParameters();
            return; 
        }

        HandleInput();
        ApplyMovement();
        UpdateAnimatorParameters();
    }
    
    void HandleInput()
    {
        Vector2 targetInput = Vector2.zero;
        bool jumpPressed = false;
        
        bool sprintBtnDown = false;
        bool sprintBtnUp = false;
        bool sprintBtnIsPressed = false;

        // --- CLAVIER ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) targetInput.y = 1;
            if (Keyboard.current.sKey.isPressed) targetInput.y = -1;
            if (Keyboard.current.dKey.isPressed) targetInput.x = 1;
            if (Keyboard.current.aKey.isPressed) targetInput.x = -1;
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame) jumpPressed = true;

            if (Keyboard.current.leftShiftKey.wasPressedThisFrame) sprintBtnDown = true;
            if (Keyboard.current.leftShiftKey.wasReleasedThisFrame) sprintBtnUp = true;
            if (Keyboard.current.leftShiftKey.isPressed) sprintBtnIsPressed = true;
        }

        // --- MANETTE ---
        if (Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            if (stickInput.magnitude > 0.1f) targetInput = stickInput;
            
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) jumpPressed = true;

            if (Gamepad.current.buttonEast.wasPressedThisFrame) sprintBtnDown = true;
            if (Gamepad.current.buttonEast.wasReleasedThisFrame) sprintBtnUp = true;
            if (Gamepad.current.buttonEast.isPressed) sprintBtnIsPressed = true;
        }

        // --- SAUT ---
        if (jumpPressed)
        {
            if (charStam.HasStamina(jumpCost))
            {
                charStam.UseStamina(jumpCost);
                Jump();
            }
        }

        // --- DASH ---
        if (sprintBtnDown)
        {
            sprintButtonDownTime = Time.time;
            sprintButtonHeld = true;
        }

        if (sprintBtnUp)
        {
            float holdDuration = Time.time - sprintButtonDownTime;
            sprintButtonHeld = false;
            IsRunning = false; 

            if (holdDuration < SPRINT_THRESHOLD)
            {
                if (charStam.HasStamina(dashCost))
                {
                    charStam.UseStamina(dashCost);
                    StartCoroutine(PerformDash());
                }
            }
        }

        if (sprintBtnIsPressed && (Time.time - sprintButtonDownTime > SPRINT_THRESHOLD))
        {
            IsRunning = true;
        }
        else if (!sprintBtnIsPressed)
        {
            IsRunning = false;
        }

        currentInputVector = Vector2.SmoothDamp(currentInputVector, targetInput, ref smoothInputVelocity, smoothInputSpeed);
    }

    void Jump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        if (animator != null) animator.SetTrigger("Jump");
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        if (animator != null) animator.SetTrigger("Dash");

        float startTime = Time.time;
        Vector3 dashDirection = transform.forward; 

        if (currentInputVector.magnitude > 0.1f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();

            dashDirection = (camForward * currentInputVector.y) + (camRight * currentInputVector.x);
            dashDirection.Normalize();
            transform.rotation = Quaternion.LookRotation(dashDirection);
        }

        verticalVelocity = 0f;

        while (Time.time < startTime + dashDuration)
        {
            controller.Move(dashDirection * dashForce * Time.deltaTime);
            yield return null;
        }
        
        isDashing = false;
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
    }

    void ApplyMovement()
    {
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDirection = (camForward * currentInputVector.y) + (camRight * currentInputVector.x);

        bool isLockedOn = (enemyTarget != null && enemyTarget.currentTarget != null);
        if (isLockedOn)
        {
            Vector3 dirToEnemy = (enemyTarget.currentTarget.position - transform.position).normalized;
            dirToEnemy.y = 0;
            if (dirToEnemy != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (moveDirection.magnitude > 0.05f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        float targetSpeed = IsRunning ? runSpeed : moveSpeed;
        float inputMagnitude = Mathf.Clamp01(currentInputVector.magnitude); 
        
        Vector3 finalVelocity = moveDirection * (targetSpeed * inputMagnitude);
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);
    }

    void UpdateAnimatorParameters()
    {
        if (animator == null) return;
        float x = 0f; float z = 0f;
        bool isLockedOn = (enemyTarget != null && enemyTarget.currentTarget != null);

        if (isLockedOn) { x = currentInputVector.x; z = currentInputVector.y; }
        else { x = 0f; z = currentInputVector.magnitude; }

        if (IsRunning && z > 0.1f) z = 2f; 

        animator.SetFloat("PosX", x, 0.1f, Time.deltaTime);
        animator.SetFloat("PosY", z, 0.1f, Time.deltaTime);
    }
}