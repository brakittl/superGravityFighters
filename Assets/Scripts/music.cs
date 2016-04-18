using UnityEngine;
using System.Collections;

public class music : MonoBehaviour {
	
	void Update(){

    if(PlayerPrefs.GetFloat("music") == 1){
      gameObject.GetComponent<AudioSource>().enabled = true;
    }
    else{
      gameObject.GetComponent<AudioSource>().enabled = false;
    }
	
	}
}
