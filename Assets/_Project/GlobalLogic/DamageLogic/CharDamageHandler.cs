using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharDamageHandler : DamageHandler
{
    
    private Health sourceHealth;
    private PlayerStats stats;
    private Collider myCollider;
    

    void Start()
    {
        PlayerCombat playerCombat = GetComponentInParent<PlayerCombat>();
        stats = playerCombat.combatStats;
        sourceHealth = GetComponentInParent<Health>();
    }
    

    public override void OnTriggerEnter(Collider other)
    {
        if (base.hitEnemies.Contains(other.gameObject))        
        {
            Debug.Log("Hit already" + other.gameObject.name);
            return;
        }


        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth != null)
        {
            // Vérifie le type de cible en utilisant le tag du GameObject
            foreach (TargetType t in targets)
            {
                if ((t == TargetType.Enemy && other.CompareTag("Enemy")) ||
                    (t == TargetType.Player && other.CompareTag("Player")) ||
                    (t == TargetType.Ally && other.CompareTag("Ally")))
                {
                    targetHealth.TakeDamage(damageAmount);
                    hitEnemies.Add(other.gameObject);

                    // omnivampirisme
                    if (stats != null && stats.omnivampirisme > 0 && sourceHealth != null)
                    {
                        int healAmount = Mathf.RoundToInt(damageAmount * (stats.omnivampirisme / 100f));
                        sourceHealth.Heal(healAmount);
                    }
                    break; // on sort de la boucle dès qu’on a trouvé un type valide
                }
            }
        }
    }

   
}

