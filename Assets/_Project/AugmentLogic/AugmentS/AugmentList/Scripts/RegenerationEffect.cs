using UnityEngine;

[CreateAssetMenu(menuName = "Augment/Effects/RegenerationEffect")]
public class RegenerationEffect : AugmentEffect
{
    public float regenerationGranted = 1f;

    public override void Apply(GameObject player)
    {
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Regeneration += regenerationGranted;
        }
    }

    protected override object[] GetDescriptionValues()
    {
        return new object[] { regenerationGranted };
    }

}