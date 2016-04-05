using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode{ NONE, SURVIVAL, DEATHMATCH, REVERSE_TAG };

public class Level : MonoBehaviour{

  public static Level S;
    public GameMode gamemode; // You can use this for selecting the GameMode right from the Map Screen
  public int rt_point_limit = 100;

  public GameObject player1, player2, player3, player4;
  public GameObject first, second, third, fourth;
  public bool player1Ready, player2Ready, player3Ready, player4Ready;

  public Vector3[] respawnPoints;
  public List<player> ranking = new List<player>();
  int numPlayers;
  Texture2D black = new Texture2D(1, 1);

  public bool gameOver;
  public int readyCount;

  public Sprite gold, silver, bronze;
  public List<Vector3> podiumPositions = new List<Vector3>() { new Vector3(-1.69f, 5f, 0f), new Vector3(-0.35f, 5f, 0f), new Vector3(1.03f, 5f, 0f), new Vector3(2.44f, 5f, 0f) };

  public Dictionary<string, string> medals = new Dictionary<string, string>()
  {
    { "SkyDiver","longest consecutive airtime" },
    { "Teleporter","most border swaps" },
    { "Cautious","least border swaps" },
    { "Airborne","most time in air" },
    { "Grounded","least airtime" },
    { "Greed","most bullets picked up" },
    { "Poverty","least bullets picked up" },
    { "Astronaut","most gravity swaps" },
    { "Steady","least gravity swaps" },
    { "Survivor","longest life" },
    { "Reckless","shortest life" },
    { "Lich King","most poisons" },
    { "Samurai","best sword accuracy" },
    { "Sniper","best bullet accuracy" },
    { "Assassin","most sword kills" },
    { "Gunslinger","most gun kills" },
    { "Pacifist","least kills" },
    { "Athlete","most distance traveled" },
    { "Camper","least distance traveled" },
    { "Whoops","most suicides" },
    { "Participation","you had fun :)" },
  };

  void Start(){

    // ==[game mode]============================================================
    // =========================================================================

    S = this;

    gameOver = false;
    player1Ready = player2Ready = player3Ready = player4Ready = false;

		if(gamemode == GameMode.NONE){ 
			if(PlayerPrefs.GetString("GameMode") == "SURVIVAL"){
				gamemode = GameMode.SURVIVAL;
			}
			else if(PlayerPrefs.GetString("GameMode") == "DEATHMATCH"){
				gamemode = GameMode.DEATHMATCH;
			}
			else if(PlayerPrefs.GetString("GameMode") == "REVERSE_TAG"){
				gamemode = GameMode.REVERSE_TAG;
			} 
			else Debug.LogError("INCORRECT GAMEMODE STRING PASSED IN FROM PLAYERPREFS. CHECK LEVEL SCRIPT");
		}
		else{
			Debug.Log("GAMEMODE ALREADY SET FROM LEVEL SCRIPT. CURRENT GAMEMODE: " + gamemode);
			Debug.Log("TO LET GAMEMODE BE SET THRU LEVEL SELECT, MAKE SURE LEVEL SCRIPT'S GAMEMODE IS NONE");
		}

    // ==[players]============================================================
    // =========================================================================

    //PlayerPrefs.SetString("P1", "orange_player");
    //PlayerPrefs.SetString("P2", "none");
    //PlayerPrefs.SetString("P3", "red_player");
    //PlayerPrefs.SetString("P4", "none");

    PlayerPrefs.SetString("Player Name", "Foobar");

    Vector3 rot = new Vector3(0, 0, 0);

    // // p1
    // numPlayers = 0;
    // if(PlayerPrefs.GetString("P1") != "none"){
    //   player1 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P1")), returnPosition(0), Quaternion.Euler(rot)) as GameObject;
    //   player1.SendMessage("SetPlayerNumber", 1);
    //   numPlayers++;
    // }
    // // p2
    // if(PlayerPrefs.GetString("P2") != "none"){
    //   player2 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P2")), returnPosition(1), Quaternion.Euler(rot)) as GameObject;
    //   player2.SendMessage("SetPlayerNumber", 2);
    //   numPlayers++;
    // }
    // // p3
    // if(PlayerPrefs.GetString("P3") != "none"){
    //   player3 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P3")), returnPosition(2), Quaternion.Euler(rot)) as GameObject;
    //   player3.SendMessage("SetPlayerNumber", 3);
    //   numPlayers++;
    // }
    // // p4
    // if(PlayerPrefs.GetString("P4") != "none"){
    //   player4 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P4")), returnPosition(3), Quaternion.Euler(rot)) as GameObject;
    //   player4.SendMessage("SetPlayerNumber", 4);
    //   numPlayers++;
    // }

  }

  void Update()
  {
    if (gameOver)
    {
      if (!player1Ready && Input.GetButtonDown("Controller 1 A Button"))
      {
        player1Ready = true;
        SetReady(1);
        readyCount++;
      }
      if (!player2Ready && Input.GetButtonDown("Controller 2 A Button"))
      {
        player2Ready = true;
        SetReady(2);
        readyCount++;
      }
      if (!player3Ready && Input.GetButtonDown("Controller 3 A Button"))
      {
        player3Ready = true;
        SetReady(3);
        readyCount++;
      }
      if (!player4Ready && Input.GetButtonDown("Controller 4 A Button"))
      {
        player4Ready = true;
        SetReady(4);
        readyCount++;
      }

      // check if all available players are ready
      if (readyCount >= numPlayers)
      {
        //SceneManager.LoadScene("_scene_MapSelect");
        SceneManager.LoadScene("_scene_Menu");
      }
    }
    else
    {
      if (gamemode == GameMode.SURVIVAL)
      {
        // check number of dead players
        int deadCount = 0;
        foreach (player p in ranking)
          deadCount++;

        if (numPlayers - deadCount <= 1)
        {
          // one player left, end the game
          AwardMedals();
          gameOver = true;
        }
      }
      else if (gamemode == GameMode.DEATHMATCH)
      {
        if (ranking.Capacity > 0)
        {
          // killcount reached, end the game
          List<player> activePlayers = new List<player>();
          if (PlayerPrefs.GetString("P1") != "none")
          {
            activePlayers.Add(player1.GetComponent<player>());
          }
          if (PlayerPrefs.GetString("P2") != "none")
          {
            activePlayers.Add(player2.GetComponent<player>());
          }
          if (PlayerPrefs.GetString("P3") != "none")
          {
            activePlayers.Add(player3.GetComponent<player>());
          }
          if (PlayerPrefs.GetString("P4") != "none")
          {
            activePlayers.Add(player4.GetComponent<player>());
          }

          // sort by kills, deaths for ties
          for (int i = 0; i < activePlayers.Capacity; i++)
          {
            for (int j = i + 1; j < activePlayers.Capacity; j++)
            {
              if (activePlayers[i].playersKilled.Capacity < activePlayers[j].playersKilled.Capacity)
              {
                player temp = activePlayers[i];
                activePlayers[i] = activePlayers[j];
                activePlayers[j] = temp;
              }
              else if (activePlayers[i].playersKilled.Capacity == activePlayers[j].playersKilled.Capacity)
              {
                int deaths1 = 10 - activePlayers[i].lives;
                int deaths2 = 10 - activePlayers[j].lives;
                if (deaths2 < deaths1)
                {
                  player temp = activePlayers[i];
                  activePlayers[i] = activePlayers[j];
                  activePlayers[j] = temp;
                }
              }
            }
          }

          first = activePlayers.Capacity >= 1 ? activePlayers[0].gameObject : null;
          second = activePlayers.Capacity >= 2 ? activePlayers[1].gameObject : null;
          third = activePlayers.Capacity >= 3 ? activePlayers[2].gameObject : null;
          fourth = activePlayers.Capacity >= 4 ? activePlayers[3].gameObject : null;

          AwardMedals();

          DisplayResults();

          foreach (player p in activePlayers)
          {
            p.gameOver = true;
          }
          gameOver = true;
        }
      }
      else if (gamemode == GameMode.REVERSE_TAG)
      {
        if (ranking.Capacity > 0)
        {
          // point limit reached, end the game
          AwardMedals();
          gameOver = true;
        }
      }
    }
  }

  public Vector3 returnPosition(int i){
    return respawnPoints[i];
  }

  public GameObject blackSquare;
  GameObject topWall, bottomWall, leftWall, rightWall;
  public bool pause = false, running = false;
  int runAgain = 0;
  public List<Vector3> positions = new List<Vector3>();

  public void KillPause(Vector3 playerPos){
    if(!running){
      running = true;
      StartCoroutine(Pause(playerPos));
    }
  }
    
  IEnumerator Pause(Vector3 pos){
    //yield return new WaitForSeconds(0.05f);
    Time.timeScale = 0.1f;
    Vector3 cubePos = pos, rot = transform.rotation.eulerAngles;
        print(cubePos);
    cubePos.y = pos.y - 3f;
        print(cubePos);
    bottomWall = Instantiate(blackSquare, cubePos, transform.rotation) as GameObject;
    cubePos.y = pos.y + 3f;
    topWall = Instantiate(blackSquare, cubePos, transform.rotation) as GameObject;
    cubePos.x = pos.x - 3f;
    rot.z = 90;
    leftWall = Instantiate(blackSquare, cubePos, Quaternion.Euler(rot)) as GameObject;
    cubePos.x = pos.x + 3f;
    rightWall = Instantiate(blackSquare, cubePos, Quaternion.Euler(rot)) as GameObject;
    
    yield return new WaitForSeconds(0.03f);
    Destroy(topWall);
    Destroy(bottomWall);
    Destroy(leftWall);
    Destroy(rightWall);
    Time.timeScale = 1;

    running = false;
  }

  public Vector3 findRespawn(){
    
    Vector3 respawnPoint = new Vector3(0, 0, 0);
    float closestPlayerDist = 10000f;
    float closest = 0;
    GameObject closestP = null;
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    
    foreach(Vector3 point in respawnPoints){

      // find closest player to this point   
      closestP = null;
      foreach(GameObject player in players){
        if((Mathf.Abs(Vector3.Distance(point, player.transform.position)) < closestPlayerDist) || (closestP == null)){
          closestPlayerDist = Mathf.Abs(Vector3.Distance(point, player.transform.position));
          closestP = player;
        }
      }

      // if the closest player is further than the closest so far set that as respawn
      if(Mathf.Abs(Vector3.Distance(point, closestP.transform.position)) > closest){
        respawnPoint = point;
        closest = Mathf.Abs(Vector3.Distance(point, closestP.transform.position));
      }
    }

    return respawnPoint;
  }

  void AwardMedals()
  {
    List<player> activePlayers = new List<player>();

    if (PlayerPrefs.GetString("P1") != "none")
    {
      activePlayers.Add(player1.GetComponent<player>());
    }
    if (PlayerPrefs.GetString("P2") != "none")
    {
      activePlayers.Add(player2.GetComponent<player>());
    }
    if (PlayerPrefs.GetString("P3") != "none")
    {
      activePlayers.Add(player3.GetComponent<player>());
    }
    if (PlayerPrefs.GetString("P4") != "none")
    {
      activePlayers.Add(player4.GetComponent<player>());
    }

    int high, low;

    // Skydiver: longest consecutive airtime
    high = 0;
    int longestAirTime = activePlayers[high].longestAirTime;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].longestAirTime;
      if (current > longestAirTime)
      {
        high = i;
        longestAirTime = current;
      }
    }
    activePlayers[high].medals.Add("Skydiver");

    // Teleporter: most border swaps
    // Cautious: least border swap
    high = 0;
    low = 0;
    int mostBorderSwaps = activePlayers[high].borderSwaps;
    int leastBorderSwaps = mostBorderSwaps;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].borderSwaps;
      if (current > mostBorderSwaps)
      {
        high = i;
        mostBorderSwaps = current;
      }
      else if (current < leastBorderSwaps)
      {
        low = i;
        leastBorderSwaps = current;
      }
    }
    activePlayers[high].medals.Add("Teleporter");
    activePlayers[low].medals.Add("Cautious");

    // Airborne: most time in air
    // Grounded: least time in air
    high = 0;
    low = 0;
    int mostAirTime = activePlayers[high].airTime;
    int leastAirTime = mostAirTime;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].airTime;
      if (current > mostAirTime)
      {
        high = i;
        mostAirTime = current;
      }
      else if (current < leastAirTime)
      {
        low = i;
        leastAirTime = current;
      }
    }
    activePlayers[high].medals.Add("Airborne");
    activePlayers[low].medals.Add("Grounded");

    // Hoarder: most bullets picked up
    // Poverty: least bullets picked up
    high = 0;
    low = 0;
    int mostBullets = activePlayers[high].bulletPickUps;
    int leastBullets = mostBullets;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].bulletPickUps;
      if (current > mostBullets)
      {
        high = i;
        mostBullets = current;
      }
      else if (current < leastBullets)
      {
        low = i;
        leastBullets = current;
      }
    }
    activePlayers[high].medals.Add("Hoarder");
    activePlayers[low].medals.Add("Poverty");

    // Astronaut: most gravity swaps
    // Steady: least gravity swaps
    high = 0;
    low = 0;
    int mostGravitySwaps = activePlayers[high].gravitySwapCount;
    int leastGravitySwaps = mostGravitySwaps;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].gravitySwapCount;
      if (current > mostGravitySwaps)
      {
        high = i;
        mostGravitySwaps = current;
      }
      else if (current < leastGravitySwaps)
      {
        low = i;
        leastGravitySwaps = current;
      }
    }
    activePlayers[high].medals.Add("Astronaut");
    activePlayers[low].medals.Add("Steady");

    // Survivor: longest life
    high = 0;
    int longestLife = activePlayers[high].longestLife;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].longestLife;
      if (current > longestLife)
      {
        high = i;
        longestLife = current;
      }
    }
    activePlayers[high].medals.Add("Survivor");

    // Reckless: shortest life
    low = 0;
    int shortestLife = activePlayers[high].shortestLife;
    for (int i = low + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].shortestLife;
      if (current < shortestLife)
      {
        low = i;
        shortestLife = current;
      }
    }
    activePlayers[low].medals.Add("Reckless");

    // Lich King: most poisons
    high = 0;
    int mostPoisons = activePlayers[high].totalPoisoned;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].totalPoisoned;
      if (current > mostPoisons)
      {
        high = i;
        mostPoisons = current;
      }
    }
    activePlayers[high].medals.Add("Lich King");

    // Samurai: best sword accuracy
    high = 0;
    float bestSwordAccuracy = (float)(activePlayers[high].numSwordHits) / (activePlayers[high].numSwordSwipes);
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      float current = (float)(activePlayers[i].numSwordHits) / (activePlayers[i].numSwordSwipes);
      if (current > bestSwordAccuracy)
      {
        high = i;
        bestSwordAccuracy = current;
      }
    }
    activePlayers[high].medals.Add("Samurai");

    // Sniper: bullet accuracy
    high = 0;
    float bestBulletAccuracy = (float)(activePlayers[high].numBulletHits) / (activePlayers[high].numBulletShots);
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      float current = (float)(activePlayers[i].numBulletHits) / (activePlayers[i].numBulletShots);
      if (current > bestBulletAccuracy)
      {
        high = i;
        bestBulletAccuracy = current;
      }
    }
    activePlayers[high].medals.Add("Sniper");

    // Assassin: most sword kills
    high = 0;
    int mostSwordKills = activePlayers[high].numSwordHits;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].numSwordHits;
      if (current > mostSwordKills)
      {
        high = i;
        mostSwordKills = current;
      }
    }
    activePlayers[high].medals.Add("Assassin");

    // Gunslinger: most bullet kills
    high = 0;
    int mostBulletKills = activePlayers[high].numBulletHits;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].numBulletHits;
      if (current > mostBulletKills)
      {
        high = i;
        mostBulletKills = current;
      }
    }
    activePlayers[high].medals.Add("Gunslinger");

    // Pacifist: least kills
    low = 0;
    int leastKills = activePlayers[low].numBulletHits + activePlayers[low].numSwordHits;
    for (int i = low + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].numBulletHits + activePlayers[low].numSwordHits;
      if (current < leastKills)
      {
        low = i;
        leastKills = current;
      }
    }
    activePlayers[low].medals.Add("Pacifist");

    // Athlete: most distance traveled
    // Camper: least distance traveled
    high = 0;
    low = 0;
    int mostDistance = activePlayers[high].steps;
    int leastDistance = mostDistance;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].steps;
      if (current > mostDistance)
      {
        high = i;
        mostDistance = current;
      }
      else if (current < leastDistance)
      {
        low = i;
        leastDistance = current;
      }
    }
    activePlayers[high].medals.Add("Athlete");
    activePlayers[low].medals.Add("Camper");

    // Whoops: most suicides
    high = 0;
    int mostSuicides = activePlayers[high].suicides;
    for (int i = high + 1; i < activePlayers.Capacity; i++)
    {
      int current = activePlayers[i].suicides;
      if (current > mostSuicides)
      {
        high = i;
        mostSuicides = current;
      }
    }
    activePlayers[high].medals.Add("Whoops");

    // Participant: had fun (couldn't find an award for them)
    for (int i = 0; i < activePlayers.Capacity; i++)
    {
      if (activePlayers[i].medals.Capacity == 0)
      {
        activePlayers[i].medals.Add("Participant");
      }
    }
  }

  void DisplayResults()
  {
    // show the end game UI
    Transform place1, place2, place3, place4;

    place1 = transform.Find("1place");
    place2 = transform.Find("2place");
    place3 = transform.Find("3place");
    place4 = transform.Find("4place");

    if (first != null)
    {
      place1.FindChild("trophy").GetComponent<Image>().sprite = gold;
      first.transform.position = podiumPositions[0];
      setResultsUI(place1, first.GetComponent<player>(), first.GetComponent<player>().medals);
    }
    else
    {
      place1.gameObject.SetActive(false);
    }

    if (second != null)
    {
      place2.FindChild("trophy").GetComponent<Image>().sprite = silver;
      second.transform.position = podiumPositions[1];
      setResultsUI(place2, second.GetComponent<player>(), second.GetComponent<player>().medals);
    }
    else
    {
      place2.gameObject.SetActive(false);
    }

    if (third != null)
    {
      place3.FindChild("trophy").GetComponent<Image>().sprite = bronze;
      third.transform.position = podiumPositions[2];
      setResultsUI(place3, third.GetComponent<player>(), third.GetComponent<player>().medals);
    }
    else
    {
      place3.gameObject.SetActive(false);
    }

    if (fourth != null)
    {
      place4.FindChild("trophy").GetComponent<Image>().enabled = false;
      fourth.transform.position = podiumPositions[3];
      setResultsUI(place4, fourth.GetComponent<player>(), fourth.GetComponent<player>().medals);
    }
    else
    {
      place4.gameObject.SetActive(false);
    }
  }

  void setResultsUI(Transform place, player p, List<string> earnedMedals)
  {
    string medal1 = "", medal2 = "", medal3 = "";
    string info1 = "", info2 = "", info3 = "";

    if (gamemode == GameMode.DEATHMATCH || gamemode == GameMode.SURVIVAL)
    {
      place.FindChild("killsText").GetComponent<Text>().text = "Kills: " + p.playersKilled.Capacity;
      place.FindChild("deathsText").GetComponent<Text>().text = "Deaths: " + (10 - p.lives);
    }
    if (gamemode == GameMode.REVERSE_TAG)
    {
      place.FindChild("killsText").GetComponent<Text>().text = "Points: " + p.rt_points;
      place.FindChild("deathsText").GetComponent<Text>().text = "";
    }

    if (earnedMedals.Capacity != 0)
    {
      medal1 = earnedMedals[Random.Range(0, earnedMedals.Capacity)];
      earnedMedals.Remove(medal1);
      info1 = medals[medal1];
      medal1 += ":";
      
    }
    if (earnedMedals.Capacity != 0)
    {
      medal2 = earnedMedals[Random.Range(0, earnedMedals.Capacity)];
      earnedMedals.Remove(medal2);
      info2 = medals[medal2];
      medal2 += ":";
    }
    if (earnedMedals.Capacity != 0)
    {
      medal3 = earnedMedals[Random.Range(0, earnedMedals.Capacity)];
      earnedMedals.Remove(medal3);
      info3 = medals[medal3];
      medal3 += ":";
    }

    place.FindChild("medal1").GetComponent<Text>().text = medal1;
    place.FindChild("info1").GetComponent<Text>().text = info1;
    place.FindChild("medal2").GetComponent<Text>().text = medal2;
    place.FindChild("info2").GetComponent<Text>().text = info2;
    place.FindChild("medal3").GetComponent<Text>().text = medal3;
    place.FindChild("info3").GetComponent<Text>().text = info3;
  }

  void SetReady(int controller)
  {
    if (first.GetComponent<player>().player_number == controller)
    {
      transform.Find("1place").FindChild("AButton").GetComponent<Image>().enabled = false;
      transform.Find("1place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
    else if (second.GetComponent<player>().player_number == controller)
    {
      transform.Find("2place").FindChild("AButton").GetComponent<Image>().enabled = false;
      transform.Find("2place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
    else if (third.GetComponent<player>().player_number == controller)
    {
      transform.Find("3place").FindChild("AButton").GetComponent<Image>().enabled = false;
      transform.Find("3place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
    else if (fourth.GetComponent<player>().player_number == controller)
    {
      transform.Find("4place").FindChild("AButton").GetComponent<Image>().enabled = false;
      transform.Find("4place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
  }
}
