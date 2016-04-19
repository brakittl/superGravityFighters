using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class music : MonoBehaviour {

  // float load_delay = 0.05f;

  void Start(){

    if(PlayerPrefs.HasKey("audio_time")){
      gameObject.GetComponent<AudioSource>().time = PlayerPrefs.GetFloat("audio_time");
    }
    else{
      PlayerPrefs.SetFloat("audio_time", gameObject.GetComponent<AudioSource>().time);
    }

  }
	
	void Update(){

    if(PlayerPrefs.GetFloat("music") == 1){
      gameObject.GetComponent<AudioSource>().enabled = true;
    }
    else{
      gameObject.GetComponent<AudioSource>().enabled = false;
    }


	
	}
}
