using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

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
    public SplineContainer spline;
    private static List<Building> buildings = new List<Building>();


    private Health health;

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

        if (spline == null)
        {
            SplineContainer[] splines = FindObjectsOfType<SplineContainer>();

            float closestDistance = float.MaxValue;
            SplineContainer closestSpline = null;

            foreach (var s in splines)
            {
                float3 nearestPoint;
                float t;

                SplineUtility.GetNearestPoint(
                    s.Spline,
                    transform.position,
                    out nearestPoint,
                    out t
                );

                float distance = Vector3.Distance(transform.position, nearestPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSpline = s;
                }
            }

            spline = closestSpline;
        }

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath.AddListener(HandleDeath);
        }

    }

    private void OnEnable()
    {
        buildings.Add(this);
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
            // Debug.Log($"Countdown: {currentCountdown:F2}, Trigger: {trigger}");
            lastCountdown = currentCountdown;
        }

        if (actOnTimer)
        {
            if (currentCountdown <= 0f)
            {
                Execute();
            }
        }
        else
        {
            if (buildings == null || buildings.Count == 0) return;

            // Cherche le bâtiment le plus proche
            Building nearest = null;
            float minDistSqr = float.MaxValue;
            Vector3 playerPos = playerTransform.position;

            foreach (var b in buildings)
            {
                float dSqr = (b.transform.position - playerPos).sqrMagnitude;
                if (dSqr < minDistSqr)
                {
                    minDistSqr = dSqr;
                    nearest = b;
                }
            }

            bool keyboardPressed = Keyboard.current != null && Keyboard.current.xKey.isPressed;
            bool gamepadPressed = Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame;

            if (nearest == this && (keyboardPressed || gamepadPressed) && minDistSqr <= distance * distance)
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

    private void OnDisable()
    {
        buildings.Remove(this);
    }

    private void HandleDeath()
    {
        //TODO: On pourrait ajouter un bruit de destuction si on est proche du joueur ou autre feedback visuel dans l'ui
        Debug.Log($"Le bâtiment {gameObject.name} a été détruit !");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath.RemoveListener(HandleDeath);
        }
    }


    protected abstract void OnUpgrade();
    protected abstract void AttackorSpawn();
}
