using System.Collections;
using UnityEngine;

public class BasicEnemy : Enemy
{
    [Header("Combat - Basique")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackStopDuration = 1.5f;
    [SerializeField] protected int damage = 20;
    private float nextAttackTime;
    private EnemyAnimator enemyAnimator;

    protected override float StoppingDistance => attackRange - 0.2f;

    protected override void Awake()
    {
        base.Awake(); 

        enemyName = "Soldat de base";
        base.health.MaxHealth = 50f;

        enemyAnimator = GetComponent<EnemyAnimator>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentTarget != null)
        {
            Vector3 targetPoint = currentTarget.position;
            Collider targetCollider = currentTarget.GetComponent<Collider>();

            if (targetCollider != null)
            {
                targetPoint = targetCollider.ClosestPoint(transform.position);
            }

            float distanceToTargetSurface = Vector3.Distance(transform.position, targetPoint);

            if (distanceToTargetSurface <= attackRange)
            {
                TryAttack();
            }
        }
    }

    private void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.PlayAttack();
            }

            StartCoroutine(PerformAttackRoutine());

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator PerformAttackRoutine()
    {
        base.SetAttackingState(true);

        yield return new WaitForSeconds(0.2f); 

        Health targetHealth = currentTarget != null ? currentTarget.GetComponent<Health>() : null;
        if (targetHealth != null && !targetHealth.IsDead)
        {
            targetHealth.TakeDamage(damage);
            Debug.Log($"<color=red>COMBAT :</color> {enemyName} inflige {damage} dégâts !");
        }

        yield return new WaitForSeconds(attackStopDuration - 0.2f);

        base.SetAttackingState(false);
    }

    protected override void Die()
    {
        base.Die();
    }
}