using UnityEngine;
using UnityEngine.Events;

public class MainBuilding : MonoBehaviour
{
    private Health health;

    public UnityAction OnCoreDestroyed;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        // On écoute à l'événement de mort
        if (health != null) health.OnDeath.AddListener(HandleCoreDestruction);
    }

    void OnDisable()
    {
        // Toujours pour éviter les fuites de mémoire
        if (health != null) health.OnDeath.RemoveListener(HandleCoreDestruction);
    }

    private void HandleCoreDestruction()
    {
        Debug.Log("Le bâtiment principal est tombé !");
        OnCoreDestroyed?.Invoke();
    }
}