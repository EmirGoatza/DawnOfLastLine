using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0); // Petit décalage au-dessus de la tête
    
    private Health healthComponent;
    private Transform targetTransform;

    void Awake()
    {
        // On récupère le script Health sur le parent
        healthComponent = GetComponentInParent<Health>();
        
        if (healthComponent != null)
        {
            targetTransform = healthComponent.transform;
            healthComponent.OnHealthChanged.AddListener(UpdateSlider);
            
            // On positionne la barre dès le départ
            PositionAboveParent();
        }
    }

    void Start()
    {
        if (healthComponent != null)
        {
            UpdateSlider(healthComponent.CurrentHealth, healthComponent.MaxHealth);
        }
    }

    // On utilise LateUpdate pour que la barre suive le perso après ses mouvements
    void LateUpdate()
    {
        if (targetTransform != null)
        {
            PositionAboveParent();
        }
    }

    private void PositionAboveParent()
    {
        // On essaie de trouver le collider pour connaître la hauteur réelle de l'objet
        Collider col = targetTransform.GetComponent<Collider>();
        float yOffset = (col != null) ? col.bounds.size.y : 2f; 

        // On place le Canvas du slider au sommet + l'offset choisi
        transform.position = targetTransform.position + Vector3.up * yOffset + offset;
    }

    private void UpdateSlider(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }

    void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged.RemoveListener(UpdateSlider);
        }
    }
}