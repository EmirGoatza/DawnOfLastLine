using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharStam : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 3f;      
    public float staminaRegen = 0.5f;  
    
    [Header("UI Dynamic Generation")]
    public GameObject barPrefab;
    public Transform barContainer;

    private float currentStamina;
    private List<Image> barImages = new List<Image>();

    void Start()
    {
        currentStamina = maxStamina;
        RefreshUIBars();
    }

    void Update()
    {
        HandleRegen();
        UpdateUIFill();
    }

    public void RefreshUIBars()
    {
        foreach (Transform child in barContainer)
        {
            Destroy(child.gameObject);
        }
        barImages.Clear();

        for (int i = 0; i < maxStamina; i++)
        {
            GameObject newBar = Instantiate(barPrefab, barContainer);
            
            Image fillImage = newBar.transform.Find("Fill").GetComponent<Image>();
            
            barImages.Add(fillImage);
        }
    }

    private void UpdateUIFill()
    {
        for (int i = 0; i < barImages.Count; i++)
        {
            barImages[i].fillAmount = Mathf.Clamp01(currentStamina - i);
        }
    }

    private void HandleRegen()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }
    }

    public bool HasStamina(float cost) => currentStamina >= cost;

    public void UseStamina(float cost)
    {
        currentStamina -= cost;
        if (currentStamina < 0) currentStamina = 0;
    }

    public void AddMaxStamina(int amount)
    {
        maxStamina += amount;
        currentStamina += amount;
        RefreshUIBars();
    }
}