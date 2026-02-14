using UnityEditor.Embree;
using UnityEngine;
using UnityEngine.Serialization;

// ----------------------
// Troop Controller (Melee)
// ----------------------
public class TroopLogic : UnitController
{
    [Header("Combo System")]
    private int maxComboCount = 3;
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    
    [Header("VFX")]
    public GameObject slashvfxPrefab;
    public GameObject firevfxPrefab;
    public GameObject shockvfxPrefab;
    public GameObject watervfxPrefab;

    public GameObject hitboxPrefab;
    
    [FormerlySerializedAs("slashsSpawnPoint")] public Transform slashSpawnPoint;


    [Header("Stats")] public int damage = 50;
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();

        // Only start a new attack if not currently attacking
        if (!isAttacking)
        {
            PerformComboAttack();
        }
    }

    private void PerformComboAttack()
    {
        // Check if we have a valid target in range
        if (targetEnemy == null || Vector3.Distance(transform.position, targetEnemy.position) > attackRange)
        {
            ResetCombo();
            return;
        }

        isAttacking = true;
        currentComboIndex++;

        // Trigger the appropriate attack animation
        switch (currentComboIndex)
        {
            case 1:
                animator.SetTrigger("A1");
                break;
            case 2:
                animator.SetTrigger("A2");
                break;
            case 3:
                animator.SetTrigger("A3");
                break;
        }

        // Reset combo if we've reached the max
        if (currentComboIndex >= maxComboCount)
        {
            currentComboIndex = 0;
        }
    }



    private void ResetCombo()
    {
        currentComboIndex = 0;
        isAttacking = false;
    }

    protected override void Die()
    {
        ResetCombo();
        base.Die();
    }

    // Override state changes to reset combo when leaving attack state
    protected override void Chase()
    {
        ResetCombo();
        base.Chase();
    }

    protected override void Idle()
    {
        ResetCombo();
        base.Idle();
    }

    protected override void Wander()
    {
        ResetCombo();
        base.Wander();
    }
    
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (targetEnemy == null) return;

        animator.SetLookAtWeight(1f);
        animator.SetLookAtPosition(targetEnemy.position);
    }
    
    
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;

        // Check if we still have ANY enemy in range
        if (targetEnemy == null || Vector3.Distance(transform.position, targetEnemy.position) > attackRange)
        {
            // No enemy in range, reset combo
            ResetCombo();
        }
        // Otherwise, ready for next combo attack immediately
    }

    public void OnAttackVFXCall()
    {
        Instantiate(slashvfxPrefab, slashSpawnPoint.position, slashSpawnPoint.rotation);
        
        if (slashvfxPrefab != null && slashSpawnPoint != null)
        {
        
            GameObject vfx = Instantiate(slashvfxPrefab, slashSpawnPoint.position, slashSpawnPoint.rotation);

            DamageHandler damageHandler = hitboxPrefab.GetComponent<DamageHandler>();
            damageHandler.ActivateHitbox(damage);
            
            /*SlashHitbox hitbox = vfx.GetComponent<SlashHitbox>();
            if (hitbox != null)
            {
                hitbox.Setup(damage, null, null);
            }
            else
            {
                Debug.LogWarning("Le prefab de Slash n'a pas le script 'SlashHitbox' !");
            }*/
        
            Destroy(vfx, 2f); 
        }
    }
}