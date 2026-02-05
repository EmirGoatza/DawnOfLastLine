using UnityEngine;
using UnityEngine.Events;

public class MainBuilding : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public UnityAction OnCoreDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnCoreDestroyed?.Invoke();
        }
    }
}