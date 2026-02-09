using UnityEngine;

public class ParticleStartAt : MonoBehaviour
{
    public float startTime = 0.15f;

    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Stop(); 
        ps.Simulate(startTime, true, true);
        ps.Play();
    }
}