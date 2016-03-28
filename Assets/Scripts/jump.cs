using UnityEngine;
using System.Collections;

public class jump : StateMachineBehaviour {

  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    animator.SetBool("jump", false);
  }

}
