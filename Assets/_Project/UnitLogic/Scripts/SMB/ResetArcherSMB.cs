using UnityEngine;

public class ResetArcherSMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ArcherLogic logic = animator.GetComponent<ArcherLogic>();

        if(logic != null && logic.pivot != null)
        {
            logic.arrowInHand.SetActive(false);
        }
    }
}