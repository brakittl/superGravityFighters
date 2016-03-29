using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour {
  GameObject ready;

  public List<string> playerPrefabNames = new List<string>()
  {
    "black_player", "blue_player", "green_player", "orange_player", "purple_player", "red_player", "yellow_player"
  };

  public Dictionary<string, int> selectedCharacter = new Dictionary<string, int>()
  {
    {"P1", -2},
    {"P2", -2},
    {"P3", -2},
    {"P4", -2}
  };

	// Use this for initialization
	void Start () {
    ready = GameObject.Find("Ready");
    if (ready != null)
    {
      ready.SetActive(false);
    }
	}

  public bool characterIsSelected(string player,int value)
  {
    if (selectedCharacter[player] == value) return false; // false because current player has it selected
    for (int i = 1; i <= 4; i++)
    {
      if (("P"+ i) != player)
      {
        if (selectedCharacter["P" + i] == value)
        {
          return true;
        }
      }
    }
    return false;
  }

  void Update()
  {
    ready.SetActive(allPlayersReady());

    if (allPlayersReady())
    {
      if (Input.GetButtonDown("Controller 1 Start Button")
      || Input.GetButtonDown("Controller 2 Start Button")
      || Input.GetButtonDown("Controller 3 Start Button")
      || Input.GetButtonDown("Controller 4 Start Button"))
      {
        for (int i = 1; i <= 4; i++)
        {
          if (selectedCharacter["P"+i] >= 0)
          {
            PlayerPrefs.SetString("P" + i, playerPrefabNames[selectedCharacter["P" + i]]);
          }
          else
          {
            PlayerPrefs.SetString("P" + i, "none");
          }
        }

        SceneManager.LoadScene("_BK");
      }
    }    
  }

  bool allPlayersReady()
  {
    int readyCount = 0;
    for (int i = 1; i <= 4; i++)
    {
      if (selectedCharacter["P"+ i] == -1)
      {
        return false;
      }
      if (selectedCharacter["P" + i] >= 0) readyCount++;
    }

    if (readyCount > 1)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
}
