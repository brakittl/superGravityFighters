using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UICursor : MonoBehaviour {
  public string mac = "";
  
  public List<GameObject> menuItems;
  int index = 0;
  bool axisHeld = false;

  void Start(){
    // Mac Check
    if (Application.platform == RuntimePlatform.OSXEditor
      || Application.platform == RuntimePlatform.OSXPlayer
      || Application.platform == RuntimePlatform.OSXPlayer)
    {
      mac = "Mac ";
    }
    else
    {
      mac = "";
    }
    transform.position = menuItems[index].transform.position;
  }
  
  void Update(){
    if(Input.GetButtonDown(mac + "Controller 1 X Button") 
      || Input.GetButtonDown(mac + "Controller 1 A Button") 
      || Input.GetButtonUp(mac + "Controller 1 Start Button")
      || Input.GetKeyDown(KeyCode.Return)
      || Input.GetKeyDown(KeyCode.KeypadEnter))
    {
      if(index == 0){
        SceneManager.LoadScene("_scene_Start");
      }
    }
    if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") >= 0.9f){
      if(!axisHeld && index < menuItems.Capacity - 1){
        index++;
        transform.position = menuItems[index].transform.position;
      }
      axisHeld = true;
    }
    else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") <= -0.9f){
      if(!axisHeld && index > 0){
        index--;
        transform.position = menuItems[index].transform.position;
      }
      axisHeld = true;
    }
    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick Y Axis") == 0){
      axisHeld = false;
    }
  }
}
