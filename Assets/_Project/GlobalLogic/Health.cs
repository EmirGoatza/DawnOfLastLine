using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent OnDeath;

    public float MaxHealth 
    { 
        get => maxHealth;
        set 
        {
            maxHealth = value;
            currentHealth = maxHealth;
        }
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} a maintenant {currentHealth} HP.");

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} meurt !");
            // On prévient tous ceux qui écoutent
            OnDeath?.Invoke();
        }
    }
}