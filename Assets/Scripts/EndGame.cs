using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {
  List<string> standing = new List<string>();
  List<GameObject> playerPrefabs = new List<GameObject>();
  List<player> playerList = new List<player>();

  public bool testMode;
  public List<Vector3> positions = new List<Vector3>();
  public List<string> availableStats = new List<string>();

  Dictionary<int, Dictionary<string, int>> statistics = new Dictionary<int, Dictionary<string, int>>();

  // Use this for initialization
  void Start() {
    if (testMode)
    {
      standing.Add("blue_player");
      standing.Add("red_player");
      standing.Add("yellow_player");
      standing.Add("green_player");
    }
    else
    {
      if (PlayerPrefs.GetString("first") != "none")
      {
        standing.Add(PlayerPrefs.GetString("first"));
      }
      if (PlayerPrefs.GetString("second") != "none")
      {
        standing.Add(PlayerPrefs.GetString("second"));
      }
      if (PlayerPrefs.GetString("third") != "none")
      {
        standing.Add(PlayerPrefs.GetString("third"));
      }
      if (PlayerPrefs.GetString("fourth") != "none")
      {
        standing.Add(PlayerPrefs.GetString("fourth"));
      }
    }

    for (int i = 0; i < standing.Capacity; i++)
    {
      playerPrefabs.Add(Instantiate(Resources.Load("__Prefabs/" + standing[i], typeof(GameObject))) as GameObject);
      playerPrefabs[i].transform.position = positions[i];
      playerList.Add(playerPrefabs[i].GetComponent<player>());
      playerList[i].enabled = false;
    }

    GetStats();

    CalculateAwards();

    DisplayAwards();
  }

  // Update is called once per frame
  void Update() {
    // return to mapselect/mainmenu
    if (Input.GetButtonDown("Controller 1 Start Button")
      || Input.GetButtonDown("Controller 2 Start Button")
      || Input.GetButtonDown("Controller 3 Start Button")
      || Input.GetButtonDown("Controller 4 Start Button"))
    {
      //SceneManager.LoadScene("_scene_MapSelect");
      SceneManager.LoadScene("_scene_Menu");
    }
  }

  void GetStats()
  {
    for (int i = 0; i < playerList.Capacity; i++)
    {
      // add player
      statistics.Add(i, new Dictionary<string, int>());

      for (int j = 0; j < availableStats.Capacity; j++)
      {
        // add individual statistic for a player
        // getting stats as "1gravityCount" for the player in first not P1
        statistics[i].Add((1 + i) + availableStats[j], PlayerPrefs.GetInt((1 + i) + availableStats[j]));
      }
    }
  }

  // calculate the winner of each award
  void CalculateAwards()
  {
    // Airborne: most time in air
    // Grounded: least time in air
    // Skydiver: longest time without touching the ground
    // Greed: most bullets picked up
    // Poverty: least bullets picked up
    // Teleporter: most border swaps
    // Cautious: least border swaps
    // Astronaut: most gravity swaps
    // Steady: least gravity swaps
    // Survivor: longest life
    // Reckless: shortest life
    // Lich King: most poisons
    // Samurai: sword accuracy
    // Sniper: bullet accuracy
    // Assassin: most sword kills
    // Gunslinger: most bullet kills
    // Pacifist: least kills
    // Athlete: most distance traveled
    // Camper: least distance traveled
    // Whoops: most suicides
    // Participant: had fun (couldn't find an award for them)
  }

  // display each player's awards
  void DisplayAwards()
  {

  }
}
