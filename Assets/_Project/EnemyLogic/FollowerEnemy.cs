using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class FollowerEnemy : Enemy
{
    public enum EnemyState { FollowingSpline, ChasingTarget, ReturningToSpline }
    
    [Header("États")]
    [SerializeField] private EnemyState currentState = EnemyState.FollowingSpline;

    [Header("Détection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private string allyTag = "Ally";
    protected Transform currentTarget;

    [Header("Vfx & Formation")]
    public float sideOffset = 0f; // Décalage latéral pour éviter que tous les ennemis soient exactement au même endroit sur la spline

    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer;
    private float distanceTraveled = 0f;

    public void SetupEnemy(SplineContainer container, float offset)
    {
        splineContainer = container;
        sideOffset = offset;
    }

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