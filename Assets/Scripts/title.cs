using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class title : MonoBehaviour {

  int i;

  public List<Text> menu_objects;

  Color red = new Color();
  Color white = new Color();

	// Use this for initialization
	void Start(){

    i = 0;

    ColorUtility.TryParseHtmlString("#ec393d", out red);
    ColorUtility.TryParseHtmlString("#ebebeb", out white);
	
	}
	
	// Update is called once per frame
	void Update(){

    if(Input.GetKeyDown(KeyCode.UpArrow)){
      --i;
      if(i < 0){
        i = menu_objects.Count - 1;
      }
    }

    else if(Input.GetKeyDown(KeyCode.DownArrow)){
      ++i;
      if(i >= menu_objects.Count){
        i = 0;
      }
    }

    foreach(Text menu_GO in menu_objects){
      menu_GO.color = white;
    }
    menu_objects[i].color = red;
	
	}
}
