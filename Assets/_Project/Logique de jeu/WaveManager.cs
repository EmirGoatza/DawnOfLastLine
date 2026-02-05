using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour {
    public enum GamePhase { Preparation, Attack }
    
    [Header("État de la Manche")]
    public GamePhase currentPhase = GamePhase.Preparation;
    public int currentWave = 0;

    [Header("Paramètres d'Attaque")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int enemiesPerWave = 5;
    private int enemiesSpawnedThisWave = 0;


    [Header("Test / Debug")]
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private Transform spawnPoint;

    void Start() {
        StartPreparationPhase();
    }

    void Update() {
        if (currentPhase == GamePhase.Attack) {
            if (enemiesSpawnedThisWave >= enemiesPerWave && GetEnemyCount() == 0) {
                StartPreparationPhase();
            }
        }
    }

    public void StartPreparationPhase() {
        currentPhase = GamePhase.Preparation;
        Debug.Log("// Phase de PRÉPARATION. Appuie sur une touche (ou bouton) pour lancer l'assaut.");
    }

    // // Cette fonction peut être appelée par un bouton UI "Lancer la vague"
    public void StartAttackPhase() {
        if (currentPhase == GamePhase.Attack) return;

        currentPhase = GamePhase.Attack;
        currentWave++;
        enemiesSpawnedThisWave = 0;
        
        Debug.Log("// Phase d'ATTAQUE ! Manche " + currentWave);
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine() {
        while (enemiesSpawnedThisWave < enemiesPerWave) {
            SpawnEnemy();
            enemiesSpawnedThisWave++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy() {
        if (enemyPrefab != null && spawnPoint != null) {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public int GetEnemyCount() {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}