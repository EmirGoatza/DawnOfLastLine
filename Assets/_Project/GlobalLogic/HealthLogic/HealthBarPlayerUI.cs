using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image fillImage;

    void Start()
    {
        if (playerHealth != null)
        {
            UpdateSlider(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            playerHealth.OnHealthChanged.AddListener(UpdateSlider);
        }
    }

    private void UpdateSlider(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;

        UpdateColor(current / max);
    }

    private void UpdateColor(float percent)
    {   
        Color color = Color.Lerp(Color.red, new Color(17f/255f, 164f/255f, 17f/255f), percent);
        fillImage.color = color;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateSlider);
        }
    }
}
