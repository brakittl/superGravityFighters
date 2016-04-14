using UnityEngine;
using System.Collections;

public class swapping : StateMachineBehaviour {

  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    animator.SetBool("swapping", true);
  }

  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    animator.SetBool("swapping", false);
  }

}
