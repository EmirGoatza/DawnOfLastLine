using UnityEngine;

public class ResetArcherSMB : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ArcherLogic logic = animator.GetComponent<ArcherLogic>();

        if(logic != null && logic.pivot != null)
        {
            logic.arrowToShoot.SetActive(false);
            Debug.Log(logic.arrowToShoot.activeSelf + "arrow");
        }
    }
}