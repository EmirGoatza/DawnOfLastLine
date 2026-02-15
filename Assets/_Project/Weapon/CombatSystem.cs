using UnityEngine;
using UnityEngine.Serialization;

public class CombatSystem : MonoBehaviour
{
    [Header("Réglages VFX")]
    public GameObject slashPrefab;
    public Transform slashSpawnPoint;
    public GameObject hitboxLight;
    public GameObject hitboxHeavy;
    
    private PlayerCombat playerCombat;
    private int currentDamage;
    private bool heavy = false;


    public void setcurrentDamage(int damage)
    {
        currentDamage = damage;
    }

    public void setHeavy(string currentcombo)
    {
        if (currentcombo == "Heavy")
        {
            heavy = true;
        }
        else heavy = false;
    }
    
    void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    public void TriggerSlash()
    {
        if (slashPrefab != null && slashSpawnPoint != null)
        {
            Quaternion finalRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
            
            GameObject vfx = Instantiate(slashPrefab, slashSpawnPoint.position, finalRotation);

            if (playerCombat != null && heavy)
            {
                vfx.transform.localScale *= playerCombat.heavyScaleMultiplier;
                Heavy(currentDamage);
            }
            else Light(currentDamage);
            
            /*SlashHitbox hitbox = vfx.GetComponent<SlashHitbox>();
            if (hitbox != null)
            {
                hitbox.Setup(damage, playerCombat.combatStats, playerCombat.GetComponent<Health>());
            }
            else
            {
                Debug.LogWarning("Le prefab de Slash n'a pas le script 'SlashHitbox' !");
            }*/
            
            Destroy(vfx, 2f); 
        }
    }

    private void Heavy(int damage)
    {
        CharDamageHandler handler = hitboxHeavy.GetComponent<CharDamageHandler>();
        
        handler.ActivateHitbox(damage);
    }

    private void Light(int damage)
    {
        CharDamageHandler handler = hitboxLight.GetComponent<CharDamageHandler>();
        
        handler.ActivateHitbox(damage);
    }

}