using UnityEngine;
using System.Collections;

public class attack : StateMachineBehaviour {

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("attack", false);
	}

}
