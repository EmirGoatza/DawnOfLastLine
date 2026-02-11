using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Réglages VFX")]
    public GameObject slashPrefab;
    public Transform slashSpawnPoint;

    public void TriggerSlash()
    {
        if (slashPrefab != null && slashSpawnPoint != null)
        {
            Quaternion finalRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
            GameObject vfx =  Instantiate(slashPrefab, slashSpawnPoint.position, finalRotation);
            
            Destroy(vfx, 2f); 
        }
    }
}