using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Health))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnDelay = 3f;
    public Transform respawnPoint;

    private Health health;
    private CharMove charMove;
    private PlayerCombat combat;

    private Renderer[] renderers;
    private Collider[] colliders;

    private CharacterController characterController;

    [Header("Gestion Caméra")]
    private CameraManager cameraManager;

    [Header("UI de Mort")]
    [SerializeField] private GameObject deathUI;
    [SerializeField] private TMP_Text deathText;

    void Awake()
    {
        health = GetComponent<Health>();
        charMove = GetComponent<CharMove>();
        combat = GetComponent<PlayerCombat>();

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        // On exclut le CharacterController
        colliders = System.Array.FindAll(colliders, c => !(c is CharacterController));
    }

    void OnEnable()
    {
        health.OnDeath.AddListener(HandleDeath);
    }

    void OnDisable()
    {
        health.OnDeath.RemoveListener(HandleDeath);
    }

    void HandleDeath()
    {
        StartCoroutine(RespawnCoroutine());
    }

    void ShowDeathUI()
    {
        if (deathUI) deathUI.SetActive(true);
        StartCoroutine(UpdateRespawnTimer());
    }

    IEnumerator UpdateRespawnTimer()
    {
        float timeRemaining = respawnDelay;
        while (timeRemaining > 0)
        {
            if (deathText) deathText.text = $"Vous êtes mort...\nRespawn dans {timeRemaining:F1}s";
            yield return new WaitForSeconds(0.1f);
            timeRemaining -= 0.1f;
        }
    }

    void HideDeathUI()
    {
        if (deathUI) deathUI.SetActive(false);
    }

    IEnumerator RespawnCoroutine()
    {

        if (CameraManager.Instance != null) CameraManager.Instance.ShowDeathCamera();

        ShowDeathUI();

        // Désactive contrôles
        if (charMove != null) charMove.canMove = false;
        if (combat != null) combat.enabled = false;

        // Désactive visuel
        foreach (var r in renderers)
            r.enabled = false;

        // Désactive collisions
        foreach (var c in colliders)
            c.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        HideDeathUI();

        // Désactive CharacterController pour éviter les problèmes
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;

        // Téléportation
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        // Réactive CharacterController
        characterController.enabled = true;


        // Reset HP
        health.ResetHealth();

        // Réactive visuel
        foreach (var r in renderers)
            r.enabled = true;

        // Réactive collisions
        foreach (var c in colliders)
            c.enabled = true;

        if (CameraManager.Instance != null)
        {
            // On attend que la transition de retour soit finie pour rendre le contrôle
            yield return CameraManager.Instance.ShowMainCamera();
        }

        // Réactive contrôles
        if (charMove != null) charMove.canMove = true;
        if (combat != null) combat.enabled = true;

    }

}
