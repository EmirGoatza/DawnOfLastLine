using UnityEngine;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 20f;
    
    [Header("Lifetime")]
    public float lifetime = 10f; // Destroy after 10 seconds
    
    private bool hasHit = false;
    
    // Static list to track all arrows
    private static List<Arrow> allArrows = new List<Arrow>();
    private static int maxArrows = 100;
    
    void OnEnable()
    {
        allArrows.Add(this);
        
        // If we exceed max arrows, destroy the oldest one
        if(allArrows.Count > maxArrows)
        {
            Arrow oldest = allArrows[0];
            allArrows.RemoveAt(0);
            if(oldest != null)
                Destroy(oldest.gameObject);
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void OnDisable()
    {
        allArrows.Remove(this);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(hasHit) return;
        hasHit = true;
        
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            
            // Stop the arrow
            Rigidbody rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        
            transform.SetParent(collision.transform);
                
        }
        

    }
}