using UnityEngine;

public abstract class AugmentEffect : ScriptableObject
{
    public abstract void Apply(GameObject player);
}