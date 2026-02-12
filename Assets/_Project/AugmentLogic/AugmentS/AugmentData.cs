using UnityEngine;

[CreateAssetMenu(menuName = "Augment/New Augment")]
public class AugmentData : ScriptableObject
{
    public string augmentName;
    public string description;

    public AugmentEffect effect;
}
