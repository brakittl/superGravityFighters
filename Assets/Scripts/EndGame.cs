using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {
  List<string> standing = new List<string>();
  List<GameObject> playerPrefabs = new List<GameObject>();
  List<player> playerList = new List<player>();
  int readyCount;
  List<bool> readyPlayers = new List<bool>() { false, false, false, false };

  public string pathToPlayers = "__Prefabs/_players/";
  public bool testMode;
  public List<Vector3> positions = new List<Vector3>() { new Vector3(-1.69f, 5f, 0f), new Vector3(-0.35f, 5f, 0f), new Vector3(1.03f, 5f, 0f), new Vector3(2.44f, 5f, 0f)};
  public List<string> availableStats = new List<string>();

  Dictionary<int, Dictionary<string, int>> statistics = new Dictionary<int, Dictionary<string, int>>();

  // Use this for initialization
  void Start() {
    readyCount = 0;

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
      playerPrefabs.Add(Instantiate(Resources.Load(pathToPlayers + standing[i], typeof(GameObject))) as GameObject);
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
    // take each player's ready input once
    if (!readyPlayers[0] && Input.GetButtonDown("Controller 1 A Button"))
    {
      readyPlayers[0] = true;
      readyCount++;

      setReady(0);
    }
    if (!readyPlayers[1] && Input.GetButtonDown("Controller 2 A Button"))
    {
      readyPlayers[1] = true;
      readyCount++;

      setReady(1);
    }
    if (!readyPlayers[2] && Input.GetButtonDown("Controller 3 A Button"))
    {
      readyPlayers[2] = true;
      readyCount++;
      setReady(2);
    }
    if (!readyPlayers[3] && Input.GetButtonDown("Controller 4 A Button"))
    {
      readyPlayers[3] = true;
      readyCount++;
      setReady(3);
    }

    // check if all available players are ready
    if (readyCount >= standing.Capacity)
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
        // Skydiver: longest time without touching the ground
        // Teleporter: most border swaps
        // Cautious: least border swaps

        // Airborne: most time in air
        // Grounded: least time in air
        // Greed: most bullets picked up
        // Poverty: least bullets picked up
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

  // change "press A to confirm" to "Ready"
  void setReady(int standing)
  {
    Transform placeOb = transform.FindChild(standing + "place");
    placeOb.FindChild("readyText").GetComponent<Text>().text = "READY!";
    placeOb.FindChild("readyText").GetComponent<Text>().fontSize = 20;
    placeOb.FindChild("AButton").GetComponent<Image>().enabled = false;
  }


}
