using UnityEngine;

public class ArcherRig : MonoBehaviour
{
    [Header("References")]
    public Transform aimTransform;    // The transform that should point at target
    public Transform chestPivot;      // The pivot point (chest)

    [Header("Settings")]
    public float rotationSpeed = 5f;
    public Vector3 aimOffset = new Vector3(0, 1.5f, 0);

    [HideInInspector] public Transform targetEnemy;
    [HideInInspector] public bool isAiming = false;

    private Quaternion initialRelativeRotation;
    private bool hasStoredRelativeRotation = false;

    void LateUpdate()
    {
        if (!isAiming || targetEnemy == null || aimTransform == null || chestPivot == null)
        {
            hasStoredRelativeRotation = false; // Reset when not aiming
            return;
        }

        // Store the relative rotation on the FIRST frame of aiming (when animation is playing)
        if (!hasStoredRelativeRotation)
        {
            initialRelativeRotation = Quaternion.Inverse(aimTransform.rotation) * chestPivot.rotation;
            hasStoredRelativeRotation = true;
        }

        Vector3 targetPoint = targetEnemy.position + aimOffset;
        
        // Calculate direction from aim transform to target
        Vector3 directionToTarget = (targetPoint - aimTransform.position).normalized;
        
        // Target rotation for aim transform
        Quaternion targetAimRotation = Quaternion.LookRotation(directionToTarget);
        
        // Smooth rotation for aim transform
        aimTransform.rotation = Quaternion.Slerp(
            aimTransform.rotation, 
            targetAimRotation, 
            Time.deltaTime * rotationSpeed
        );

        // Now rotate chest to maintain the same relative rotation to the aim transform
        chestPivot.rotation = aimTransform.rotation * initialRelativeRotation;
    }
}