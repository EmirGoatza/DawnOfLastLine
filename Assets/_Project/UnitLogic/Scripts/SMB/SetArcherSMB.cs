using UnityEngine;

public class SetArcherSMB : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      ArcherLogic logic = animator.GetComponent<ArcherLogic>();
      
      if (logic.arrowInHand != null)
          logic.arrowInHand.SetActive(true);
    }


}
