using UnityEngine;

public class HealthDamagePopup : MonoBehaviour
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
            popupPrefab = Resources.Load<GameObject>("HealthDamagePopup");
        }

        health.OnHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnHealthChanged(float current, float max)
    {
        float damage = lastHealth - current;

        if (damage > 0)
        {
            SpawnPopup(damage);
        }

        lastHealth = current;
    }

    private void SpawnPopup(float damage)
    {
        Vector3 pos = transform.position + Vector3.up * 1.5f;
        // On randomise pour rendre ça un peu plus vivant
        pos += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        DamagePopup popup = Instantiate(popupPrefab, pos, Quaternion.identity)
            .GetComponent<DamagePopup>();

        popup.Setup(damage);
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnHealthChanged.RemoveListener(OnHealthChanged);
    }
}
