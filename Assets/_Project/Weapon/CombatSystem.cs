using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Réglages VFX")]
    public GameObject slashPrefab;
    public Transform slashSpawnPoint;

    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    public void TriggerSlash(int damage)
    {
        if (slashPrefab != null && slashSpawnPoint != null)
        {
            Quaternion finalRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
            
            GameObject vfx = Instantiate(slashPrefab, slashSpawnPoint.position, finalRotation);

            if (playerCombat != null && playerCombat.ComboStep == 3)
            {
                vfx.transform.localScale *= playerCombat.heavyScaleMultiplier;
            }
            SlashHitbox hitbox = vfx.GetComponent<SlashHitbox>();
            if (hitbox != null)
            {
                hitbox.Setup(damage);
            }
            else
            {
                Debug.LogWarning("Le prefab de Slash n'a pas le script 'SlashHitbox' !");
            }
            
            Destroy(vfx, 2f); 
        }
    }
}