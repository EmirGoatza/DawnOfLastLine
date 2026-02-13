using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Omnivamp Boost")]
public class OmnivampBoostEffect : AugmentEffect
{
    public int bonusOmnivamp = 1;

    public override void Apply(GameObject player)
    {
        PlayerCombat combat = player.GetComponent<PlayerCombat>();

        if (combat != null)
        {
            combat.combatStats.omnivampirisme += bonusOmnivamp;
            Debug.Log($"Omnivampirisme augmenté ! Nouvel omnivampirisme : {combat.combatStats.omnivampirisme}");
        }
    }
}
