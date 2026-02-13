using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color zeroHealthColor = Color.red;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private bool showHealthText = true;

    void Start()
    {
        if (playerHealth != null)
        {
            UpdateSlider(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            playerHealth.OnHealthChanged.AddListener(UpdateSlider);
        }
        if (healthText != null)
        {
            healthText.gameObject.SetActive(showHealthText);
        }
    }

    private void UpdateSlider(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;

        UpdateColor(current / max);
        UpdateHealthText();
    }

    private void UpdateColor(float percent)
    {
        Color color = Color.Lerp(zeroHealthColor, fullHealthColor, percent);
        fillImage.color = color;
    }
    private void UpdateHealthText()
    {
        if (showHealthText && healthText != null)
        {
            healthText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateSlider);
        }
    }
}
