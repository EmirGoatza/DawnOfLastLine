using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/RegenerationEffect")]
public class RegenerationEffect : AugmentEffect
{
        public float regenerationGranted = 1f;
        
    public override void Apply(GameObject player)    {
        Debug.Log("Regeneration Effect appliqué");
    }
}
