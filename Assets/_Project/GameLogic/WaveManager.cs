using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;

public class WaveManager : MonoBehaviour
{
    public int currentWave = 0;

    [Header("Configuration")]
    [SerializeField] private WaveConfig waveConfig;

    [Header("Paramètres de Formation")]
    [SerializeField] private float laneWidth = 5f;
    [SerializeField] private float spacingZ = 3f;

    [Header("Ennemi References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SplineContainer splineContainer;

    private int enemiesSpawnedThisWave = 0;

    public void StartWave(int waveNumber)
    {
        enemiesSpawnedThisWave = 0;
        this.currentWave = waveNumber;
        
        // On récupère la liste générée dynamiquement
        List<WaveConfig.EnemyGroup> groups = waveConfig.GenerateWaveLayout(waveNumber);
        StartCoroutine(SpawnWaveRoutine(waveNumber, groups));
    }

    IEnumerator SpawnWaveRoutine(int waveNumber, List<WaveConfig.EnemyGroup> groups)
    {
        foreach (var group in groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(i, group.count, waveNumber);
                enemiesSpawnedThisWave++;
            }

            yield return new WaitForSeconds(group.delayAfter);
        }
    }

    void SpawnEnemy(int indexInGroup, int groupSize, int waveNumber)
    {
        if (enemyPrefab == null || spawnPoint == null) return;

        float xPos = 0;
        if (groupSize > 1)
        {
            float completion = (float)indexInGroup / (groupSize - 1);
            xPos = (completion - 0.5f) * laneWidth;
        }

        float zPos = indexInGroup * -spacingZ;
        Vector3 spawnPos = spawnPoint.position + new Vector3(xPos, 0, zPos);

        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // On remonte au parent pour le nom (si existant)
        string laneName = transform.parent != null ? transform.parent.name : "Lane";
        enemyInstance.name = $"Enemy_{laneName}_Wave{waveNumber}_{enemiesSpawnedThisWave}";

        Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.setContainerSpline(splineContainer);
            enemyScript.sideOffset = xPos;
            enemyScript.SetInitialDistanceOffset(indexInGroup * spacingZ);
        }
    }
}