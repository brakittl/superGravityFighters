using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class options : MonoBehaviour {

	public string mac = "";
  public int i;
  public List<Text> menu_objects;

  Color red = new Color();
  Color white = new Color();

  bool axis_held_x, axis_held_y;

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

    i = 0;
    axis_held_x = false;
    axis_held_y = false;

    ColorUtility.TryParseHtmlString("#ec393d", out red);
    ColorUtility.TryParseHtmlString("#ebebeb", out white);

    if(PlayerPrefs.HasKey("screen")){
      menu_objects[0].text = PlayerPrefs.GetString("screen");
    }
    else{
      PlayerPrefs.SetString("screen", menu_objects[0].text);
    }
    
    if(PlayerPrefs.HasKey("tutorial")){
      if(PlayerPrefs.GetFloat("tutorial") > 0){
        menu_objects[1].text = "ON";
      }
      else{
        menu_objects[1].text = "OFF";
      }
    }
    else{
      PlayerPrefs.SetFloat("tutorial", 1);
    }
    
    if(PlayerPrefs.HasKey("music")){
      if(PlayerPrefs.GetFloat("music") > 0){
        menu_objects[2].text = "ON";
      }
      else{
        menu_objects[2].text = "OFF";
      }
    }
    else{
      PlayerPrefs.SetFloat("music", 1);
    }
    
    if(PlayerPrefs.HasKey("sfx")){
      if(PlayerPrefs.GetFloat("sfx") > 0){
        menu_objects[3].text = "ON";
      }
      else{
        menu_objects[3].text = "OFF";
      }
    }
    else{
      PlayerPrefs.SetFloat("screen", 1);
    }
  
  }
  
  // Update is called once per frame
  void Update(){

    if(Input.GetKeyDown(KeyCode.UpArrow) ||
       (!axis_held_y && Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") <= -0.9f)){
      --i;
      if(i < 0){
        i = menu_objects.Count - 1;
      }
      axis_held_y = true;
    }

    if(Input.GetKeyDown(KeyCode.DownArrow) ||
       (!axis_held_y && Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") >= 0.9f)){
      ++i;
      if(i >= menu_objects.Count){
        i = 0;
      }
      axis_held_y = true;
    }

    if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) ||
       (!axis_held_x && Input.GetAxis(mac + "Controller 1 Left Stick X Axis") >= 0.9f) ||
       (!axis_held_x && Input.GetAxis(mac + "Controller 1 Left Stick X Axis") <= -0.9f) ){
      ToggleOption();
      axis_held_x = true;
    }

    if(Input.GetButtonDown(mac + "Controller 1 A Button") ||
       Input.GetButtonDown(mac + "Controller 1 Start Button") ||
       Input.GetKeyDown(KeyCode.Return)){
      if(i != 4){
        ToggleOption();
      }
      else{
        SceneManager.LoadScene("_title");
      }
    }

    if(Input.GetButtonDown(mac + "Controller 1 B Button") ||
       Input.GetButtonDown(mac + "Controller 1 Back Button") ||
       Input.GetKeyDown(KeyCode.B)){
      SceneManager.LoadScene("_title");
    }

    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick Y Axis") == 0){
      axis_held_y = false;
    }

    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick X Axis") == 0){
      axis_held_y = false;
    }

    foreach(Text menu_GO in menu_objects){
      menu_GO.color = white;
    }
    menu_objects[i].color = red;
  
  }

  void ToggleOption(){
    if(i == 0){
      if(menu_objects[i].text == "MONITOR"){
        menu_objects[i].text = "TABLETOP";
        PlayerPrefs.SetString("screen", "TABLETOP");
      }
      else{
        menu_objects[i].text = "MONITOR";
        PlayerPrefs.SetString("screen", "MONITOR");
      }
    }
    else if(i == 1){
      if(menu_objects[i].text == "ON"){
        menu_objects[i].text = "OFF";
        PlayerPrefs.SetFloat("tutorial", 0);
      }
      else{
        menu_objects[i].text = "ON";
        PlayerPrefs.SetFloat("tutorial", 1);
      }
    }
    else if(i == 2){
      if(menu_objects[i].text == "ON"){
        menu_objects[i].text = "OFF";
        PlayerPrefs.SetFloat("music", 0);
      }
      else{
        menu_objects[i].text = "ON";
        PlayerPrefs.SetFloat("music", 1);
      }
    }
    else if(i == 3){
      if(menu_objects[i].text == "ON"){
        menu_objects[i].text = "OFF";
        PlayerPrefs.SetFloat("sfx", 0);
      }
      else{
        menu_objects[i].text = "ON";
        PlayerPrefs.SetFloat("sfx", 1);
      }
    }
  }

}
