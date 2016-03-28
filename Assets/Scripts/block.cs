using UnityEngine;
using System.Collections;

public class block : StateMachineBehaviour {

  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    animator.SetBool("block", false);
  }

}
