using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
        // Destroy after the particle system's duration + max lifetime
        if (ps != null && !ps.main.loop)
        {
            Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }
}