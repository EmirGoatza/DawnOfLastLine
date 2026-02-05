using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 9f; // Vitesse quand on court
    public float rotationSpeed = 100f;
    public float acceleration = 5f;
    public float gravity = -9.81f;

    public float CurrentV { get; private set; }
    public float CurrentH { get; private set; }
    public bool IsRunning { get; private set; } // Pour l'animator

    private CharacterController controller;
    private Transform cam;
    private float verticalVelocity;
    private float targetV;
    private float targetH;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (Camera.main != null) cam = Camera.main.transform;
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            // Détection de Shift
            IsRunning = Keyboard.current.leftShiftKey.isPressed;

            targetV = 0;
            if (Keyboard.current.wKey.isPressed) targetV = 1;
            if (Keyboard.current.sKey.isPressed) targetV = -1;

            targetH = 0;
            if (Keyboard.current.dKey.isPressed) targetH = 1;
            if (Keyboard.current.qKey.isPressed) targetH = -1;

            CurrentV = Mathf.MoveTowards(CurrentV, targetV, acceleration * Time.deltaTime);
            CurrentH = Mathf.MoveTowards(CurrentH, targetH, acceleration * Time.deltaTime);
        }

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * CurrentV) + (right * CurrentH);

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        // On choisit la vitesse selon IsRunning
        float speed = IsRunning ? runSpeed : moveSpeed;
        Vector3 finalMovement = moveDirection * speed;
        finalMovement.y = verticalVelocity;

        controller.Move(finalMovement * Time.deltaTime);
    }
}