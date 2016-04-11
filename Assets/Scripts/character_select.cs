using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class character_select : MonoBehaviour {
  public string mac = "";

  public int player;
  public Vector3 start_position;
  public List<GameObject> player_prefabs;

  Vector3 resize;

  public GameObject manager;
  
  public GameObject join_set;
  public GameObject select_set;
  public GameObject return_set;
  public GameObject podium;

  public GameObject join_button;
  public GameObject confirm_button;
  public GameObject cancel_button;

  int prefab_number = 0;
  GameObject player_object;

  Color character_color = new Color(1, 1, 1, 1);

  bool joined;
  bool selected;
  bool axis_in_use;

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

    joined = false;
    selected = false;
    axis_in_use = false;

    if(player > Input.GetJoystickNames().Length){
    // if(player > 1){
      this.gameObject.SetActive(false);
      join_button.SetActive(false);
    }

    resize = new Vector3(1f, 1f, 1f);
	
	}
	
	void Update(){

    if(System.Math.Abs(Input.GetAxis(mac + "Controller " + player + " Left Stick X Axis")) < 0.9f){
      axis_in_use = false;
    }

    // axis_in_use = false;

    // set color to black if already selected
    if(player_object != null){
      if(manager.GetComponent<character_select_manager>().character_is_selected("P" + player, prefab_number)){
        player_object.GetComponent<SpriteRenderer>().color = Color.black;
      }
      else{
        player_object.GetComponent<SpriteRenderer>().color = character_color;
      }
    }

    // join
    if(!joined){

      // join game
      if(Input.GetButtonDown(mac + "Controller " + player + " X Button") ||
         Input.GetKeyDown(KeyCode.X)){
        Join();
      }

      // join game
      if(Input.GetButtonDown(mac + "Controller " + player + " B Button") ||
         Input.GetButtonDown(mac + "Controller " + player + " Back Button") ||
         Input.GetKeyDown(KeyCode.B)){
        SceneManager.LoadScene("_scene_Menu");
      }      

    }

    // in character select
    else if(joined && !selected){

      // change color right
      if(Input.GetButtonDown(mac + "Controller " + player + " Right Bumper") ||
         (!axis_in_use && Input.GetAxis(mac + "Controller " + player + " Left Stick X Axis") >= 0.95f) ||
         Input.GetKeyDown(KeyCode.RightArrow)){
        Cycle(1);
      }

      // change color left
      if(Input.GetButtonDown(mac + "Controller " + player + " Left Bumper") ||
         (!axis_in_use && Input.GetAxis(mac + "Controller " + player + " Left Stick X Axis") <= -0.95f) ||
         Input.GetKeyDown(KeyCode.LeftArrow)){
        Cycle(-1);
      }

      // confirm color
      if(Input.GetButtonDown(mac + "Controller " + player + " A Button") ||
         Input.GetKeyDown(KeyCode.A)){
        Confirm();
      }

      // unjoin game
      if(Input.GetButtonDown(mac + "Controller " + player + " Back Button") ||
         Input.GetKeyDown(KeyCode.B)){
        Unjoin();
      }

    }

    else{

      // return to join screen
      if(Input.GetButtonDown(mac + "Controller " + player + " Back Button") ||
         Input.GetKeyDown(KeyCode.B)){
        Unconfirm();
      }

    }

	}

  void Join(){
    joined = true;
    join_set.SetActive(false);
    join_button.SetActive(false);
    select_set.SetActive(true);
    confirm_button.SetActive(true);
    podium.SetActive(true);
    Show(true);
    manager.GetComponent<character_select_manager>().selected_character["P" + player] = -1;
  }

  void Unjoin(){
    Show(false);
    joined = false;
    join_set.SetActive(true);
    join_button.SetActive(true);
    select_set.SetActive(false);
    confirm_button.SetActive(false);
    podium.SetActive(false);
    manager.GetComponent<character_select_manager>().selected_character["P" + player] = -2;
  }

  void Confirm(){
    if(!manager.GetComponent<character_select_manager>().character_is_selected("P" + player, prefab_number)){
      selected = true;
      select_set.SetActive(false);
      confirm_button.SetActive(false);
      podium.SetActive(false);
      return_set.SetActive(true);
      cancel_button.SetActive(true);
      player_object.GetComponent<player>().player_number = player;
      manager.GetComponent<character_select_manager>().selected_character["P" + player] = prefab_number;
    }
  }

  void Unconfirm(){
    Show(false);
    selected = false;
    Join();
    return_set.SetActive(false);
    cancel_button.SetActive(false);
    manager.GetComponent<character_select_manager>().selected_character["P" + player] = -1;
  }

  void Show(bool value){
    if(value){
      player_object = Instantiate(player_prefabs[prefab_number]) as GameObject;
      player_object.transform.position = start_position;
      player_object.transform.localScale = resize;
      player_object.GetComponent<player>().player_number = 0;
    }
    else{
      Destroy(player_object);
    }
  }

  // cycles player prefab
  void Cycle(int value){

    axis_in_use = true;

    Destroy(player_object);
    prefab_number += value;

    if(prefab_number >= player_prefabs.Capacity){
      prefab_number -= player_prefabs.Capacity;
    }
    else if(prefab_number < 0){
      prefab_number += player_prefabs.Capacity;
    }

    Show(true);
  }

}
