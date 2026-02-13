using UnityEngine;

public class AttackAnimationEventSMB : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TroopLogic troop = animator.GetComponent<TroopLogic>();
        if (troop != null)
        {
            troop.OnAttackAnimationEnd();
        }
    }
}