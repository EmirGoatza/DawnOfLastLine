using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStats combatStats = new PlayerStats();
    
    [Header("Scaling")]
    [Tooltip("Grossissement du VFX pour l'attaque lourde")]
    public float heavyScaleMultiplier = 1.5f;

    private CombatSystem combatSystem;
    

    private bool isAttacking = false;


    private CharMove charMove;
    private EnemyTarget targeting;
    private Animator anim;

    private const string light = "Light";
    private const string heavy = "Heavy";


    private float regenerationTimer = 0f;
    
    
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
        ApplyRegeneration();
    }

    void HandleInput()
    {
        if (!isAttacking)
        {
            bool inputLight = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                              || (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);

            bool inputHeavy = (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
                              || (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            bool isInLightLockedCombo = currentState.IsTag("LightLocked");
            bool isInHeavyLockedCombo = currentState.IsTag("HeavyLocked");
            
            if (inputLight && !isInLightLockedCombo)
            {
                    PerformCombo(light);
            }
            else if (inputHeavy && !isInHeavyLockedCombo)
            {
                    PerformCombo(heavy);
            }
        }
    }

    void PerformCombo(string currentCombo)
    {
        isAttacking = true;
        if (charMove != null) charMove.canMove = false;

        
        FaceTarget();

        int currentDamage = combatStats.baseDamage;

        if (currentCombo == heavy)
        {
            currentDamage = Mathf.RoundToInt(combatStats.baseDamage * combatStats.heavyMultiplier);
        }
        
        
        if (combatSystem != null)
        {
            combatSystem.setcurrentDamage(currentDamage);
            combatSystem.setHeavy(currentCombo);
            //OnAttackVFXCall();
        }
        else
        {
            Debug.LogError("Pas de CombatSystem trouvé !");
        }
        
        anim.SetTrigger(currentCombo);

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
    
    void ApplyRegeneration()
    {
        if (combatStats.regenerationRate > 0)
        {
            Health playerHealth = GetComponent<Health>();
            if (playerHealth != null)
            {
                regenerationTimer += Time.deltaTime;
                if (regenerationTimer >= 1f)
                {
                    playerHealth.Heal(combatStats.regenerationRate);
                    regenerationTimer = 0f;
                }
            }
        }
    }
    
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }
    
    public void OnAttackVFXCall()
    {
        if(combatSystem != null) combatSystem.TriggerSlash();
    }

    public void OnAnimationEnd()
    {
        if (charMove != null && !isAttacking) charMove.canMove = true;
    }
    
}
