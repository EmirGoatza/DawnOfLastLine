using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Pv Boost")]
public class PvBoostEffect : AugmentEffect
{
    public float bonusPv = 100f;

    public override void Apply(GameObject player)
    {
        Health h = player.GetComponent<Health>();
        if (h != null)
        {
            h.MaxHealth += bonusPv;
            h.CurrentHealth += bonusPv;
        }
    }
}
