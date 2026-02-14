using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine.Events;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyState { FollowingSpline, ChasingTarget, ReturningToSpline, Attacking }

    [Header("Composants & Références Internes")]
    protected NavMeshAgent agent;
    protected NavMeshObstacle obstacle;
    protected Health health;
    private EnemyAnimator enemyAnimator; // Si présent dans les enfants

    [Header("Statistiques de Base")]
    [SerializeField] protected string enemyName;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float rotationSpeed = 10f;
    [SerializeField] protected float moneyDropped = 10f;

    [Header("Machine d'État")]
    [SerializeField] private EnemyState currentState = EnemyState.FollowingSpline;

    [Header("Cibles & Détection")]
    [SerializeField] private float detectionRange = 5f;
    protected virtual float StoppingDistance => 0f;
    [SerializeField] private float avoidanceDistance = 1.2f; // Distance à laquelle il s'arrête s'il y a un pote devant
    [SerializeField] private string allyTag = "Ally";
    [SerializeField] private string buildingTag = "Building";
    protected Transform currentTarget;

    [Header("Logique Spline")]
    [SerializeField] public SplineContainer splineContainer;
    public float sideOffset = 0f; 
    private float distanceTraveled = 0f;
    
    [SerializeField] private LayerMask enemyLayer; // pour ne détecter que les ennemis

    [Header("Références Externes (Auto-remplies)")]
    protected Transform player;
    private Health playerHealth;
    private PlayerMoney playerMoney;
    protected Transform mainBuilding;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        // On trouve le joueur automatiquement au démarrage
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        GameObject mainBuildingObj = GameObject.FindGameObjectWithTag("MainBuilding");

        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = true;
            
            // On synchronise la stoppingDistance
            agent.stoppingDistance = StoppingDistance; 
        }

        // Configuration de l'obstacle
        if (obstacle != null)
        {
            obstacle.enabled = false;
            obstacle.carving = true; // Pour découper le sol
        }

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<Health>();
            playerMoney = playerObj.GetComponent<PlayerMoney>();
        }
        if (mainBuildingObj != null) mainBuilding = mainBuildingObj.transform;
    }

    public void setContainerSpline(SplineContainer container)
    {
        splineContainer = container;
    }

    public void SetInitialDistanceOffset(float offset)
    {
        // On met une distance négative pour créer un retard
        distanceTraveled = -offset;
    }

    void OnEnable()
    {
        // On écoute à l'événement de mort
        if (health != null) health.OnDeath.AddListener(Die);
    }

    void OnDisable()
    {
        // Toujours pour éviter les fuites de mémoire
        if (health != null) health.OnDeath.RemoveListener(Die);
    }

    public void TakeDamage(float amount)
    {
        if (health != null) health.TakeDamage(amount);
    }

    protected virtual void Die()
    {
        // Debug.Log($"{enemyName} est mort !");
        if (playerMoney != null)
        {
            playerMoney.Add(CalculateMoneyDropRandomized());
        }
        Destroy(gameObject);
    }

    protected int CalculateMoneyDrop()
    {
        return (int)moneyDropped;
    }
    protected int CalculateMoneyDropRandomized()
    {
        float variance = moneyDropped * 0.2f; // 20% de variance
        float randomizedAmount = moneyDropped + UnityEngine.Random.Range(-variance, variance);
        return Mathf.Max(1, Mathf.RoundToInt(randomizedAmount));
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        switch (currentState)
        {
            case EnemyState.FollowingSpline:
                FollowSplineLogic();
                CheckForTargets();
                break;

            case EnemyState.ChasingTarget:
                MoveTowardsTarget();
                CheckForTargets();
                break;

            case EnemyState.ReturningToSpline:
                CheckForTargets();
                ReturnToSplineLogic();
                break;
            case EnemyState.Attacking:
                RotateTowardsTarget(); 
                break;
        }
    }

    public void SetAttackingState(bool isAttacking)
    {
        if (isAttacking)
        {
            currentState = EnemyState.Attacking;
            if (agent != null) agent.enabled = false; 
            if (obstacle != null) obstacle.enabled = true;
        }
        else
        {
            // On rend l'objet mobile à nouveau
            if (obstacle != null) obstacle.enabled = false;

            if (agent != null) 
            {
                agent.enabled = true;
                // pour être sûr qu'il ne se téléporte pas
                agent.Warp(transform.position); 
            };

            if (currentTarget != null) currentState = EnemyState.ChasingTarget;
            else currentState = EnemyState.ReturningToSpline;
        }
    }

    protected void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // Logique sur la Spline
    private void FollowSplineLogic()
    {
        if (splineContainer == null) return;
        
        distanceTraveled += moveSpeed * Time.deltaTime;
        float actualDistance = Mathf.Max(0, distanceTraveled); // si la distance est négative, l'ennemi reste bloqué au point 0 (le départ) en attendant son tour (pour en faire spown pleins pas sur de garder).
        float t = actualDistance / splineContainer.CalculateLength();

        // Si on a atteint la fin de la spline, on meurt
        if (t >= 1f) { Die(); return; }

        // Calcul de la position sur la spline avec le décalage latéral
        Vector3 pos = (Vector3)splineContainer.EvaluatePosition(t);
        Vector3 tangent = (Vector3)splineContainer.EvaluateTangent(t);

        Vector3 sideDirection = Vector3.Cross(tangent, Vector3.up).normalized;
        // On calcule la position cible avec le décalage
        Vector3 targetPosWithOffset = pos + (sideDirection * sideOffset);

        agent.SetDestination(targetPosWithOffset);

    }

    // Détection de perso à attaquer
    private void CheckForTargets()
    {
        Transform bestTarget = null;
        float closestDistance = detectionRange;

        // On cherche d'abord si le joueur est à portée
        if (player != null && playerHealth != null && !playerHealth.IsDead)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer < closestDistance)
            {
                bestTarget = player;
                closestDistance = distToPlayer;
            }
        }

        // On cherche l'objet le plus proche avec le tag Ally
        if (bestTarget == null) {
            GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);
            foreach (GameObject ally in allies)
            {
                // On recup le collider pour calculer la distance à la surface, pas au centre
                Collider allyCollider = ally.GetComponent<Collider>();
                Vector3 targetPoint = (allyCollider != null) 
                ? allyCollider.ClosestPoint(transform.position) 
                : ally.transform.position;

                // On calcule la distance par rapport à la surface
                float distToAlly = Vector3.Distance(transform.position, targetPoint);

                // Si cet allié est à portée et plus proche que ce qu'on a trouvé avant
                if (distToAlly < closestDistance)
                {
                    closestDistance = distToAlly;
                    bestTarget = ally.transform;
                }
            }
        }

        // En troisième priorité, on cherche les bâtiments (sauf si on a déjà trouvé un joueur ou un allié à attaquer)
        if (bestTarget == null) {
            // On cherche les objets avec le tag "Building"
            GameObject[] buildings = GameObject.FindGameObjectsWithTag(buildingTag);
            foreach (GameObject bld in buildings)
            {
                Collider bldCollider = bld.GetComponent<Collider>();
                Vector3 targetPoint = (bldCollider != null) 
                ? bldCollider.ClosestPoint(transform.position) 
                : bld.transform.position;

                float distToBld = Vector3.Distance(transform.position, targetPoint);

                if (distToBld < closestDistance)
                {
                    closestDistance = distToBld;
                    bestTarget = bld.transform;
                }
            }
        }


        // On ne le check que si on n'a pas trouvé de cible plus urgente (joueur ou allié)
        if (bestTarget == null && mainBuilding != null)
        {
            float distToBuilding = Vector3.Distance(transform.position, mainBuilding.position);
            if (distToBuilding < detectionRange)
            {
                bestTarget = mainBuilding;
            }
        }

        // Application de la cible finale
        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            currentState = EnemyState.ChasingTarget;
        }
    }

    // Aller vers la cible
    private void MoveTowardsTarget()
    {
        if (currentTarget == null || (currentTarget == player && playerHealth != null && playerHealth.IsDead)) 
        { 
            currentState = EnemyState.ReturningToSpline; 
            return; 
        }
        Vector3 destination = currentTarget.position;
        Collider targetCollider = currentTarget.GetComponent<Collider>();
        
        // Si c'est un gros bâtiment, on vise le bord, pas le centre
        if (targetCollider != null && (currentTarget.CompareTag("MainBuilding") || currentTarget.CompareTag(buildingTag)))
        {
            destination = targetCollider.ClosestPoint(transform.position);
        }

        agent.isStopped = false;
        agent.SetDestination(destination);

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        // Si la cible est trop loin, on retourne à la spline
        if (distanceToTarget > detectionRange * 1.5f)
        {
            currentState = EnemyState.ReturningToSpline;
        }

    }

    // Revenir sur la spline
    private void ReturnToSplineLogic()
    {
        // Trouver le point le plus proche sur la spline par rapport à la position du cube
        // On doit convertir la position du cube en espace local à la spline
        float3 localPos = splineContainer.transform.InverseTransformPoint(transform.position);
        // Cette fonction calcule le point t le plus proche sur la courbe
        SplineUtility.GetNearestPoint(splineContainer.Spline, localPos, out float3 nearestLocalPos, out float tNearest);

        // On convertit ce point en position Monde
        Vector3 posOnSpline = splineContainer.transform.TransformPoint((Vector3)nearestLocalPos);
        // On calcule la direction perpendiculaire pour garder notre couloir
        Vector3 tangent = splineContainer.EvaluateTangent(tNearest);
        Vector3 sideDirection = Vector3.Cross(tangent, Vector3.up).normalized;

        // Cible de retour = Point sur la spline + le décalage latéral d'origine
        Vector3 targetPosWithOffset = posOnSpline + (sideDirection * sideOffset);

        if (NavMesh.SamplePosition(targetPosWithOffset, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            // Si vraiment on ne trouve pas de NavMesh, on Warp l'ennemi pour le débloquer
            agent.Warp(targetPosWithOffset);
        }

        // pathPending est true pendant que l'agent calcule. On attend qu'il ait fini.
        if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance + 0.3f))
        {
            distanceTraveled = tNearest * splineContainer.CalculateLength();
            currentState = EnemyState.FollowingSpline;
        }

    }

    private bool IsPathBlocked()
    {
        // On lance un rayon vers l'avant (à hauteur de taille pour ne pas détecter le sol)
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, avoidanceDistance))
        {
            // Si on touche quelque chose qui n'est pas notre cible actuelle
            // et que c'est un autre ennemi (ou un allié qui bloque)
            if (hit.transform != currentTarget && (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag(allyTag)))
            {
                return true; // Voie bloquée
            }
        }
        return false;
    }
}
