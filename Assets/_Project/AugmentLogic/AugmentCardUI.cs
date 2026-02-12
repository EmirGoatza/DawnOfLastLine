using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentCardUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon; 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    public AugmentData augmentData;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (augmentData != null)
        {
            if (titleText != null) titleText.text = augmentData.augmentName;
            if (descriptionText != null) descriptionText.text = augmentData.description;
            if (icon != null) icon.sprite = augmentData.icon; 
        }
    }

    public void SetSelected(bool selected)
    {
        Color color = selected ? selectedColor : normalColor;
        color.a = selected ? 0.5f : 0f;
        background.color = color;
        transform.localScale = selected ? Vector3.one * 1.1f : Vector3.one;
    }
}
