using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private SplineContainer targetSpline;

    [Header("Paramètres de Vague")]
    [SerializeField] private int totalEnemies = 10;
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Paramètres de Formation")]
    [SerializeField] private float laneWidth = 2f; // Largeur du couloir de marche

    private void Start()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefab == null || targetSpline == null) return;

        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Récupération du script
        FollowerEnemy follower = newEnemy.GetComponent<FollowerEnemy>();

        if (follower != null)
        {
            // Calcul d'un décalage latéral aléatoire pour éviter que les ennemis se chevauchent
            float randomSideOffset = Random.Range(-laneWidth / 2f, laneWidth / 2f);

            // Injection des données (Spline + Décalage)
            follower.SetupEnemy(targetSpline, randomSideOffset);
        }
    }
}