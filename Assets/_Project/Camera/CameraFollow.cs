using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Cibles")]
    public GameObject player;
    
    [Header("Paramètres de Caméra")]
    public float distance = 6f;
    public float height = 2f;
    public float sideOffset = 1f;
    
    [Header("Contrôles")]
    public float mouseSensitivity = 0.2f;
    public float gamepadSensitivity = 150f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 60f;

    [Header("Effet de Sprint (FOV)")]
    public float baseFOV = 60f;
    public float runFOV = 75f;
    public float fovChangeSpeed = 5f;

    // Variables internes
    private float mouseX;
    private float mouseY;
    private Camera cam;
    private CharMove playerMover;
    private Transform target;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<Camera>();
        
        if (cam != null) cam.fieldOfView = baseFOV;

        if (player != null)
        {
            playerMover = player.GetComponent<CharMove>();
            target = player.transform;

            Debug.Log(playerMover);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleInput();
        HandleCameraMovement();
        HandleFOV();
    }

    void HandleInput()
    {
        // Souris
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX += mouseDelta.x * mouseSensitivity;
            mouseY -= mouseDelta.y * mouseSensitivity;
        }

        // Manette (Stick Droit)
        if (Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.rightStick.ReadValue();
            
            if (stickInput.magnitude > 0.1f)
            {
                mouseX += stickInput.x * gamepadSensitivity * Time.deltaTime;
                mouseY -= stickInput.y * gamepadSensitivity * Time.deltaTime;
            }
        }

        // limitation angle vertical 
        mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);
    }

    void HandleCameraMovement()
    {
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

        Vector3 relativePosition = new Vector3(sideOffset, height, -distance);
        transform.position = target.position + (rotation * relativePosition);
        
        Vector3 lookTarget = target.position + (rotation * new Vector3(sideOffset, 1.5f, 0));
        transform.LookAt(lookTarget);
    }

    void HandleFOV()
    {
        if (cam == null || playerMover == null) return;

        float targetFOV = playerMover.IsRunning ? runFOV : baseFOV;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }
}