using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0); // Petit décalage au-dessus de la tête
    
    private Health healthComponent;
    private Transform targetTransform;
    private Transform mainCameraTransform;

    void Awake()
    {
        healthComponent = GetComponentInParent<Health>();

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        
        if (healthComponent != null)
        {
            targetTransform = healthComponent.transform;
            healthComponent.OnHealthChanged.AddListener(UpdateSlider);
            
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
            RotateTowardsCamera();
        }
    }

    private void PositionAboveParent()
    {
        Collider col = targetTransform.GetComponent<Collider>();
        float yOffset = (col != null) ? col.bounds.size.y : 2f; 

        transform.position = targetTransform.position + Vector3.up * yOffset + offset;
    }

    private void RotateTowardsCamera()
    {
        if (mainCameraTransform != null)
        {
            transform.LookAt(transform.position + mainCameraTransform.forward);
        }
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