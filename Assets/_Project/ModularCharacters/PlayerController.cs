using UnityEngine;
using UnityEngine.InputSystem; // INDISPENSABLE pour le nouveau système

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            animator.SetTrigger("Attack");
        }
    }
}