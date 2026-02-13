using UnityEngine;
using UnityEngine.InputSystem;

public class HealBuilding : MonoBehaviour
{
    [Header("Heal Settings")]
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private int healCost = 20;
    [SerializeField] private float detectionRange = 5f;

    private Health buildingHealth;
    private Transform playerTransform;
    private PlayerMoney playerMoney;

    void Start()
    {
        buildingHealth = GetComponent<Health>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerMoney = player.GetComponent<PlayerMoney>();
        }
    }

    void Update()
    {

        bool pPressed = Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
        bool trianglePressed = Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame;


        if (pPressed || trianglePressed)
        {
            TryHeal();
        }
    }

    private void TryHeal()
    {
        if (buildingHealth == null || playerMoney == null || playerTransform == null) { return; }

        bool inRange = ToolsSceneRange.IsWithinRange(transform, playerTransform, detectionRange);
        if (!inRange)
        {
            return;
        }

        // On vérifie si le bâtiment a besoin de soins et que l'on est à proximité
        if (buildingHealth.CurrentHealth < buildingHealth.MaxHealth)
        {
            // On tente de dépenser l'argent
            if (playerMoney.Spend(healCost))
            {
                // Soigne sans dépasser le max
                float newHealth = Mathf.Min(buildingHealth.CurrentHealth + healAmount, buildingHealth.MaxHealth);
                buildingHealth.CurrentHealth = newHealth;

                Debug.Log($"{gameObject.name} soigné ! Santé actuelle : {newHealth}");
            }
            else
            {
                Debug.Log("Pas assez d'argent pour soigner !");
            }
        }
        else
        {
            Debug.Log("Le bâtiment est déjà en pleine forme.");
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}