using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class map_select : MonoBehaviour {

  public string mac = "";

  int game_mode;
  public int map;

  public List<GameObject> map_game_objects;
  public List<Text> game_mode_objects;

  public Text description;
  public Text abilities;
  public Text map_name;

  public GameObject camera;

  Color red = new Color();
  Color white = new Color();
  
  public SpriteRenderer left_arrow_sr;
  public SpriteRenderer right_arrow_sr;

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

    game_mode = 0;
    map = 0;
    axis_held_x = axis_held_y = false;

    ColorUtility.TryParseHtmlString("#ec393d", out red);
    ColorUtility.TryParseHtmlString("#ebebeb", out white);

	}
	
	// Update is called once per frame
	void Update(){

    if(Input.GetKeyDown(KeyCode.RightArrow) ||
       (!axis_held_x && Input.GetAxis(mac + "Controller 1 Left Stick X Axis") >= 0.95f)){
      
      axis_held_x = true;

      ++map;
      if(map >= map_game_objects.Count){
        map = 0;
      }
      
      // right_arrow_sr.color = red;
      
      map_name.text = map_game_objects[map].name.ToUpper();
      map_name.color = white;

      foreach(GameObject map_GO in map_game_objects){
        map_GO.SetActive(false);
      }

      map_game_objects[map].SetActive(true);
    }

    else if(Input.GetKeyDown(KeyCode.LeftArrow) ||
            (!axis_held_x && Input.GetAxis(mac + "Controller 1 Left Stick X Axis") <= -0.95f)){
      
      axis_held_x = true;

      --map;
      if(map < 0){
        map = map_game_objects.Count - 1;
      }

      // left_arrow_sr.color = red;
      
      map_name.text = map_game_objects[map].name.ToUpper();
      map_name.color = white;
      
      foreach(GameObject map_GO in map_game_objects){
        map_GO.SetActive(false);
      }
      
      map_game_objects[map].SetActive(true);
    }

    else if(Input.GetKeyDown(KeyCode.UpArrow) ||
            (!axis_held_y && Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") <= -0.9f)){
      
      axis_held_y = true;

      --game_mode;
      if(game_mode < 0){
        game_mode = game_mode_objects.Count - 1;
      }
      axis_held_y = true;
    }

    else if(Input.GetKeyDown(KeyCode.DownArrow) ||
            (!axis_held_y && Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") >= 0.9f)){
      
      axis_held_y = true;

      ++game_mode;
      if(game_mode >= game_mode_objects.Count){
        game_mode = 0;
      }
      axis_held_y = true;
    }

    else{
      right_arrow_sr.color = white;
      left_arrow_sr.color = white;
    }

    if(Input.GetButtonDown(mac + "Controller 1 B Button")
      || Input.GetButtonDown(mac + "Controller 2 B Button")
      || Input.GetButtonDown(mac + "Controller 3 B Button")
      || Input.GetButtonDown(mac + "Controller 4 B Button")
      || Input.GetKeyDown(KeyCode.B)
      || Input.GetKeyDown(KeyCode.Escape)){

      if(PlayerPrefs.GetString("screen") == "MONITOR"){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_character_select");
      }
      else{
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_character_select_tabletop");
      }
      
    }

    if(Input.GetKeyDown(KeyCode.Return) ||
       Input.GetButtonDown(mac + "Controller 1 Start Button") ){

      if(game_mode == 0){
        PlayerPrefs.SetString("GameMode", "SURVIVAL");
      }

      else if(game_mode == 1){
        PlayerPrefs.SetString("GameMode", "REVERSE_TAG");
      }

      if(map == 0){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_galaxy");
      }
      else if(map == 1){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_city");
      }
      else if(map == 2){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_desert");
      }
      else if(map == 3){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_forest");
      }
      else if(map == 4){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_volcano");
      }

    }

    if(Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown(mac + "Controller 1 Y Button") ){
      SceneManager.LoadScene("_game_options");
    }

    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick Y Axis") == 0){
      axis_held_y = false;
    }

    if (Input.GetAxisRaw(mac + "Controller 1 Left Stick X Axis") == 0){
      axis_held_x = false;
    }

    foreach (Text game_mode_GO in game_mode_objects){
      game_mode_GO.color = white;
    }
    game_mode_objects[game_mode].color = red;

    if(game_mode == 0){
      description.text = "Survive to win! In this combat mode, the last man standing wins. Stay alive as your opponents use burst attacks and bullets to try and kill you.";
      abilities.text = "Gravity swap\nBurst attack\n1 bullet per life";
    }
    // else if(game_mode == 1){
    //   description.text = "Kill to win! Be the first player to score 10 kills. Continue to fight with infinite lives, and become the gravity swap champion.";
    //   abilities.text = "Gravity swap\nBurst attack\n1 bullet per life";
    // }
    else{
      description.text = "Run to win! In this non-combat mode, stay in control of the gravity stone to win points. Steal they stone from your enemies by touching them.";
      abilities.text = "Gravity swap";
    }
	
	}
}
