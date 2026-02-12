using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Pv Boost")]
public class PvBoostEffect : AugmentEffect
{
    public float bonusPv = 100f;

    public override void Apply(GameObject player)
    {
        player.GetComponent<Health>().MaxHealth += bonusPv;
        player.GetComponent<Health>().CurrentHealth += bonusPv;
        Debug.Log($"PV augmentés ! Nouveau maxHealth : {player.GetComponent<Health>().MaxHealth}");
    }
}
