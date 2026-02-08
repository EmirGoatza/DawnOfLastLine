using UnityEngine;

public class SpawnBuilding : Building
{
    
    [Header("Ally Unit")]
    public GameObject unitPrefab;
    public Transform spawnPoint;

    protected override void AttackorSpawn()
    {
        if (unitPrefab != null && spawnPoint != null)
        {
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    protected override void OnUpgrade()
    {

    }

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.2f); // petit repère
            Gizmos.DrawLine(transform.position, spawnPoint.position); // ligne vers le bâtiment
        }
    }
}