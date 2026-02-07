using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class EnemyTarget : MonoBehaviour
{
    [Header("Réglages")]
    public float detectionRadius = 30f;
    public LayerMask enemyLayer;
    public Transform currentTarget; 
    
    [Header("Switch Target")]
    public float switchThreshold = 0.5f;
    public float switchCooldown = 0.3f;

    [Header("UI")]
    public GameObject lockOnIcon; 

    private float lastSwitchTime;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (lockOnIcon != null) lockOnIcon.SetActive(false);
    }

    void Update()
    {
        bool toggleLock = false;
        
        // R3 (Manette) ou Molette (Souris)
        if (Gamepad.current != null && Gamepad.current.rightStickButton.wasPressedThisFrame) toggleLock = true;
        if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame) toggleLock = true;

        if (toggleLock)
        {
            if (currentTarget != null) Unlock();
            else TryLock();
        }

        if (currentTarget != null && (!currentTarget.gameObject.activeInHierarchy))
        {
            Unlock();
        }
        
        if (currentTarget != null)
        {
            HandleTargetSwitch();
        }

        UpdateUI();
    }

    void HandleTargetSwitch()
    {
        if (Time.time < lastSwitchTime + switchCooldown) return;

        float stickX = 0f;
        
        if (Gamepad.current != null)
        {
            stickX = Gamepad.current.rightStick.ReadValue().x;
        }
        // flèches directionnelles clavier
        if (Keyboard.current != null)
        {
             if (Keyboard.current.leftArrowKey.isPressed) stickX = -1;
             if (Keyboard.current.rightArrowKey.isPressed) stickX = 1;
        }

        // Si on pousse fort à droite ou à gauche
        if (Mathf.Abs(stickX) > switchThreshold)
        {
            SwitchTarget(stickX > 0);
            lastSwitchTime = Time.time;
        }
    }

    void SwitchTarget(bool searchRight)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        
        Transform bestNewTarget = null;
        float closestScreenDistance = Mathf.Infinity;

        Vector3 currentScreenPos = mainCam.WorldToViewportPoint(currentTarget.position);

        foreach (Collider enemy in enemies)
        {
            Transform candidate = enemy.transform;

            if (candidate == currentTarget) continue;
            if (candidate == transform) continue;

            Vector3 candidateScreenPos = mainCam.WorldToViewportPoint(candidate.position);

            bool isOnRight = candidateScreenPos.x > currentScreenPos.x;
            bool isOnLeft = candidateScreenPos.x < currentScreenPos.x;

            if ((searchRight && isOnRight) || (!searchRight && isOnLeft))
            {

                float dist = Vector2.Distance(currentScreenPos, candidateScreenPos);

                if (dist < closestScreenDistance)
                {
                    closestScreenDistance = dist;
                    bestNewTarget = candidate;
                }
            }
        }

        if (bestNewTarget != null)
        {
            currentTarget = bestNewTarget;
        }
    }

    void TryLock()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy;
        }
    }

    public void Unlock()
    {
        currentTarget = null;
    }

    void UpdateUI()
    {
        if (lockOnIcon != null)
        {
            lockOnIcon.SetActive(currentTarget != null);
            if (currentTarget != null)
            {
                lockOnIcon.transform.position = currentTarget.position + Vector3.up * 1.5f;
                lockOnIcon.transform.LookAt(mainCam.transform);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}