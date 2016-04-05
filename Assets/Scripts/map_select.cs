using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class map_select : MonoBehaviour {

  int game_mode;
  int map;

  public List<GameObject> map_game_objects;
  public List<Text> game_mode_objects;

  public Text description;
  public Text abilities;
  public Text map_name;

  Color red = new Color();
  Color white = new Color();
  
  public SpriteRenderer left_arrow_sr;
  public SpriteRenderer right_arrow_sr;

	// Use this for initialization
	void Start(){

    game_mode = 0;
    map = 0;

    ColorUtility.TryParseHtmlString("#ec393d", out red);
    ColorUtility.TryParseHtmlString("#ebebeb", out white);

	}
	
	// Update is called once per frame
	void Update(){

    if(Input.GetKeyDown(KeyCode.RightArrow)){
      ++map;
      if(map >= map_game_objects.Count){
        map = 0;
      }
      right_arrow_sr.color = red;
      
      map_name.text = map_game_objects[map].name.ToUpper();

      foreach(GameObject map_GO in map_game_objects){
        map_GO.SetActive(false);
      }

      map_game_objects[map].SetActive(true);
    }

    else if(Input.GetKeyDown(KeyCode.LeftArrow)){
      --map;
      if(map < 0){
        map = map_game_objects.Count - 1;
      }
      left_arrow_sr.color = red;
      
      map_name.text = map_game_objects[map].name.ToUpper();
      
      foreach(GameObject map_GO in map_game_objects){
        map_GO.SetActive(false);
      }
      
      map_game_objects[map].SetActive(true);
    }

    else if(Input.GetKeyDown(KeyCode.UpArrow)){
      --game_mode;
      if(game_mode < 0){
        game_mode = game_mode_objects.Count - 1;
      }
    }

    else if(Input.GetKeyDown(KeyCode.DownArrow)){
      ++game_mode;
      if(game_mode >= game_mode_objects.Count){
        game_mode = 0;
      }
    }

    else{
      right_arrow_sr.color = white;
      left_arrow_sr.color = white;
    }

    

    foreach(Text game_mode_GO in game_mode_objects){
      game_mode_GO.color = white;
    }
    game_mode_objects[game_mode].color = red;

    if(game_mode == 0){
      description.text = "Survive to win! Last man standing wins after 10 lives. Come back as a ghost and poison your nemisis if you lose all 10 lives.";
      abilities.text = "Gravity swap\nSlash\n1 bullet per life";
    }
    else if(game_mode == 1){
      description.text = "Kill to win! Be the first player to score 10 kills. Continue to fight with infinite lives, and become the gravity swap champion.";
      abilities.text = "Gravity swap\nSlash\n1 bullet per life";
    }
    else{
      description.text = "Run to win! Control the gravity stone to win 100 points. Steal they stone from your enemies by\ntouching them.";
      abilities.text = "Gravity swap";
    }
	
	}
}
