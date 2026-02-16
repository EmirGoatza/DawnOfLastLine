using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Splines;
using System.Collections.Generic;


public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyGroup
    {
        public int count;
        public float delayAfter;
    }

    public int currentWave = 0;

    [Header("Configuration")]
    [SerializeField] private WaveConfig waveConfig;

    [Header("Paramètres d'Attaque")]
    [SerializeField] private List<EnemyGroup> groupsInWave;
    private int enemiesSpawnedThisWave = 0;

    [Header("Paramètres de Formation")]
    [SerializeField] private float laneWidth = 5f;
    [SerializeField] private float spacingZ = 3f;


    [Header("Ennemi References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SplineContainer splineContainer;

    public void StartWave(int waveNumber)
    {
        enemiesSpawnedThisWave = 0;
        this.currentWave = waveNumber;
        StartCoroutine(SpawnWaveRoutine(waveNumber));
    }

    IEnumerator SpawnWaveRoutine(int waveNumber)
    {
        int extraEnemies = waveConfig.GetExtraEnemies(waveNumber);

        foreach (EnemyGroup group in groupsInWave)
        {
            // Récupération du nombre total d'ennemis à spawn pour ce groupe
            int finalCount = group.count + extraEnemies;

            for (int i = 0; i < finalCount; i++)
            {
                SpawnEnemy(i, finalCount, waveNumber);
                enemiesSpawnedThisWave++;
            }

            yield return new WaitForSeconds(waveConfig.GetScaledDelay(group.delayAfter, waveNumber));
        }
    }

    void SpawnEnemy(int indexInGroup, int groupSize, int waveNumber)
    {
        if (enemyPrefab == null || spawnPoint == null)
            return;

        float xPos = 0;

        if (groupSize > 1)
        {
            float completion = (float)indexInGroup / (groupSize - 1);
            xPos = (completion - 0.5f) * laneWidth;
        }

        float zPos = indexInGroup * -spacingZ;
        Vector3 spawnPos = spawnPoint.position + new Vector3(xPos, 0, zPos);

        GameObject enemyInstance =
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        string laneName = transform.parent.name;

        enemyInstance.name =
            $"Enemy_{laneName}_Wave{waveNumber}_{enemiesSpawnedThisWave}";

        Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
        enemyScript.setContainerSpline(splineContainer);
        enemyScript.sideOffset = xPos;
        enemyScript.SetInitialDistanceOffset(indexInGroup * spacingZ);
    }
}
