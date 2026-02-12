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
        public int count;        // Combien d'ennemis dans ce groupe
        public float delayAfter; // Temps d'attente après ce groupe
    }

    public enum GamePhase { Preparation, Attack }

    [Header("État de la Manche")]
    public GamePhase currentPhase = GamePhase.Preparation;
    public int currentWave = 0;

    [Header("Paramètres d'Attaque")]
    [SerializeField] private List<EnemyGroup> groupsInWave;
    private int totalEnemiesThisWave = 0;
    private int enemiesSpawnedThisWave = 0;

    [Header("Paramètres de Formation")]
    [SerializeField] private float laneWidth = 5f; // Largeur du couloir de marche
    [SerializeField] private float spacingZ = 3f; // Distance entre les rangs


    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI waveText;


    [Header("Ennemi References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SplineContainer splineContainer;

    void Start()
    {
        if (currentPhase == GamePhase.Attack)
        {
            StartAttackPhase();
        }
        if (currentPhase == GamePhase.Preparation)
        {
            StartPreparationPhase();
        }
    }

    void Update()
    {

        waveText.text = "Manche : " + currentWave;
        if (currentPhase == GamePhase.Attack)
        {
            if (enemiesSpawnedThisWave >= totalEnemiesThisWave && GetEnemyCount() == 0)
            {
                StartPreparationPhase();
            }
        }
    }

    public void StartPreparationPhase()
    {
        currentPhase = GamePhase.Preparation;
        Debug.Log("// Phase de PRÉPARATION. Appuie sur une touche (ou bouton) pour lancer l'assaut.");
    }

    // Cette fonction peut être appelée par un bouton UI "Lancer la vague"
    public void StartAttackPhase()
    {
        currentWave++;
        enemiesSpawnedThisWave = 0;

        // On calcule le total d'ennemis prévus dans la liste pour savoir quand finir la vague
        totalEnemiesThisWave = 0;
        foreach (var group in groupsInWave) {
            totalEnemiesThisWave += group.count;
        }

        Debug.Log("// Phase d'ATTAQUE ! Manche " + currentWave);
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        // On parcourt chaque groupe défini à la main dans l'Inspecteur
        foreach (EnemyGroup group in groupsInWave)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(i, group.count);
                enemiesSpawnedThisWave++;
            }

            // On attend le temps spécifique défini pour ce groupe précis
            yield return new WaitForSeconds(group.delayAfter);
        }
    }

    void SpawnEnemy(int indexInGroup, int groupSize)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            // Calcul de la pos en x (Répartition régulière)  
            float xPos = 0;
            if (groupSize > 1) 
            {
                // Calcule un ratio entre -0.5 et 0.5 selon l'index
                float completion = (float)indexInGroup / (groupSize - 1);
                xPos = (completion - 0.5f) * laneWidth;
            }

            // Calcul de la position z (Pour les décaler un peu en profondeur)
            float zPos = indexInGroup * -spacingZ; 
            Vector3 spawnPos = spawnPoint.position + new Vector3(xPos, 0, zPos);

            GameObject enemyInstance = Instantiate(
            enemyPrefab,
            spawnPos,
            Quaternion.identity
            );


            string laneName = transform.parent.name;

            enemyInstance.name =
                $"Enemy_{laneName}_Wave{currentWave}_{enemiesSpawnedThisWave}";

            Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
            enemyScript.setContainerSpline(splineContainer);
            enemyScript.sideOffset = xPos;
            enemyScript.SetInitialDistanceOffset(indexInGroup * spacingZ);
        }
    }

    public int GetEnemyCount()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}