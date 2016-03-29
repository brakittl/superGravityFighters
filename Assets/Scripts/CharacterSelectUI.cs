using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour {
  GameObject ready;

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
        PlayerPrefs.SetInt("P1", selectedCharacter["P1"]);
        PlayerPrefs.SetInt("P2", selectedCharacter["P2"]);
        PlayerPrefs.SetInt("P3", selectedCharacter["P3"]);
        PlayerPrefs.SetInt("P4", selectedCharacter["P4"]);

        SceneManager.LoadScene("2");
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

    if (readyCount > 0)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
}
