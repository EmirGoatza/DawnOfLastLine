using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/Stamina Boost")]
public class StaminaBoostEffect : AugmentEffect
{
    public float bonusStamina = 1f;

    public override void Apply(GameObject player)
    {
        Debug.Log("Stamina Boost appliqué");
    }
    protected override object[] GetDescriptionValues()
    {
        return new object[] { bonusStamina };
    }
}
