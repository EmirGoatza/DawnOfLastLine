using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float regenerationRate = 0f; // En points de vie par seconde
    private float regenerationTimer = 0f;

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

    public float Regeneration
    {
        get => regenerationRate;
        set => regenerationRate = value;
    }

    public bool IsDead { get; private set; } = false;

    void Awake()
    {
        currentHealth = maxHealth;

        if (GetComponent<HealthDamagePopup>() == null)
        {
            gameObject.AddComponent<HealthDamagePopup>();
        }
        if (GetComponent<HealthAddedPopup>() == null)
        {
            gameObject.AddComponent<HealthAddedPopup>();
        }
    }

    void Update()
    {
        ApplyRegeneration();
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

            // Debug.Log($"{gameObject.name} meurt !");
            // On prévient tous ceux qui écoutent
            OnDeath?.Invoke();
        }
    }

    public void ResetHealth()
    {
        IsDead = false;
        CurrentHealth = maxHealth;
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }


    public void ApplyRegeneration()
    {
        if(regenerationRate <= 0) return;

        regenerationTimer += Time.deltaTime;
        if (regenerationTimer >= 1f)
        {
            Heal(regenerationRate);
            regenerationTimer = 0f;
        }
    }

}