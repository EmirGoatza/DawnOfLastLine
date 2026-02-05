using UnityEngine;
using UnityEngine.Splines;

public class FollowerEnemy : Enemy
{
    public enum EnemyState { FollowingSpline, ChasingTarget, ReturningToSpline }
    
    [Header("États")]
    [SerializeField] private EnemyState currentState = EnemyState.FollowingSpline;

    [Header("Détection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private string allyTag = "Ally";
    protected Transform currentTarget;

    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer;
    private float distanceTraveled = 0f;

    protected override void Move()
    {
        switch (currentState)
        {
            case EnemyState.FollowingSpline:
                FollowSplineLogic();
                CheckForTargets();
                break;

            case EnemyState.ChasingTarget:
                MoveTowardsTarget();
                break;

            case EnemyState.ReturningToSpline:
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

        transform.position = (Vector3)splineContainer.EvaluatePosition(t);
        transform.rotation = Quaternion.LookRotation((Vector3)splineContainer.EvaluateTangent(t));
    }

    // Détection de perso à attaquer
    private void CheckForTargets()
    {
        // On cherche d'abord si le joueur est à portée
        if (player != null && Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            currentTarget = player;
            currentState = EnemyState.ChasingTarget;
            return;
        }

        // On cherche l'objet le plus proche avec le tag Ally
        GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);
        foreach (GameObject ally in allies)
        {
            if (Vector3.Distance(transform.position, ally.transform.position) < detectionRange)
            {
                currentTarget = ally.transform;
                currentState = EnemyState.ChasingTarget;
                break;
            }
        }
    }

    // Aller vers la cible
    private void MoveTowardsTarget()
    {
        if (currentTarget == null) { currentState = EnemyState.ReturningToSpline; return; }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
        
        // Si la cible est trop loin ou si l'action est finie, on retourne à la spline
        if (Vector3.Distance(transform.position, currentTarget.position) > detectionRange * 1.5f)
        {
            currentState = EnemyState.ReturningToSpline;
        }
    }

    // Revenir sur la spline
    private void ReturnToSplineLogic()
    {
        // On calcule la position où il devrait être sur la spline
        float t = distanceTraveled / splineContainer.CalculateLength();
        Vector3 targetSplinePos = (Vector3)splineContainer.EvaluatePosition(t);

        transform.position = Vector3.MoveTowards(transform.position, targetSplinePos, moveSpeed * Time.deltaTime);

        // Si on est assez proche du point sur la spline, on reprend le suivi normal
        if (Vector3.Distance(transform.position, targetSplinePos) < 0.1f)
        {
            currentState = EnemyState.FollowingSpline;
        }
    }
}