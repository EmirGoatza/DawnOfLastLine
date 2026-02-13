using UnityEngine;

public class HealthAddedPopup : MonoBehaviour
{
    private static GameObject popupPrefab;

    private Health health;
    private float lastHealth;

    void Awake()
    {
        health = GetComponent<Health>();
        lastHealth = health.CurrentHealth;

        if (popupPrefab == null)
        {
            popupPrefab = Resources.Load<GameObject>("HealthAddedPopup");
        }

        health.OnHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnHealthChanged(float current, float max)
    {
        float heal = current - lastHealth;

        if (heal > 0)
        {
            SpawnPopup(heal);
        }

        lastHealth = current;
    }

    private void SpawnPopup(float heal)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("Le prefab 'HealthAddedPopup' est introuvable dans les dossiers Resources !");
            return;
        }
        Vector3 pos = transform.position + Vector3.up * 1.5f;
        // On randomise pour rendre ça un peu plus vivant
        pos += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        HealPopup popup = Instantiate(popupPrefab, pos, Quaternion.identity)
            .GetComponent<HealPopup>();

        popup.Setup(heal);
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnHealthChanged.RemoveListener(OnHealthChanged);
    }
}
