using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Paramètres de Vitesse")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;

    [Header("Paramètres de Rotation")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Paramètres de Physique")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Réglages du Sol (NOUVEAU)")]
    public Transform groundCheck;      // L'objet vide qu'on a créé
    public float groundDistance = 0.4f; // Rayon de la sphère de détection
    public LayerMask groundMask;

    // Références
    private Animator animator;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // On crée une sphère invisible à la position du GroundCheck. 
        // Si elle touche le "groundMask", alors on est au sol.
        groundedPlayer = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; 
        }
        
        // Mise à jour de l'Animator
        animator.SetBool("IsGrounded", groundedPlayer);

        Vector2 input = GetMovementInput();
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
        bool isRunning = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        // Animation de marche
        float targetAnimSpeed = (direction.magnitude >= 0.1f) ? (isRunning ? 1.0f : 0.5f) : 0f;
        animator.SetFloat("Speed", targetAnimSpeed, 0.1f, Time.deltaTime);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && groundedPlayer)
        {
            // Formule physique : v = racine(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // Gravité
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            animator.SetTrigger("Attack");
        }
    }

    private Vector2 GetMovementInput()
    {
        if (Keyboard.current == null) return Vector2.zero;
        float x = 0; float y = 0;
        if (Keyboard.current.wKey.isPressed) y += 1;
        if (Keyboard.current.sKey.isPressed) y -= 1;
        if (Keyboard.current.aKey.isPressed) x -= 1;
        if (Keyboard.current.dKey.isPressed) x += 1;
        return new Vector2(x, y).normalized;
    }
}