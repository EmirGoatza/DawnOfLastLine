using UnityEngine;

// ----------------------
// Archer Controller
// ----------------------
public class ArcherLogic : UnitController
{
    [Header("Arrow")]
    public GameObject arrowToShoot;

    [Header("Attack Settings")]
    public float maxAttackDistance = 10f;

    [Header("Pivot Transform")] 
    public Transform pivot;
    public Vector3 aimOffset = new Vector3(0, 1.5f, 0);

    [HideInInspector] 
    public Quaternion initialPivotRotation;
    private Quaternion currentLerpedRotation;
    private Quaternion desiredPivotRotation;
    private bool isAiming = false;
    private bool hasInitializedLerp = false;
    
    
    protected override void Start()
    {
        base.Start();
        
        if (pivot != null)
        {
            initialPivotRotation = pivot.localRotation;
        }
    }
    


    void Update()
    {
        base.Update();
        
        if (currentState != State.Attack)
        {
            isAiming = false;
        }
        
        
        if (pivot != null && targetEnemy != null && isAiming)
        {
            if (!hasInitializedLerp)
            {
                currentLerpedRotation = pivot.rotation;
                hasInitializedLerp = true;
            }
    
            Vector3 targetPoint = targetEnemy.position + aimOffset;
            Vector3 direction = (targetPoint - pivot.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            desiredPivotRotation = lookRotation * Quaternion.Euler(0, 80, 10);
            currentLerpedRotation = Quaternion.Slerp(currentLerpedRotation, desiredPivotRotation, Time.deltaTime * 5f);
        }
        else
        {
            hasInitializedLerp = false;
            if (pivot != null)
            {
                pivot.localRotation = initialPivotRotation;
            }
        }
    }

    void LateUpdate()
    {
        if (isAiming && pivot != null)
        {
            pivot.rotation = currentLerpedRotation;
        }
    }

    protected override void Chase()
    {
        if (targetEnemy == null)
        {
            currentState = State.Idle;
            return;
        }

        float distToEnemy = Vector3.Distance(transform.position, targetEnemy.position);
        Vector3 dir = (targetEnemy.position - transform.position).normalized;

        Vector3 targetPos = targetEnemy.position - dir * (maxAttackDistance - 0.5f);
        agent.SetDestination(targetPos);
    }


    protected override void Attack()
    {
        base.Attack();
    
        if(canAttack)
        {
            animator.SetTrigger("Shoot");
            if (arrowToShoot != null)
                arrowToShoot.SetActive(true);

            isAiming = true; // Set the flag
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }
}