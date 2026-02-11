using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnDelay = 3f;
    public Transform respawnPoint;

    private Health health;
    private CharMove charMove;
    private PlayerCombat combat;

    void Awake()
    {
        health = GetComponent<Health>();
        charMove = GetComponent<CharMove>();
        combat = GetComponent<PlayerCombat>();
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

    IEnumerator RespawnCoroutine()
    {
        // Désactive les contrôles
        if (charMove != null) charMove.canMove = false;
        if (combat != null) combat.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Téléportation
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        // Reset HP
        health.MaxHealth = health.MaxHealth; // remet currentHealth à max

        // Réactive contrôles
        if (charMove != null) charMove.canMove = true;
        if (combat != null) combat.enabled = true;
    }
}
