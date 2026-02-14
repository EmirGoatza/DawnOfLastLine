using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStats combatStats = new PlayerStats();

    [Header("Combo Settings")]
    public float comboWindowDuration = 0.8f;

    [Header("Impact Timing")]
    public float lightImpactTime = 0.2f;
    public float heavyImpactTime = 0.5f;
    public float heavyRecoveryTime = 1.0f;

    [Header("Scaling")]
    [Tooltip("Grossissement du VFX pour l'attaque lourde")]
    public float heavyScaleMultiplier = 1.5f;

    private CombatSystem combatSystem;

    public int ComboStep { get { return comboStep; } }
    private int comboStep = 0;

    private bool isAttacking = false;
    private bool canChain = false;
    private bool queuedLight = false;
    private bool queuedHeavy = false;

    private CharMove charMove;
    private EnemyTarget targeting;
    private Animator anim;

    private const string ANIM_LIGHT1 = "AttackA";
    private const string ANIM_LIGHT2 = "AttackB";
    private const string ANIM_HEAVY = "AttackC";


    void Start()
    {
        anim = GetComponent<Animator>();
        charMove = GetComponent<CharMove>();
        targeting = GetComponent<EnemyTarget>();
        combatSystem = GetComponentInChildren<CombatSystem>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        bool inputLight = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                          || (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);

        bool inputHeavy = (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
                          || (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        if (!isAttacking)
        {
            if (inputLight) StartCoroutine(PerformCombo(1));
            else if (inputHeavy) StartCoroutine(PerformCombo(3));
        }
        else
        {
            if (inputLight) queuedLight = true;
            if (inputHeavy) queuedHeavy = true;
        }
    }

    IEnumerator PerformCombo(int step)
    {
        isAttacking = true;
        canChain = false;
        queuedLight = false;
        queuedHeavy = false;
        comboStep = step;

        if (charMove != null) charMove.canMove = false;

        FaceTarget();

        int currentDamage = combatStats.baseDamage;
        float currentImpactTime = lightImpactTime;

        if (step == 1)
        {
            anim.SetTrigger(ANIM_LIGHT1);
        }
        else if (step == 2)
        {
            anim.SetTrigger(ANIM_LIGHT2);
        }
        else if (step == 3)
        {
            anim.SetTrigger(ANIM_HEAVY);
            currentDamage = Mathf.RoundToInt(combatStats.baseDamage * combatStats.heavyMultiplier);
            currentImpactTime = heavyImpactTime;
        }

        yield return new WaitForSeconds(currentImpactTime);

        if (combatSystem != null)
        {
            combatSystem.TriggerSlash(currentDamage);
        }
        else
        {
            Debug.LogError("Pas de CombatSystem trouvé !");
        }

        if (step == 3)
        {
            yield return new WaitForSeconds(heavyRecoveryTime);
            ResetCombat();
            yield break;
        }

        canChain = true;

        float timer = 0;
        bool nextAttackFound = false;

        while (timer < comboWindowDuration)
        {
            if (comboStep == 1 && queuedLight)
            {
                nextAttackFound = true;
                StartCoroutine(PerformCombo(2));
                break;
            }
            else if (comboStep == 2 && queuedHeavy)
            {
                nextAttackFound = true;
                StartCoroutine(PerformCombo(3));
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (!nextAttackFound)
        {
            ResetCombat();
        }
    }

    void FaceTarget()
    {
        if (targeting != null && targeting.currentTarget != null)
        {
            Vector3 directionToEnemy = (targeting.currentTarget.position - transform.position).normalized;
            directionToEnemy.y = 0;
            if (directionToEnemy != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(directionToEnemy);
        }
    }

    void ResetCombat()
    {
        isAttacking = false;
        canChain = false;
        comboStep = 0;
        queuedLight = false;
        queuedHeavy = false;
        if (charMove != null) charMove.canMove = true;
    }
}