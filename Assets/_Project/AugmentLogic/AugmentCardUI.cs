using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentCardUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public AugmentData augmentData;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (augmentData != null)
        {
            titleText?.SetText(augmentData.augmentName);
            descriptionText?.SetText(augmentData.Description);
        }
    }

    public void SetSelected(bool selected)
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, selected ? 1f : 0.27f);
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, selected ? 1f : 0.27f);
        descriptionText.color = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, selected ? 1f : 0.27f);
    }
}
