using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class game_options : MonoBehaviour {

	public string mac = "";
  public int i;
  public List<Text> menu_objects;

  public List<int> lives_options = new List<int>(){1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
  public List<int> rt_options = new List<int>(){25, 50, 100, 150, 250};
  public int lives_index = 4;
  public int rt_index = 1;

  public GameObject camera;

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

    if(!PlayerPrefs.HasKey("lives")){
      lives_index = 4;
      PlayerPrefs.SetInt("lives", lives_options[lives_index]);
      PlayerPrefs.SetInt("lives_index", lives_index);
    }
    menu_objects[0].text = PlayerPrefs.GetInt("lives").ToString();
    lives_index = PlayerPrefs.GetInt("lives_index");
    
    if(!PlayerPrefs.HasKey("rt_point_limit")){
      rt_index = 1;
      PlayerPrefs.SetInt("rt_point_limit", rt_options[rt_index]);
      PlayerPrefs.SetInt("rt_index", rt_index);
    }
    menu_objects[1].text = PlayerPrefs.GetInt("rt_point_limit").ToString();
    rt_index = PlayerPrefs.GetInt("rt_index");
  
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

    if(Input.GetKeyDown(KeyCode.LeftArrow) ||
       (!axis_held_x && Input.GetAxis(mac + "Controller 1 Left Stick X Axis") <= -0.9f) ){
      ToggleOption(false);
      axis_held_x = true;
    }

    if(Input.GetKeyDown(KeyCode.RightArrow) ||
       (!axis_held_x && Input.GetAxis(mac + "Controller 1 Left Stick X Axis") >= 0.9f) ){
      ToggleOption(true);
      axis_held_x = true;
    }

    if(Input.GetButtonDown(mac + "Controller 1 A Button") ||
       Input.GetButtonDown(mac + "Controller 1 Start Button") ||
       Input.GetButtonDown(mac + "Controller 1 B Button") ||
       Input.GetButtonDown(mac + "Controller 1 Back Button") ||
       Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.B)){
      PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
      SceneManager.LoadScene("_map_game_select");
    }

    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick Y Axis") == 0){
      axis_held_y = false;
    }

    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick X Axis") == 0){
      axis_held_x = false;
    }

    foreach(Text menu_GO in menu_objects){
      menu_GO.color = white;
    }
    menu_objects[i].color = red;
  
  }

  void ToggleOption(bool right){
    // lives
    if(i == 0){
      if(right){
        ++lives_index;
        if(lives_index >= lives_options.Count){
          lives_index = 0;
        }
      }
      else{
        --lives_index;
        if(lives_index < 0){
          lives_index = lives_options.Count - 1;
        }
      }
      PlayerPrefs.SetInt("lives", lives_options[lives_index]);
      PlayerPrefs.SetInt("lives_index", lives_index);
      menu_objects[0].text = PlayerPrefs.GetInt("lives").ToString();
    }
    else if(i == 1){
      if(right){
        ++rt_index;
        if(rt_index >= rt_options.Count){
          rt_index = 0;
        }
      }
      else{
        --rt_index;
        if(rt_index < 0){
          rt_index = rt_options.Count - 1;
        }
      }
      PlayerPrefs.SetInt("rt_point_limit", rt_options[rt_index]);
      PlayerPrefs.SetInt("rt_index", rt_index);
      menu_objects[1].text = PlayerPrefs.GetInt("rt_point_limit").ToString();
    }
  }

}
