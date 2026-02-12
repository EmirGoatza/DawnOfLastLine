using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Mana Boost")]
public class ManaBoostEffect : AugmentEffect
{
    public float ManaBoost = 20f;

    public override void Apply(GameObject player)
    {
        Debug.Log("Mana Boost appliqué");
    }
}
