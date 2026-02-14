using UnityEngine;

public enum Rarity
{
    Common = 50,
    Rare = 20,
    Epic = 5,
    Legendary = 1
}

[CreateAssetMenu(menuName = "Augment/New Augment")]
public class AugmentData : ScriptableObject
{
    public string augmentName;
    public AugmentEffect effect;
    public Rarity rarity;

    public string Description => effect != null ? effect.GetDescription() : "";
}
