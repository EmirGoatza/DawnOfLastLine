using UnityEngine.InputSystem;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("Upgrade Models")]
    public GameObject level1model;
    public GameObject level2model;
    public GameObject level3model;

    private int currentLevel = 1;

    public int Level
    {
        get => currentLevel;
    }

    [Header("Values")]
    public float countdown;

    private float currentCountdown;

    private float upgradeCooldown;
    private bool trigger = false;
    
    private float lastCountdown = 0f;

    [Header("Building Behavior")]
    public Transform playerTransform;
    public bool actOnTimer;
    public float distance;

    public void Trigger()
    {
        if (currentCountdown <= 0f)
        {
            Execute();
            return;
        }

        if (currentCountdown <= 0.5f)
        {
            trigger = true;
        }
    }

    void Awake()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    void Start()
    {
        currentCountdown = 0.0f;
        upgradeCooldown = 0.0f;
        upgradeVisual();
    }

    void Update()
    {
        if (currentCountdown > 0f)
            currentCountdown = Mathf.Max(0f, currentCountdown - Time.deltaTime);
        
        if (upgradeCooldown > 0f)
            upgradeCooldown = Mathf.Max(0f, upgradeCooldown - Time.deltaTime);
        
        if (Mathf.Floor(currentCountdown) != Mathf.Floor(lastCountdown))
        {
            Debug.Log($"Countdown: {currentCountdown:F2}, Trigger: {trigger}");
            lastCountdown = currentCountdown;
        }

        if (actOnTimer)
        {
            if (currentCountdown <= 0f )
            {
                Execute();
            }
        }
        else
        {
            if (Keyboard.current.xKey.isPressed && Vector3.Distance(playerTransform.position, transform.position) <= distance)
            {
                Trigger();
            }
        
            if (currentCountdown <= 0f && trigger)
            {
                Execute();
            }
        }

    }
    
    void Execute()
    {
        AttackorSpawn();
        currentCountdown = countdown;
        trigger = false;
    }

    public void Upgrade()
    {
        if (currentLevel < 3)
        {
            currentLevel++;
        }

        OnUpgrade();
        upgradeVisual();
    }
    
    private void upgradeVisual()
    {
        if (level1model == null) Debug.LogError("level1model is NULL!");
        if (level2model == null) Debug.LogError("level2model is NULL!");
        if (level3model == null) Debug.LogError("level3model is NULL!");
    
        level1model.SetActive(currentLevel == 1);
        level2model.SetActive(currentLevel == 2);
        level3model.SetActive(currentLevel == 3);
    
        Debug.Log($"Level {currentLevel} - L1:{level1model.activeSelf}, L2:{level2model.activeSelf}, L3:{level3model.activeSelf}");
    }
    
    protected abstract void OnUpgrade();
    protected abstract void AttackorSpawn();
}
