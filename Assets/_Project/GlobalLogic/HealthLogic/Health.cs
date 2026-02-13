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
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    public bool IsDead { get; private set; } = false;

    void Awake()
    {
        currentHealth = maxHealth;

        if (GetComponent<HealthDamagePopup>() == null)
        {
            gameObject.AddComponent<HealthDamagePopup>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        // Debug.Log($"{gameObject.name} a maintenant {currentHealth} HP.");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            IsDead = true;

            Debug.Log($"{gameObject.name} meurt !");
            // On prévient tous ceux qui écoutent
            OnDeath?.Invoke();
        }
    }

    public void ResetHealth()
    {
        IsDead = false;
        CurrentHealth = maxHealth;
    }

}