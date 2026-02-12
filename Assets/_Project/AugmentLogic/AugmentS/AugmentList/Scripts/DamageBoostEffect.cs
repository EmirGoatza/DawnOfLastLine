using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Damage Boost")]
public class DamageBoostEffect : AugmentEffect
{
    public float bonusDamage = 5f;

    public override void Apply()
    {
        Debug.Log("Damage Boost appliqué");
    }
}
