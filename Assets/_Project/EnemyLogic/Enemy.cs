using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour
{
    [Header("Statistiques de Base")]
    [SerializeField] protected string enemyName;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float moneyDropped = 10f;


    public enum EnemyState { FollowingSpline, ChasingTarget, ReturningToSpline }
    
    [Header("États")]
    [SerializeField] private EnemyState currentState = EnemyState.FollowingSpline;

    [Header("Détection")]
    [SerializeField] private float detectionRange = 5f;
    protected virtual float StoppingDistance => 0f;
    [SerializeField] private string allyTag = "Ally";
    protected Transform currentTarget;

    [Header("Vfx & Formation")]
    public float sideOffset = 0f; // Décalage latéral pour éviter que tous les ennemis soient exactement au même endroit sur la spline

    [Header("Spline")]
    [SerializeField] public SplineContainer splineContainer;
    private float distanceTraveled = 0f;

    protected Health health;

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
        Debug.Log($"{enemyName} est mort !");
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
        }
    }

    // Logique sur la Spline
    private void FollowSplineLogic()
    {
        if (splineContainer == null) return;
        
        distanceTraveled += moveSpeed * Time.deltaTime;
        float t = distanceTraveled / splineContainer.CalculateLength();

        // Si on a atteint la fin de la spline, on meurt
        if (t >= 1f) { Die(); return; }

        // Calcul de la position sur la spline avec le décalage latéral
        Vector3 pos = (Vector3)splineContainer.EvaluatePosition(t);
        Vector3 tangent = (Vector3)splineContainer.EvaluateTangent(t);
        Vector3 sideDirection = Vector3.Cross(tangent, Vector3.up).normalized;

        transform.position = pos + (sideDirection * sideOffset);
        transform.rotation = Quaternion.LookRotation(tangent);
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

        if (targetCollider != null)
        {
            // On vise le point le plus proche sur le bord
            destination = targetCollider.ClosestPoint(transform.position);
        }
        
        // Calcul de la distance à la surface pour savoir s'il faut abandonner ou attaquer
        float distanceToSurface = Vector3.Distance(transform.position, destination);

        // On ne bouge que si on est plus loin que la distance d'arrêt
        if (distanceToSurface > StoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }

        if (distanceToSurface > detectionRange * 1.5f)
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

        transform.position = Vector3.MoveTowards(transform.position, targetPosWithOffset, moveSpeed * Time.deltaTime);

        if (tangent != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tangent);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Une fois arrivé, on synchronise la distance et on reprend le suivi
        if (Vector3.Distance(transform.position, targetPosWithOffset) < 0.1f)
        {
            // On met à jour distanceTraveled pour que le script sache qu'on redémarre à partir de ce nouveau point "t"
            distanceTraveled = tNearest * splineContainer.CalculateLength();
            
            currentState = EnemyState.FollowingSpline;
        }
    }
}
