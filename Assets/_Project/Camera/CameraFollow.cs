using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Cibles")]
    public Transform target;
    
    [Header("Paramètres")]
    public float distance = 6f;
    public float height = 2f;
    public float sideOffset = 0.5f; 
    public float damping = 5f;
    
    [Header("Contrôles")]
    public float mouseSensitivity = 0.2f;
    public float gamepadSensitivity = 150f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 60f;

    [Header("Effet de Sprint (FOV)")]
    public float baseFOV = 60f;
    public float runFOV = 75f;
    public float fovChangeSpeed = 5f;

    private float mouseX;
    private float mouseY;
    private Camera cam;
    
    private CharMove charMove;
    private EnemyTarget enemyTarget;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<Camera>();
        if (cam != null) cam.fieldOfView = baseFOV;

        if (target != null)
        {
            charMove = target.GetComponent<CharMove>();
            if (charMove == null) charMove = target.GetComponentInParent<CharMove>();
            
            enemyTarget = target.GetComponent<EnemyTarget>();
            if (enemyTarget == null) enemyTarget = target.GetComponentInParent<EnemyTarget>();
        }
        
        Vector3 angles = transform.eulerAngles;
        mouseX = angles.y;
        mouseY = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool isLockedOn = (enemyTarget != null && enemyTarget.currentTarget != null);

        if (isLockedOn)
        {
            HandleLockOnMovement();
        }
        else
        {
            HandleManualInput();
        }

        ApplyCameraPosition();
        HandleFOV();
    }

    void HandleManualInput()
    {
        // Souris
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX += mouseDelta.x * mouseSensitivity;
            mouseY -= mouseDelta.y * mouseSensitivity;
        }

        // Manette
        if (Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.rightStick.ReadValue();
            if (stickInput.magnitude > 0.1f)
            {
                mouseX += stickInput.x * gamepadSensitivity * Time.deltaTime;
                mouseY -= stickInput.y * gamepadSensitivity * Time.deltaTime;
            }
        }

        mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);
    }

    void HandleLockOnMovement()
    {
        Transform enemy = enemyTarget.currentTarget;

        if (enemy == null) return;

        Vector3 dirToEnemy = enemy.position - target.position;
        
        if (dirToEnemy != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dirToEnemy);
            Vector3 lookAngles = lookRotation.eulerAngles;

            mouseX = Mathf.LerpAngle(mouseX, lookAngles.y, damping * Time.deltaTime);
            
            mouseY = Mathf.LerpAngle(mouseY, 15f, damping * Time.deltaTime);
        }
    }

    void ApplyCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

        Vector3 relativePosition = new Vector3(sideOffset, height, -distance);
        transform.position = target.position + (rotation * relativePosition);
        
        Vector3 lookTarget = target.position + (rotation * new Vector3(sideOffset, 1.5f, 0));
        
        if (enemyTarget != null && enemyTarget.currentTarget != null)
        {
             Vector3 enemyPos = enemyTarget.currentTarget.position;
             lookTarget = Vector3.Lerp(lookTarget, enemyPos, 0.3f); 
        }

        transform.LookAt(lookTarget);
    }

    void HandleFOV()
    {
        if (cam == null || charMove == null) return;
        
        float targetFOV = charMove.IsRunning ? runFOV : baseFOV;
    
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }
}