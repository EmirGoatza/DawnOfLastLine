using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Damage Boost")]
public class DamageBoostEffect : AugmentEffect
{
    public int bonusDamage = 5;

    public override void Apply(GameObject player)
    {
        PlayerCombat combat = player.GetComponent<PlayerCombat>();

        if (combat != null)
        {
            combat.combatStats.baseDamage += bonusDamage;
            Debug.Log($"Dégâts augmentés ! Nouveau baseDamage : {combat.combatStats.baseDamage}");
        }
    }
    protected override object[] GetDescriptionValues()
    {
        return new object[] { bonusDamage };
    }
}
