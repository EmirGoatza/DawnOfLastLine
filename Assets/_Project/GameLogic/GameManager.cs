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

    [Header("Preparation UI")]
    [SerializeField] private TMP_Text preparationText;
    [SerializeField] private float blinkSpeed = 0.5f;

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

        bool playerNear =
            Vector3.Distance(player.position, buildingTransform.position) <= activationDistance;

        /// Texte de préparation
        if (preparationText)
        {
            string baseText =
                "Préparez-vous contre la horde...\nDébut dans " +
                Mathf.CeilToInt(preparationTimer) + " secondes";

            if (playerNear)
            {
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
                int alphaInt = Mathf.RoundToInt(alpha * 255);
                string alphaHex = alphaInt.ToString("X2");

                baseText += $"\n\n<color=#FFFF00{alphaHex}>Appuyez sur P ou Triangle pour appeler la horde en avance</color>";

            }

            preparationText.text = baseText;
        }

        /// Logique de début de la phase d'attaque
        bool timerFinished = preparationTimer <= 0f;


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

        if (preparationText)
        {
            preparationText.gameObject.SetActive(true);
        }
    }

    void StartAttackPhase()
    {
        currentPhase = GamePhase.Attack;

        if (preparationText) { preparationText.gameObject.SetActive(false); }

        foreach (var lane in lanes)
        {
            lane.StartWave();
        }
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
