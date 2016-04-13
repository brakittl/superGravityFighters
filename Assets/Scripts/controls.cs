using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class controls : MonoBehaviour {

	public string mac = "";
  
  // Use this for initialization
  void Start(){
    // Mac Check
    if (Application.platform == RuntimePlatform.OSXEditor
      || Application.platform == RuntimePlatform.OSXPlayer
      || Application.platform == RuntimePlatform.OSXPlayer){
      mac = "Mac ";
    }
    else{
      mac = "";
    }
  
  }
  
  // Update is called once per frame
  void Update(){

    if(Input.GetButtonDown(mac + "Controller 1 B Button") ||
       Input.GetButtonDown(mac + "Controller 1 A Button") ||
       Input.GetButtonDown(mac + "Controller 1 Back Button") ||
       Input.GetButtonDown(mac + "Controller 1 Start Button") ||
       Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.B)){
      SceneManager.LoadScene("_title");
    }
  
  }
}
