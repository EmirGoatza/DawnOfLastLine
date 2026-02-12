using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GamePhase { Preparation, Attack }

    [Header("Game State")]
    public GamePhase currentPhase = GamePhase.Preparation;

    [Header("Wave Management")]
    [SerializeField] private float preparationDuration = 10f;
    private float preparationTimer;

    [SerializeField] private List<WaveManager> lanes;

    [Header("Player / Building")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform buildingTransform;
    [SerializeField] private float activationDistance = 5f;

    [Header("Game Over")]
    [SerializeField] private MainBuilding mainBuilding;
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text gameOverText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartPreparationPhase();
    }

    void Update()
    {
        if (currentPhase == GamePhase.Preparation)
        {
            HandlePreparation();
        }
        else if (currentPhase == GamePhase.Attack)
        {
            CheckEndOfWave();
        }
    }

    void HandlePreparation()
    {
        preparationTimer -= Time.deltaTime;

        bool timerFinished = preparationTimer <= 0f;

        bool playerNear =
            Vector3.Distance(player.position, buildingTransform.position) <= activationDistance;

        bool inputPressed =
            Keyboard.current.pKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame);

        if (timerFinished || (playerNear && inputPressed))
        {
            StartAttackPhase();
        }
    }

    void StartPreparationPhase()
    {
        currentPhase = GamePhase.Preparation;
        preparationTimer = preparationDuration;

        Debug.Log("=== PHASE PREPARATION ===");
    }

    void StartAttackPhase()
    {
        currentPhase = GamePhase.Attack;

        foreach (var lane in lanes)
        {
            lane.StartWave();
        }

        Debug.Log("=== PHASE ATTACK ===");
    }

    void CheckEndOfWave()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            StartPreparationPhase();
        }
    }

    void OnEnable()
    {
        mainBuilding.OnCoreDestroyed += HandleGameOver;
    }

    void OnDisable()
    {
        mainBuilding.OnCoreDestroyed -= HandleGameOver;
    }

    void HandleGameOver()
    {
        if (gameOverUI) gameOverUI.SetActive(true);
        StartCoroutine(RestartGameRoutine());
    }

    IEnumerator RestartGameRoutine()
    {
        float remainingTime = restartDelay;

        while (remainingTime > 0f)
        {
            if (gameOverText)
            {
                gameOverText.text =
                    $"Game Over\nLa partie se relancera dans {remainingTime:F1} secondes";
            }

            remainingTime -= Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    void OnDrawGizmos()
    {
        if (buildingTransform == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(buildingTransform.position, activationDistance);
    }
}
