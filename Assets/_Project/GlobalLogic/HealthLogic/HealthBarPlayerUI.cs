using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Health playerHealth;

    void Start()
    {
        if (playerHealth != null)
        {
            // Initialisation
            UpdateSlider(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            
            // On s'abonne à l'événement
            playerHealth.OnHealthChanged.AddListener(UpdateSlider);
        }
    }

    private void UpdateSlider(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateSlider);
        }
    }
}