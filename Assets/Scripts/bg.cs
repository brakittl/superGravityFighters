using UnityEngine;
using System.Collections;

public class bg : MonoBehaviour {

  public Animator background_animator;
  public string map;

	void Start(){
    background_animator.Play(map);
	}
	
}
