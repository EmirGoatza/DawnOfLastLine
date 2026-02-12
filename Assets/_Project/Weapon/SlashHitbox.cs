using UnityEngine;
using System.Collections; // Nécessaire pour IEnumerator
using System.Collections.Generic;

public class SlashHitbox : MonoBehaviour
{
    [Header("Durée de la hitbox")]
    [Tooltip("Temps en secondes avant que la zone de dégâts ne se désactive.")]
    public float activeDuration = 0.1f;

    private int damageAmount;
    private List<GameObject> hitEnemies = new List<GameObject>();
    private Collider myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    void Start()
    {
        StartCoroutine(DisableColliderAfterTime());
    }

    public void Setup(int damage)
    {
        damageAmount = damage;
    }

    IEnumerator DisableColliderAfterTime()
    {
        yield return new WaitForSeconds(activeDuration);
        
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hitEnemies.Contains(other.gameObject)) return;

        Health enemyHealth = other.GetComponentInParent<Health>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
            hitEnemies.Add(other.gameObject);
            Debug.Log($"[TOUCHÉ] {other.name} a pris {damageAmount} dégâts !");
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        
        if (box != null)
        {
            if (!box.enabled) 
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            }
            else 
            {
                Gizmos.color = new Color(1, 0, 0, 0.4f);
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = box.enabled ? Color.red : Color.gray;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}