using UnityEngine;
using System.Collections;

public class character_select_border : MonoBehaviour {

  public Sprite red_line;
  public GameObject box;

  public bool touched = false;

  void Start(){
    if(PlayerPrefs.GetFloat("tutorial") == 0){
      touched = true;
    }
    else{
      touched = false;
    }
  }

  void OnCollisionEnter2D(Collision2D coll){
    if(!touched){
      gameObject.GetComponent<SpriteRenderer>().sprite = red_line;
      box.GetComponent<character_select>().borders_touched += 1;
      touched = true;
    }
  }

}
