using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour {
  public int player = 1;
  GameObject xbutton, text, leftBumper, rightBumper;
  GameObject UI, backTochange;
  public List<GameObject> playerPrefabs;
  int prefabNumber = 0;
  GameObject playerPrefab;
  public Vector3 startPosition;
  GameObject playerObject, playerStand;
  bool confirmed, joined;
  Color characterColor = new Color(1,1,1,1);
  bool axisInUse = false;

  void Start(){
    if (player > Input.GetJoystickNames().Length)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      backTochange = GameObject.Find("P" + player + " BackToChange");
      if (backTochange != null)
      {
        backTochange.SetActive(false);
      }
      UI = GameObject.Find("UI");
      leftBumper = GameObject.Find("P" + player + " Left Bumper Image");
      if (leftBumper != null)
      {
        leftBumper.SetActive(false);
      }
      rightBumper = GameObject.Find("P" + player + " Right Bumper Image");
      if (rightBumper != null)
      {
        rightBumper.SetActive(false);
      }
      playerStand = GameObject.Find("P" + player + " Stand");
      startPosition = playerStand.transform.position + new Vector3(0, 0.3f, 0);
      xbutton = GameObject.Find("P" + player + " X Button Image");
      text = GameObject.Find("P" + player + " Text");
      Debug.Log(player);
      if (player > Input.GetJoystickNames().Length)
      {
        this.gameObject.SetActive(false);
      }
      joined = false;
      confirmed = false;
    }
  }
  
  void Update(){
    if (System.Math.Abs(Input.GetAxis("Controller " + player + " Left Stick X Axis")) < 0.9f)
    {
      axisInUse = false;
    }
    if (playerObject != null)
    {
      if (UI.GetComponent<CharacterSelectUI>().characterIsSelected("P" + player, prefabNumber))
      {
        playerObject.GetComponent<SpriteRenderer>().color = Color.black;
      }
      else
      {
        playerObject.GetComponent<SpriteRenderer>().color = characterColor;
      }
    }

    // UI controls waiting for player to join
    if(!joined){
      if (Input.GetButtonDown("Controller " + player + " X Button"))
      {
        joined = true;
        ShowJoinUI(false);
        ShowCharacterSelectUI(true);
        ShowPlayer(true);
        UI.GetComponent<CharacterSelectUI>().selectedCharacter["P" + player] = -1;
      }
      else if (Input.GetButtonDown("Controller " + player + " B Button")
        || Input.GetButtonDown("Controller " + player + " Back Button"))
      {
        SceneManager.LoadScene("_scene_Menu");
      }
    }
    else if (!confirmed)
    {
      if (Input.GetButtonDown("Controller " + player + " Right Bumper")
        || (!axisInUse && Input.GetAxis("Controller " + player + " Left Stick X Axis") >= 0.95f))
      {
        // change to next character
        axisInUse = true;
        ChangeCharacter(1);
      }
      else if (Input.GetButtonDown("Controller " + player + " Left Bumper")
        || (!axisInUse && Input.GetAxis("Controller " + player + " Left Stick X Axis") <= -0.95f))
      {
        // change to previous character
        axisInUse = true;
        ChangeCharacter(-1);
      }
      else if (Input.GetButtonDown("Controller " + player + " Start Button")
        || Input.GetButtonDown("Controller " + player + " A Button"))
      {
        // hide character select UI and let player move around
        if (!UI.GetComponent<CharacterSelectUI>().characterIsSelected("P"+player,prefabNumber))
        {
          confirmed = true;
          ShowCharacterSelectUI(false);
          UnlockCharacter(true);
        }
      }
      else if (Input.GetButtonDown("Controller " + player + " Back Button") 
        || Input.GetButtonDown("Controller " + player + " B Button"))
      {
        // return to unjoined UI
        joined = false;
        ShowJoinUI(true);
        ShowCharacterSelectUI(false);
        ShowPlayer(false);
        UI.GetComponent<CharacterSelectUI>().selectedCharacter["P" + player] = -2;
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
        UI.GetComponent<CharacterSelectUI>().selectedCharacter["P" + player] = -1;
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
    leftBumper.SetActive(value);
    rightBumper.SetActive(value);

  }

  // toggles Player
  void ShowPlayer(bool value)
  {
    if (value)
    {
      playerObject = Instantiate(playerPrefabs[prefabNumber]) as GameObject;
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
  void ChangeCharacter(int value)
  {
    Destroy(playerObject);
    prefabNumber += value;

    if (prefabNumber >= playerPrefabs.Capacity)
    {
      prefabNumber -= playerPrefabs.Capacity;
    }
    else if (prefabNumber < 0)
    {
      prefabNumber += playerPrefabs.Capacity;
    }

    ShowPlayer(true);
  }

  // disables the colliders locking the player into the character select
  void UnlockCharacter(bool value)
  {
    if (value)
    {
      backTochange.SetActive(true);
      UI.GetComponent<CharacterSelectUI>().selectedCharacter["P" + player] = prefabNumber;
      playerStand.GetComponent<BoxCollider2D>().enabled = false;
      playerObject.GetComponent<player>().player_number = player;
    }
    else
    {
      backTochange.SetActive(false);
      UI.GetComponent<CharacterSelectUI>().selectedCharacter["P" + player] = -1;
      ShowPlayer(false);
      playerStand.GetComponent<BoxCollider2D>().enabled = true;
      ShowPlayer(true);
    }
  }
}
