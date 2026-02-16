using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageHandler : MonoBehaviour
{
    
    public enum TargetType
    {
        Player,
        Enemy,
        Ally
    }
    
    [Header("Durée de la hitbox")]
    [Tooltip("Temps en secondes avant que le collider ne se désactive.")]
    public float activeDuration = 0.05f;

    [HideInInspector] public int damageAmount;
    
    protected Collider myCollider;
    protected HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    [Header("Cibles adverses")]
    public List<TargetType> targets; 
    
    
    
    void Awake()
    {
        myCollider = GetComponent<Collider>();
        if (myCollider != null)
            myCollider.enabled = false; // collider désactivé par défaut
    }
    
    public void ActivateHitbox(int damage)
    {
        // Debug.Log($"[ACTIVATE] ActivateHitbox called. Clearing list. Previous count: {hitEnemies.Count}");
        damageAmount = damage;
        hitEnemies.Clear();
        // Debug.Log($"[ACTIVATE] List cleared. New count: {hitEnemies.Count}");

        if (myCollider != null)
        {
            myCollider.enabled = true;
            StartCoroutine(DisableColliderAfterTime());
        }
    }
    
    public void DeactivateHitbox()
    {
        hitEnemies.Clear();

        if (myCollider != null)
            myCollider.enabled = false;
    }

    private IEnumerator DisableColliderAfterTime()
    {
        yield return new WaitForSeconds(activeDuration);

        DeactivateHitbox();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        
        
        if (hitEnemies.Contains(other.gameObject))
            return;
        
        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth != null)
        {
            foreach (TargetType t in targets)
            {
                if ((t == TargetType.Enemy && other.CompareTag("Enemy")) ||
                    (t == TargetType.Player && other.CompareTag("Player")) ||
                    (t == TargetType.Ally && other.CompareTag("Ally")))
                {
                    targetHealth.TakeDamage(damageAmount);
                    hitEnemies.Add(other.gameObject);
                    
                    break; // on sort de la boucle dès qu’on a trouvé un type valide
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.color = myCollider != null && myCollider.enabled
                ? new Color(1, 0, 0, 0.4f)
                : new Color(0.5f, 0.5f, 0.5f, 0.2f);

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = myCollider != null && myCollider.enabled ? Color.red : Color.gray;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}
