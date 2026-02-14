using UnityEngine;

public class SpawnBuilding : Building
{
    
    [Header("Ally Unit")]
    public GameObject unitPrefab;
    public Transform spawnPoint;
    public int limit;
    
    protected override void AttackorSpawn()
    {
        if (unitPrefab != null && spawnPoint != null )
        {
            if (limit > 0)
            {
                GameObject unit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);

                UnitController controller = unit.GetComponent<UnitController>();
                if (controller != null)
                    controller.spawn = this.gameObject;

                limit--;
            }
            else
            {
                {
                    // Debug.Log("Trop d'unités sur site !");
                }
            }
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