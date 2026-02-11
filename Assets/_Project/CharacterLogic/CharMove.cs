using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharMove : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSpeed = 10f;
    public float acceleration = 10f;
    public float gravity = -9.81f;

    [Header("State")]
    public bool IsRunning { get; private set; }

    [HideInInspector] public bool canMove = true;



    private CharacterController controller;
    private Transform cam;
    private float verticalVelocity;
    
    // lisser le mouvement
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity; 
    private float smoothInputSpeed = 0.1f;

    private Health health;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();

        if (Camera.main != null) cam = Camera.main.transform;
        
        // Cacher le curseur
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    void Update()
    {
        if(health.IsDead)
        {
            Debug.Log("Le joueur est mort, mouvement désactivé.");
            return;
        }
        
        ApplyGravity();


        if (!canMove || (health != null && health.IsDead))
        {
            controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
            return; 
        }

        HandleInput();
        ApplyMovement();
    }

    void HandleInput()
    {
        Vector2 targetInput = Vector2.zero;

        // Clavier
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) targetInput.y = 1;
            if (Keyboard.current.sKey.isPressed) targetInput.y = -1;
            if (Keyboard.current.dKey.isPressed) targetInput.x = 1;
            if (Keyboard.current.aKey.isPressed) targetInput.x = -1;
            
            IsRunning = Keyboard.current.leftShiftKey.isPressed;
        }

        // Manette (Prioritaire si utilisée)
        if (Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            // On prend l'input manette s'il est significatif
            if (stickInput.magnitude > 0.1f) 
            {
                targetInput = stickInput;
            }

            // Bouton Est (B sur Xbox)
            if (Gamepad.current.buttonEast.isPressed)
            {
                IsRunning = true;
            }
        }

        currentInputVector = Vector2.SmoothDamp(currentInputVector, targetInput, ref smoothInputVelocity, smoothInputSpeed);
    }


    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;
    }


    void ApplyMovement()
    {
        // vecteurs caméra
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * currentInputVector.y) + (camRight * currentInputVector.x);

        if (moveDirection.magnitude > 0.05f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float targetSpeed = IsRunning ? runSpeed : moveSpeed;
        
        float inputMagnitude = Mathf.Clamp01(currentInputVector.magnitude); 
        
        Vector3 finalVelocity = moveDirection * (targetSpeed * inputMagnitude);
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);
    }
}