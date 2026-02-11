using UnityEngine;
using UnityEngine.AI;

// ----------------------
// Generic Unit Controller
// ----------------------
public class UnitController : MonoBehaviour
{
    protected enum State { Idle, Wander, Chase, Attack }

    [Header("Stats")]
    public float wanderRange = 10f;
    public float detectionRange = 15f;
    public float attackRange = 2f;  // Melee by default
    public float attackCooldown = 1f;

    [Header("References")]
    public Animator animator;
    public string enemyTag = "Enemy";
    [HideInInspector] public Transform targetEnemy;
    protected Health health;
    
    protected NavMeshAgent agent;
    protected State currentState = State.Idle;

    [HideInInspector] public GameObject spawn;
    protected Vector3 wanderPoint;
    protected bool wanderPointSet = false;
    protected bool canAttack = true;

    
    protected virtual void Awake()
    {
        health = GetComponent<Health>();
    }
    
    protected virtual void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();

    }

    protected virtual void Update()
    {
        FindClosestEnemy();

        switch (currentState)
        {
            case State.Idle: Idle(); break;
            case State.Wander: Wander(); break;
            case State.Chase: Chase(); break;
            case State.Attack: Attack(); break;
        }

        UpdateAnimatorParameters();
    }

    protected void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDist = Mathf.Infinity;
        targetEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetEnemy = enemy.transform;
            }
        }

        if (targetEnemy != null)
        {
            float distToEnemy = Vector3.Distance(transform.position, targetEnemy.position);
            if (distToEnemy <= attackRange) currentState = State.Attack;
            else if (distToEnemy <= detectionRange) currentState = State.Chase;
            else if (currentState == State.Chase || currentState == State.Attack) currentState = State.Idle;
        }
        else if (currentState == State.Chase || currentState == State.Attack)
        {
            currentState = State.Idle;
        }
    }
    
    void OnEnable()
    {
        if (health != null) health.OnDeath.AddListener(Die);
    }

    void OnDisable()
    {
        if (health != null) health.OnDeath.RemoveListener(Die);
    }

    protected virtual void Idle()
    {
        if (!wanderPointSet)
            SetWanderPoint();
        currentState = State.Wander;
    }

    protected virtual void Wander()
    {
        if (!wanderPointSet)
            SetWanderPoint();

        agent.SetDestination(wanderPoint);

        if (Vector3.Distance(transform.position, wanderPoint) < 0.2f)
            wanderPointSet = false;
    }

    protected void SetWanderPoint()
    {
        float randomX = Random.Range(-wanderRange, wanderRange);
        float randomZ = Random.Range(-wanderRange, wanderRange);
        Vector3 randomPoint = spawn.transform.position + new Vector3(randomX, 0, randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            wanderPoint = hit.position;
            wanderPointSet = true;
        }
    }

    protected virtual void Chase()
    {
        if (targetEnemy == null)
        {
            currentState = State.Idle;
            return;
        }

        agent.SetDestination(targetEnemy.position);
    }

    protected virtual void Attack()
    {
        if (targetEnemy == null)
        {
            currentState = State.Idle;
            return;
        }

        transform.LookAt(new Vector3(targetEnemy.position.x, transform.position.y, targetEnemy.position.z));

        

    }

    protected void ResetAttack() => canAttack = true;

    protected virtual void Die()
    {
        SpawnBuilding sb = spawn.GetComponent<SpawnBuilding>();

        if (sb != null)
            sb.limit++;
        
        Destroy(gameObject);
    }
    
    protected void UpdateAnimatorParameters()
    {
        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        animator.SetFloat("PosX", localVelocity.x);
        animator.SetFloat("PosY", localVelocity.z);
    }
}
