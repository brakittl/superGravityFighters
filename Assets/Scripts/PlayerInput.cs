using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
  public int player = 1;
  GameObject xbutton, text;
  public GameObject playerPrefab;
  public Vector3 startPosition;
  GameObject playerObject;
  bool confirmed, joined;

  void Start(){
    xbutton = GameObject.Find("P" + player + " X Button Image");
    text = GameObject.Find("P" + player + " Text");
    Debug.Log(player);
    if(player > Input.GetJoystickNames().Length){
      this.gameObject.SetActive(false);
    }
    joined = false;
    confirmed = false;
  }
  
  void Update(){
    // UI controls waiting for player to join
    if(!joined){
      if (Input.GetButtonDown("Controller " + player + " X Button"))
      {
        joined = true;
        ShowJoinUI(false);
        ShowCharacterSelectUI(true);
        ShowPlayer(true);
      }
    }
    else if (!confirmed)
    {
      playerObject.transform.position = startPosition;
      if (Input.GetButtonDown("Controller " + player + " Right Bumper"))
      {
        // change to next character
        ChangeCharacter();
      }
      else if (Input.GetButtonDown("Controller " + player + " Left Bumper"))
      {
        // change to previous character
        ChangeCharacter();
      }
      else if (Input.GetButtonDown("Controller " + player + " Start Button"))
      {
        // hide character select UI and let player move around
        confirmed = true;
        ShowCharacterSelectUI(false);
        UnlockCharacter(true);
      }
      else if (Input.GetButtonDown("Controller " + player + " Back Button"))
      {
        // return to unjoined UI
        joined = false;
        ShowJoinUI(true);
        ShowCharacterSelectUI(false);
        ShowPlayer(false);
      }
    }
    else
    {
      // return to character selector
      if (Input.GetButtonDown("Controller " + player + " Back Button"))
      {
        confirmed = false;
        ShowCharacterSelectUI(true);
        UnlockCharacter(false);
      }
    }
  }

  // toggles join UI
  void ShowJoinUI(bool value)
  {
    if (xbutton != null)
    {
      xbutton.SetActive(value);
    }
    if (text != null)
    {
      text.SetActive(value);
    }
  }

  // toggles character select UI
  void ShowCharacterSelectUI(bool value)
  {
    
  }

  // toggles Player
  void ShowPlayer(bool value)
  {
    if (value)
    {
      playerObject = Instantiate(playerPrefab) as GameObject;
      playerObject.transform.position = startPosition;
      playerObject.GetComponent<player>().acceleration = 0.7f;
      playerObject.GetComponent<player>().player_number = 0;
    }
    else
    {
      Destroy(playerObject);
    }
  }

  // cycles player prefab
  void ChangeCharacter()
  {

  }

  // disables the colliders locking the player into the character select
  void UnlockCharacter(bool value)
  {
    if (value)
    {
      playerObject.GetComponent<player>().player_number = player;
    }
    else
    {
      playerObject.transform.position = startPosition;
      playerObject.GetComponent<player>().player_number = 0;
    }
  }
}
