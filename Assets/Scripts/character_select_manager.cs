using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class character_select_manager : MonoBehaviour {

  public string mac = "";

  public GameObject ready;
  public GameObject ready_text;

  string[] player_prefab_names = new string[] {"black_player", "blue_player", "green_player",  "purple_player", "red_player", "yellow_player"};

  public Dictionary<string, int> selected_character = new Dictionary<string, int>(){
    {"P1", -2},
    {"P2", -2},
    {"P3", -2},
    {"P4", -2}
  };

  public Dictionary<string, int> ready_character = new Dictionary<string, int>(){
    {"P1", -2},
    {"P2", -2},
    {"P3", -2},
    {"P4", -2}
  };

  public bool character_is_selected(string player,int value){
    if(selected_character[player] == value){
      return false; // false because current player has it selected
    }
    for(int i = 1; i <= 4; ++i){
      if(("P" + i) != player){
        if(selected_character["P" + i] == value){
          return true;
        }
      }
    }
    return false;
  }

  public bool character_is_ready(string player, int value){
    if(ready_character[player] == value){
      return false; // false because current player has it ready
    }
    for(int i = 1; i <= 4; ++i){
      if(("P" + i) != player){
        if(ready_character["P" + i] == value){
          return true;
        }
      }
    }
    return false;
  }

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

    PlayerPrefs.SetString("GameMode", "SURVIVAL");
    level.S.gamemode = GameMode.SURVIVAL;

    selected_character["P1"] = PlayerPrefs.GetInt("P1Num");
    selected_character["P2"] = PlayerPrefs.GetInt("P2Num");
    selected_character["P3"] = PlayerPrefs.GetInt("P3Num");
    selected_character["P4"] = PlayerPrefs.GetInt("P4Num");

    ready_character["P1"] = PlayerPrefs.GetInt("P1Num");
    ready_character["P2"] = PlayerPrefs.GetInt("P2Num");
    ready_character["P3"] = PlayerPrefs.GetInt("P3Num");
    ready_character["P4"] = PlayerPrefs.GetInt("P4Num");
  }

  void Update(){

    ready.SetActive(all_players_ready());
    ready_text.SetActive(all_players_ready());

    if(all_players_ready()){
      if(Input.GetButtonUp(mac + "Controller 1 Start Button")
      || Input.GetButtonUp(mac + "Controller 2 Start Button")
      || Input.GetButtonUp(mac + "Controller 3 Start Button")
      || Input.GetButtonUp(mac + "Controller 4 Start Button")
      || Input.GetKeyDown(KeyCode.Return)){
        for(int i = 1; i <= 4; i++){
          if(ready_character["P" + i] >= 0){
            PlayerPrefs.SetInt("P" + i + "Num", ready_character["P" + i]);
            PlayerPrefs.SetString("P" + i, player_prefab_names[ready_character["P" + i]]);
                        
          }
          else{
            PlayerPrefs.SetInt("P" + i + "Num", -2);
            PlayerPrefs.SetString("P" + i, "none");
          }
        }

        foreach(string p_string in player_prefab_names)
                {
                    print(p_string);
                }

        SceneManager.LoadScene("_map_game_select");
      }
    }    
  }

  bool all_players_ready(){
    int ready_count = 0;
    for(int i = 1; i <= 4; ++i){
      if(ready_character["P" + i] == -1){
        return false;
      }
      if(ready_character["P" + i] >= 0){
        ++ready_count;
      }
    }

    if(ready_count > 1)
        {
            
            return true;
    }

    else{
      return false;
    }

  }
}
