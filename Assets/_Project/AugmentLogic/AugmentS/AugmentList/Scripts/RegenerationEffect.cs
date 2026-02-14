using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/RegenerationEffect")]
public class RegenerationEffect : AugmentEffect
{
        public float regenerationGranted = 0.1f;
        
    public override void Apply(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.regenerationRate += regenerationGranted;
        }
    }
}
