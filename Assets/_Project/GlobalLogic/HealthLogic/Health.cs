using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent OnDeath;
    public UnityEvent<float, float> OnHealthChanged;

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            currentHealth = maxHealth;
        }
    }
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = value;
            // On prévient automatiquement le slider
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} a maintenant {currentHealth} HP.");

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} meurt !");
            // On prévient tous ceux qui écoutent
            OnDeath?.Invoke();
        }
    }
}