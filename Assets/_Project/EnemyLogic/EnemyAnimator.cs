using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    
    [Header("Réglages Animation")]
    public float referenceWalkSpeed = 5f; 
    public float smoothTime = 0.1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        CalculateMovementAndAnimate();
    }

    public void PlayAttack()
    {
        if(animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }
    void CalculateMovementAndAnimate()
    {
        Vector3 worldVelocity = (transform.position - lastPosition) / Time.deltaTime;
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

        float forwardAmount = localVelocity.z / referenceWalkSpeed;
        float strafeAmount = localVelocity.x / referenceWalkSpeed;

        animator.SetFloat("PosX", strafeAmount, smoothTime, Time.deltaTime);
        animator.SetFloat("PosY", forwardAmount, smoothTime, Time.deltaTime);

        lastPosition = transform.position;
    }
}