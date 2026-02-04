using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 2f;
    public float height = 2f;
    public float sideOffset = 2f;
    public float sensitivity = 0.2f;

    private float mouseX;
    private float mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            mouseX += delta.x * sensitivity;
            mouseY -= delta.y * sensitivity;
            mouseY = Mathf.Clamp(mouseY, -20f, 60f);
        }

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

        Vector3 relativePosition = new Vector3(sideOffset, height, -distance);
        transform.position = target.position + (rotation * relativePosition);
        
        Vector3 lookTarget = target.position + (rotation * new Vector3(sideOffset, 1.5f, 0));
        
        transform.LookAt(lookTarget);
    }
}