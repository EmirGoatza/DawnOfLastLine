using UnityEngine;

public class BasicEnemy : Enemy
{
    [Header("Combat - Basique")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float nextAttackTime;

    protected override void Awake()
    {
        base.Awake(); 

        enemyName = "Soldat de base";
        base.health.MaxHealth = 50f;
        damage = 10;
    }

    protected override void Update()
    {
        base.Update();

        // Logique d'attaque contre la cible
        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            
            if (distanceToTarget <= attackRange)
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

        if (targetHealth != null)
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