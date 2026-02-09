using UnityEngine;

public class ShootArrowSMB : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ArcherLogic logic = animator.GetComponent<ArcherLogic>();
        float arrowSpeed = logic.arrowSpeed;
        
        if(logic != null && logic.arrowToShoot != null && logic.arrowInHand != null && logic.targetEnemy != null)
        {
            Quaternion pivotBeforeRotation = logic.pivot.rotation;
            
            if(logic.isAiming && logic.pivot != null)
            {
                logic.pivot.rotation = logic.GetCurrentLerpedRotation();
            }
            
            Vector3 spawnPosition = logic.arrowInHand.transform.position;
            Vector3 targetPosition = logic.targetEnemy.position + logic.aimOffset;
            Vector3 direction = (targetPosition - spawnPosition).normalized;
            Quaternion arrowRotation = Quaternion.LookRotation(direction);
            
            GameObject arrow = Instantiate(logic.arrowToShoot, spawnPosition, arrowRotation);
            
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddForce(direction * arrowSpeed, ForceMode.VelocityChange);
            }
            
            logic.pivot.rotation = pivotBeforeRotation;
        }
    }
}