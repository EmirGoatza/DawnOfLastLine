using UnityEngine;

public class BasicEnemy : Enemy
{
    [Header("Combat - Basique")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] protected int damage = 20;
    private float nextAttackTime;

    protected override float StoppingDistance => attackRange - 0.2f;

    protected override void Awake()
    {
        base.Awake(); 

        enemyName = "Soldat de base";
        base.health.MaxHealth = 50f;
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
        // On essaie de récupérer le script Health sur la cible actuelle
        Health targetHealth = currentTarget.GetComponent<Health>();

        if (targetHealth != null && !targetHealth.IsDead)
        {
            // On inflige les dégâts
            targetHealth.TakeDamage(damage);
            
            Debug.Log($"<color=red>COMBAT :</color> {enemyName} inflige {damage} dégâts à {currentTarget.name} !");
        }

        nextAttackTime = Time.time + attackCooldown;
    }
    }

    protected override void Die()
    {
        base.Die();
    }
}