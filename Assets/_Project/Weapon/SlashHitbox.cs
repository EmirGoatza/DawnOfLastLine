using UnityEngine;
using System.Collections.Generic;

public class SlashHitbox : MonoBehaviour
{
    private int damageAmount;
    private List<GameObject> hitEnemies = new List<GameObject>(); 

    public void Setup(int damage)
    {
        damageAmount = damage;
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
            Gizmos.color = new Color(1, 0, 0, 0.4f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}