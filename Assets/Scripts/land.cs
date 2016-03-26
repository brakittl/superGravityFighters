using UnityEngine;
using System.Collections;

public class land : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("landing", true);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("landing", false);
    animator.SetBool("attack", false);
	}

}
