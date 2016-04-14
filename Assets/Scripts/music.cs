using UnityEngine;
using System.Collections;

public class music : MonoBehaviour {
	
	void Update(){

    if(PlayerPrefs.GetInt("music") == 1){
      gameObject.GetComponent<AudioSource>().enabled = true;
    }
    else{
      gameObject.GetComponent<AudioSource>().enabled = false;
    }
	
	}
}
