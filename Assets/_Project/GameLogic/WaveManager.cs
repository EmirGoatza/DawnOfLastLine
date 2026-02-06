using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Splines;

public class WaveManager : MonoBehaviour {
    public enum GamePhase { Preparation, Attack }
    
    [Header("État de la Manche")]
    public GamePhase currentPhase = GamePhase.Preparation;
    public int currentWave = 0;

    [Header("Paramètres d'Attaque")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int enemiesPerWave = 5;
    private int enemiesSpawnedThisWave = 0;
    
    [Header("Paramètres de Formation")]
    [SerializeField] private float laneWidth = 2f; // Largeur du couloir de marche


    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI waveText;


    [Header("Ennemi References")]
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SplineContainer splineContainer;

    void Start() {
        if (currentPhase == GamePhase.Attack) {
            StartAttackPhase();
        }
        if (currentPhase == GamePhase.Preparation) {
            StartPreparationPhase();
        }
    }

    void Update() {

        waveText.text = "Manche : " + currentWave;
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

    // Cette fonction peut être appelée par un bouton UI "Lancer la vague"
    public void StartAttackPhase() {
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
            float randomSideOffset = Random.Range(-laneWidth / 2f, laneWidth / 2f);
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position + new Vector3(randomSideOffset, 0, 0), Quaternion.identity);
            enemyInstance.name = "Enemy_Wave" + currentWave + "_" + enemiesSpawnedThisWave;
            Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
            enemyScript.setContainerSpline(FindObjectOfType<SplineContainer>());
        }
    }

    public int GetEnemyCount() {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}