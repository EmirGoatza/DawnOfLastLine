using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public int damageAmount = 20;
    public float attackCooldown = 0.5f;

    [Header("Hitbox")]
    public Vector3 hitboxSize = new Vector3(8f, 4f, 6f);
    public float hitboxDistance = 1.5f;
    public float impactTime = 0.2f;
    public LayerMask enemyLayers;

    private bool isAttacking = false;
    private bool showHitboxDebug = false;


    private CharMove charMove;
    private EnemyTarget targeting;

    void Start()
    {
        charMove = GetComponent<CharMove>();
        targeting = GetComponent<EnemyTarget>();
    }

    void Update()
    {
        if (isAttacking) return;

        if (ShouldAttack())
        {
            StartCoroutine(PerformAttack());
        }
    }

    bool ShouldAttack()
    {
        bool mouseClick = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool gamepadR1 = Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame;
        return mouseClick || gamepadR1;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // mouvement bloqué
        if (charMove != null) charMove.canMove = false;


        if (targeting != null && targeting.currentTarget != null)
        {
            Vector3 directionToEnemy = (targeting.currentTarget.position - transform.position).normalized;
            directionToEnemy.y = 0;
            if (directionToEnemy != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToEnemy);
            }
        }


        // animator ici animator.SetTrigger("Attack")
        yield return new WaitForSeconds(impactTime);

        showHitboxDebug = true;
        CheckHitbox();

        yield return new WaitForSeconds(attackCooldown);

        showHitboxDebug = false;
        isAttacking = false;
        
        if (charMove != null) charMove.canMove = true;
    }

    void CheckHitbox()
    {
        Vector3 hitboxCenter = transform.position + (transform.forward * hitboxDistance);
        Collider[] hitEnemies = Physics.OverlapBox(hitboxCenter, hitboxSize / 2, transform.rotation, enemyLayers);

        foreach (Collider enemyCollider in hitEnemies)
        {
            Health enemyHealth = enemyCollider.GetComponentInParent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 center = transform.position + transform.forward * hitboxDistance;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, hitboxSize);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // Ligne de range (du joueur jusqu'au centre)
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, center);

        // Point centre hitbox
        Gizmos.DrawSphere(center, 0.05f);
    }
}