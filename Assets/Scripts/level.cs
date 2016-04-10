using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode{ NONE, SURVIVAL, DEATHMATCH, REVERSE_TAG };

public class level : MonoBehaviour {

  public bool isMap;
  public GameObject podium;

  public static level S;
  public GameMode gamemode; // You can use this for selecting the GameMode right from the Map Screen
  public int rt_point_limit = 50;

  public GameObject player1, player2, player3, player4;
  public GameObject first, second, third, fourth;
  public bool player1Ready, player2Ready, player3Ready, player4Ready;

  public Vector3[] respawnPoints;
  public List<player> ranking = new List<player>();
  int numPlayers;
  // Texture2D black = new Texture2D(1, 1);

  public bool gameOver;
  public int readyCount;
  public GameObject PostGameOb;
  public Canvas PostGameUI;

  public GameObject gravity_stone;
  public Vector3 stone_position;

  public Sprite gold, silver, bronze;
  public List<Vector3> podiumPositions = new List<Vector3>(){ new Vector3(-1.48f, 5f, 0f), new Vector3(-0.35f, 10f, 0f), new Vector3(0.98f, 15f, 0f), new Vector3(1.82f, 20f, 0f) };

  public Dictionary<string, string> medals = new Dictionary<string, string>(){
    { "SKY DIVER","longest consecutive airtime" },
    { "TELEPORTER","most border teleports" },
    { "CAUTIOUS","least border teleports" },
    { "AIRBORNE","most time in air" },
    { "GROUNDED","least airtime" },
    { "HOARDER","most bullets picked up" },
    { "POVERTY","least bullets picked up" },
    { "ASTRONAUT","most gravity swaps" },
    { "STEADY","least gravity swaps" },
    { "SURVIVOR","longest life" },
    { "RECKLESS","shortest life" },
    { "LICH KING","most poisons" },
    { "SAMURAI","best sword accuracy" },
    { "SNIPER","best bullet accuracy" },
    { "ASSASSIN","most sword kills" },
    { "GUNSLINGER","most gun kills" },
    { "PACIFIST","least kills" },
    { "ATHLETE","most distance traveled" },
    { "CAMPER","least distance traveled" },
    { "WHOOPS","most suicides" },
    { "PARTICIPANT","you had fun :)" },
    { "THUG LIFE","most steals" },
    { "JUST A GOOD PERSON","least steals" },
    { "IRON GRIP","longest single possession" },
    { "BUTTERFINGERS","shortest single possession" }
  };

  void Start(){
    if(SceneManager.GetActiveScene().name == "_city"
      || SceneManager.GetActiveScene().name == "_forest"
      || SceneManager.GetActiveScene().name == "_galaxy"
      || SceneManager.GetActiveScene().name == "_desert"
      || SceneManager.GetActiveScene().name == "_volcano"){
      isMap = true;
    }
    else{
      isMap = false;
    }

    // ==[game mode]============================================================
    // =========================================================================

    S = this;

    if(isMap){
      podium = GameObject.Find("Podium");
      if (podium != null) podium.SetActive(false);
      PostGameOb = GameObject.Find("PostGameCanvas");
      if (PostGameOb != null) PostGameOb.SetActive(false);
      //PostGameUI = GameObject.Find("PostGameCanvas").GetComponent<Canvas>();
      //if(PostGameUI != null) PostGameUI.enabled = false;

      gameOver = false;
      player1Ready = player2Ready = player3Ready = player4Ready = false;
    }

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
			else Debug.Log("INCORRECT GAMEMODE STRING PASSED IN FROM PLAYERPREFS. CHECK level SCRIPT");
		}
		else{
			// Debug.Log("GAMEMODE ALREADY SET FROM level SCRIPT. CURRENT GAMEMODE: " + gamemode);
			// Debug.Log("TO LET GAMEMODE BE SET THRU level SELECT, MAKE SURE level SCRIPT'S GAMEMODE IS NONE");
		}

    // ==[players]============================================================
    // =========================================================================


    // PlayerPrefs.SetString("P1", "orange_player");
    // PlayerPrefs.SetString("P2", "none");
    // PlayerPrefs.SetString("P3", "none");
    // PlayerPrefs.SetString("P4", "green_player");

    PlayerPrefs.SetString("Player Name", "Foobar");

    Vector3 rot = new Vector3(0, 0, 0);

    if(isMap){

      // p1
      numPlayers = 0;
      if(PlayerPrefs.GetString("P1") != "none"){
        player1 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P1")), returnPosition(0), Quaternion.Euler(rot)) as GameObject;
        player1.SendMessage("SetPlayerNumber", 1);
        numPlayers++;
      }
      // p2
      if(PlayerPrefs.GetString("P2") != "none"){
        player2 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P2")), returnPosition(1), Quaternion.Euler(rot)) as GameObject;
        player2.SendMessage("SetPlayerNumber", 2);
        numPlayers++;
      }
      // p3
      if(PlayerPrefs.GetString("P3") != "none"){
        player3 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P3")), returnPosition(2), Quaternion.Euler(rot)) as GameObject;
        player3.SendMessage("SetPlayerNumber", 3);
        numPlayers++;
      }
      // p4
      if(PlayerPrefs.GetString("P4") != "none"){
        player4 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P4")), returnPosition(3), Quaternion.Euler(rot)) as GameObject;
        player4.SendMessage("SetPlayerNumber", 4);
        numPlayers++;
      }
    }

    if(isMap && gamemode == GameMode.REVERSE_TAG){
      GameObject gravity_stone_instance;
      gravity_stone_instance = Instantiate(gravity_stone, stone_position, Quaternion.Euler(rot)) as GameObject;
      Debug.Log(gravity_stone_instance);
    }


  }

  void Update(){
    // disable all control if current scene is not a map
    if(!isMap){
      return;
    }

    if(gameOver){
      if(!player1Ready && Input.GetButtonDown("Controller 1 A Button")){
        player1Ready = true;
        SetReady(1);
        readyCount++;
      }
      if(!player2Ready && Input.GetButtonDown("Controller 2 A Button")){
        player2Ready = true;
        SetReady(2);
        readyCount++;
      }
      if(!player3Ready && Input.GetButtonDown("Controller 3 A Button")){
        player3Ready = true;
        SetReady(3);
        readyCount++;
      }
      if(!player4Ready && Input.GetButtonDown("Controller 4 A Button")){
        player4Ready = true;
        SetReady(4);
        readyCount++;
      }

      // check if all available players are ready
      if(readyCount >= numPlayers){
        Debug.Log("ReadyCount: " + readyCount);
        Debug.Log("numPlayers: " + numPlayers);
        //SceneManager.LoadScene("_scene_MapSelect");
        //SceneManager.LoadScene("_scene_Menu");
        SceneManager.LoadScene("_map_game_select");
      }
    }
    else{
      if(gamemode == GameMode.SURVIVAL){
        // check number of dead players
        int deadCount = 0;
        foreach (player p in ranking)
        {
          Debug.Log(p);
          deadCount++;
        }

        if(numPlayers - deadCount <= 1){
          // one player left, end the game
          List<player> activePlayers = new List<player>();
          if(PlayerPrefs.GetString("P1") != "none"){
            activePlayers.Add(player1.GetComponent<player>());
          }
          if(PlayerPrefs.GetString("P2") != "none"){
            activePlayers.Add(player2.GetComponent<player>());
          }
          if(PlayerPrefs.GetString("P3") != "none"){
            activePlayers.Add(player3.GetComponent<player>());
          }
          if(PlayerPrefs.GetString("P4") != "none"){
            Debug.Log(PlayerPrefs.GetString("P4"));
            activePlayers.Add(player4.GetComponent<player>());
          }

          // sort by deathTime
          for (int i = 0; i < numPlayers; i++){
            for (int j = i + 1; j < numPlayers; j++){
              if(activePlayers[i].deathTime < activePlayers[j].deathTime){
                player temp = activePlayers[i];
                activePlayers[i] = activePlayers[j];
                activePlayers[j] = temp;
              }
            }
          }

          first = numPlayers >= 1 ? activePlayers[numPlayers-1].gameObject : null;
          second = numPlayers >= 2 ? activePlayers[0].gameObject : null;
          third = numPlayers >= 3 ? activePlayers[1].gameObject : null;
          fourth = numPlayers >= 4 ? activePlayers[2].gameObject : null;

          AwardMedals();

          DisplayResults();

          foreach (player p in activePlayers){
            p.gameOver = true;
          }

          GameObject.Find("colliders").SetActive(false);
          podium.SetActive(true);

          gameOver = true;
        }
      }
      else if(gamemode == GameMode.DEATHMATCH){
        if(ranking.Capacity > 0){
          // killcount reached, end the game
          List<player> activePlayers = new List<player>();
          if(PlayerPrefs.GetString("P1") != "none"){
            activePlayers.Add(player1.GetComponent<player>());
          }
          if(PlayerPrefs.GetString("P2") != "none"){
            activePlayers.Add(player2.GetComponent<player>());
          }
          if(PlayerPrefs.GetString("P3") != "none"){
            activePlayers.Add(player3.GetComponent<player>());
          }
          if(PlayerPrefs.GetString("P4") != "none"){
            activePlayers.Add(player4.GetComponent<player>());
          }

          // sort by kills, deaths for ties
          for (int i = 0; i < numPlayers; i++){
            for (int j = i + 1; j < numPlayers; j++){
              if(activePlayers[i].playersKilled.Capacity < activePlayers[j].playersKilled.Capacity){
                player temp = activePlayers[i];
                activePlayers[i] = activePlayers[j];
                activePlayers[j] = temp;
              }
              else if(activePlayers[i].playersKilled.Capacity == activePlayers[j].playersKilled.Capacity){
                int deaths1 = 10 - activePlayers[i].lives;
                int deaths2 = 10 - activePlayers[j].lives;
                if(deaths2 < deaths1){
                  player temp = activePlayers[i];
                  activePlayers[i] = activePlayers[j];
                  activePlayers[j] = temp;
                }
              }
            }
          }

          first = numPlayers >= 1 ? activePlayers[0].gameObject : null;
          second = numPlayers >= 2 ? activePlayers[1].gameObject : null;
          third = numPlayers >= 3 ? activePlayers[2].gameObject : null;
          fourth = numPlayers >= 4 ? activePlayers[3].gameObject : null;

          AwardMedals();

          DisplayResults();

          foreach (player p in activePlayers){
            p.gameOver = true;
          }

          GameObject.Find("colliders").SetActive(false);
          podium.SetActive(true);

          gameOver = true;
        }
      }
      else if(gamemode == GameMode.REVERSE_TAG){
        if(ranking.Count > 0){
          // point limit reached, end the game
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

          // sort by rt_points, ties broken by longest continuous hold
          for (int i = 0; i < numPlayers; i++)
          {
            for (int j = i + 1; j < numPlayers; j++)
            {
              if (activePlayers[i].rt_points < activePlayers[j].rt_points)
              {
                player temp = activePlayers[i];
                activePlayers[i] = activePlayers[j];
                activePlayers[j] = temp;
              }
              else if (activePlayers[i].rt_points == activePlayers[j].rt_points)
              {
                if (activePlayers[i].rt_longest_continuous_hold < activePlayers[j].rt_longest_continuous_hold)
                {
                  player temp = activePlayers[i];
                  activePlayers[i] = activePlayers[j];
                  activePlayers[j] = temp;
                }
              }
            }
          }

          first = numPlayers >= 1 ? activePlayers[0].gameObject : null;
          second = numPlayers >= 2 ? activePlayers[1].gameObject : null;
          third = numPlayers >= 3 ? activePlayers[2].gameObject : null;
          fourth = numPlayers >= 4 ? activePlayers[3].gameObject : null;
          

          AwardMedals();

          DisplayResults();

          foreach (player p in activePlayers)
          {
            p.gameOver = true;
          }

          GameObject.Find("colliders").SetActive(false);
          podium.SetActive(true);

          gameOver = true;
        }
      }
    }
  }

  public Vector3 returnPosition(int i){
    return respawnPoints[i];
  }

  public GameObject killStreak;
  GameObject streak;
  public bool pause = false, running = false;
  // int runAgain = 0;
  //public List<Vector3> positions = new List<Vector3>();

  public void KillPause(Vector3 playerPos){
    if(!running){
      running = true;
      StartCoroutine(Pause(playerPos));
    }
  }
    
  IEnumerator Pause(Vector3 pos){
    Time.timeScale = 0.1f;
    Vector3 rot = transform.rotation.eulerAngles;

        //rot.z = 180 - Mathf.Atan2(killerPos.y - pos.y, killerPos.x - pos.x) * 180 / Mathf.PI;
        rot.z = Random.Range(0, 90);
    streak = Instantiate(killStreak, pos, Quaternion.Euler(rot)) as GameObject;
    
    yield return new WaitForSeconds(0.03f);
    Destroy(streak);
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

  void AwardMedals(){
    List<player> activePlayers = new List<player>();

    if(PlayerPrefs.GetString("P1") != "none"){
      activePlayers.Add(player1.GetComponent<player>());
    }
    if(PlayerPrefs.GetString("P2") != "none"){
      activePlayers.Add(player2.GetComponent<player>());
    }
    if(PlayerPrefs.GetString("P3") != "none"){
      activePlayers.Add(player3.GetComponent<player>());
    }
    if(PlayerPrefs.GetString("P4") != "none"){
      activePlayers.Add(player4.GetComponent<player>());
    }

    if(numPlayers > 0){
      int high, low;
      bool duplicate, lowDuplicate, highDuplicate;

      // Skydiver: longest consecutive airtime
      high = 0; duplicate = false;
      int longestAirTime = activePlayers[high].longestAirTime;
      for (int i = high + 1; i < numPlayers; i++){
        int current = activePlayers[i].longestAirTime;
        if(current > longestAirTime){
          high = i;
          longestAirTime = current;
          duplicate = false;
        }
        else if (current == longestAirTime)
        {
          duplicate = true;
        }
      }
      if (!duplicate) activePlayers[high].medals.Add("SKY DIVER");

      // Teleporter: most border swaps
      // Cautious: least border swap
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostBorderSwaps = activePlayers[high].borderSwaps;
      int leastBorderSwaps = mostBorderSwaps;
      for (int i = high + 1; i < numPlayers; i++){
        int current = activePlayers[i].borderSwaps;
        if (current > mostBorderSwaps)
        {
          high = i;
          mostBorderSwaps = current;
          highDuplicate = false;
        }
        else if (current < leastBorderSwaps)
        {
          low = i;
          leastBorderSwaps = current;
          lowDuplicate = false;
        }
        else if (current == mostBorderSwaps)
        {
          highDuplicate = true;
        }
        else if (current == leastBorderSwaps)
        {
          lowDuplicate = true;
        }
      }
      if (!highDuplicate) activePlayers[high].medals.Add("TELEPORTER");
      if (!lowDuplicate) activePlayers[low].medals.Add("CAUTIOUS");

      // Airborne: most time in air
      // Grounded: least time in air
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostAirTime = activePlayers[high].airTime;
      int leastAirTime = mostAirTime;
      for (int i = high + 1; i < numPlayers; i++){
        int current = activePlayers[i].airTime;
        if(current > mostAirTime){
          high = i;
          mostAirTime = current;
          highDuplicate = false;
        }
        else if(current < leastAirTime){
          low = i;
          leastAirTime = current;
          lowDuplicate = false;
        }
        else if(current == mostAirTime)
        {
          highDuplicate = true;
        }
        else if(current == leastAirTime)
        {
          lowDuplicate = true;
        }
      }
      if (!highDuplicate) activePlayers[high].medals.Add("AIRBORNE");
      if (!lowDuplicate) activePlayers[low].medals.Add("GROUNDED");

      // Astronaut: most gravity swaps
      // Steady: least gravity 
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostGravitySwaps = activePlayers[high].gravitySwapCount;
      int leastGravitySwaps = mostGravitySwaps;
      for (int i = high + 1; i < numPlayers; i++)
      {
        int current = activePlayers[i].gravitySwapCount;
        if (current > mostGravitySwaps)
        {
          high = i;
          mostGravitySwaps = current;
          highDuplicate = false;
        }
        else if (current < leastGravitySwaps)
        {
          low = i;
          leastGravitySwaps = current;
          lowDuplicate = false;
        }
        else if (current == mostGravitySwaps)
        {
          highDuplicate = true;
        }
        else if (current == leastGravitySwaps)
        {
          lowDuplicate = true;
        }
      }
      if (!highDuplicate) activePlayers[high].medals.Add("ASTRONAUT");
      if (!lowDuplicate) activePlayers[low].medals.Add("STEADY");

      // Athlete: most distance traveled
      // Camper: least distance traveled
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostDistance = activePlayers[high].steps;
      int leastDistance = mostDistance;
      for (int i = high + 1; i < numPlayers; i++)
      {
        int current = activePlayers[i].steps;
        if (current > mostDistance)
        {
          high = i;
          mostDistance = current;
          highDuplicate = false;
        }
        else if (current < leastDistance)
        {
          low = i;
          leastDistance = current;
          lowDuplicate = false;
        }
        else if (current == mostDistance)
        {
          highDuplicate = true;
        }
        else if (current == leastDistance)
        {
          lowDuplicate = true;
        }
      }
      if (!highDuplicate) activePlayers[high].medals.Add("ATHLETE");
      if (!lowDuplicate) activePlayers[low].medals.Add("CAMPER");

      /////////////////////////////// COMBAT ONLY MEDALS ////////////////////////////////////////////
      if (gamemode == GameMode.SURVIVAL || gamemode == GameMode.DEATHMATCH)
      {
        // First Blood

        // Hoarder: most bullets picked up
        // Poverty: least bullets picked up
        lowDuplicate = false; highDuplicate = false;
        high = 0;
        low = 0;
        int mostBullets = activePlayers[high].bulletPickUps;
        int leastBullets = mostBullets;
        for (int i = high + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].bulletPickUps;
          if (current > mostBullets)
          {
            high = i;
            mostBullets = current;
            highDuplicate = false;
          }
          else if (current < leastBullets)
          {
            low = i;
            leastBullets = current;
            lowDuplicate = false;
          }
          else if (current == mostBullets)
          {
            highDuplicate = true;
          }
          else if (current == leastBullets)
          {
            lowDuplicate = true;
          }
        }
        if (!highDuplicate) activePlayers[high].medals.Add("HOARDER");
        if (!lowDuplicate) activePlayers[low].medals.Add("POVERTY");

        // Survivor: longest life
        duplicate = false;
        high = 0;
        int longestLife = activePlayers[high].longestLife;
        for (int i = high + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].longestLife;
          if (current > longestLife)
          {
            high = i;
            longestLife = current;
            duplicate = false;
          }
          else if (current == longestLife)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[high].medals.Add("SURVIVOR");

        // Reckless: shortest life
        duplicate = false;
        low = 0;
        int shortestLife = activePlayers[high].shortestLife;
        for (int i = low + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].shortestLife;
          if (current < shortestLife)
          {
            low = i;
            shortestLife = current;
            duplicate = false;
          }
          else if (current == shortestLife)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[low].medals.Add("RECKLESS");

        // Samurai: best sword accuracy
        duplicate = false;
        high = 0;
        float bestSwordAccuracy = (float)(activePlayers[high].numSwordHits) / (activePlayers[high].numSwordSwipes);
        for (int i = high + 1; i < numPlayers; i++)
        {
          float current = (float)(activePlayers[i].numSwordHits) / (activePlayers[i].numSwordSwipes);
          if (current > bestSwordAccuracy)
          {
            high = i;
            bestSwordAccuracy = current;
            duplicate = false;
          }
          else if (current == bestSwordAccuracy)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[high].medals.Add("SAMURAI");

        // Sniper: bullet accuracy
        duplicate = false;
        high = 0;
        float bestBulletAccuracy = (float)(activePlayers[high].numBulletHits) / (activePlayers[high].numBulletShots);
        for (int i = high + 1; i < numPlayers; i++)
        {
          float current = (float)(activePlayers[i].numBulletHits) / (activePlayers[i].numBulletShots);
          if (current > bestBulletAccuracy)
          {
            high = i;
            bestBulletAccuracy = current;
            duplicate = false;
          }
          else if (current == bestBulletAccuracy)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[high].medals.Add("SNIPER");

        // Assassin: most sword kills
        duplicate = false;
        high = 0;
        int mostSwordKills = activePlayers[high].numSwordHits;
        for (int i = high + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].numSwordHits;
          if (current > mostSwordKills)
          {
            high = i;
            mostSwordKills = current;
            duplicate = false;
          }
          else if (current == mostSwordKills)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[high].medals.Add("ASSASSIN");

        // Gunslinger: most bullet kills
        duplicate = false;
        high = 0;
        int mostBulletKills = activePlayers[high].numBulletHits;
        for (int i = high + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].numBulletHits;
          if (current > mostBulletKills)
          {
            high = i;
            mostBulletKills = current;
            duplicate = false;
          }
          else if (current == mostBulletKills)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[high].medals.Add("GUNSLINGER");

        // Pacifist: least kills
        duplicate = false;
        low = 0;
        int leastKills = activePlayers[low].numBulletHits + activePlayers[low].numSwordHits;
        for (int i = low + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].numBulletHits + activePlayers[low].numSwordHits;
          if (current < leastKills)
          {
            low = i;
            leastKills = current;
            duplicate = false;
          }
          else if (current == leastKills)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[low].medals.Add("PACIFIST");

        // Whoops: most suicides
        duplicate = false;
        high = 0;
        int mostSuicides = activePlayers[high].suicides;
        for (int i = high + 1; i < numPlayers; i++)
        {
          int current = activePlayers[i].suicides;
          if (current > mostSuicides)
          {
            high = i;
            mostSuicides = current;
            duplicate = false;
          }
          else if (current == mostSuicides)
          {
            duplicate = true;
          }
        }
        if (!duplicate) activePlayers[high].medals.Add("WHOOPS");

        /* poisons taken out
        // Lich King: most poisons
        high = 0;
        int mostPoisons = activePlayers[high].totalPoisoned;
        for (int i = high + 1; i < numPlayers; i++){
          int current = activePlayers[i].totalPoisoned;
          if(current > mostPoisons){
            high = i;
            mostPoisons = current;
          }
        }
        activePlayers[high].medals.Add("LICH KING");
        */
      }
      else if (gamemode == GameMode.REVERSE_TAG) ////// REVERSE TAG ONLY MEDALS
      {
        // Longest Held
        // Shortest Held
        lowDuplicate = false; highDuplicate = false;
        high = 0;
        low = 0;
        float longestHeld = activePlayers[high].rt_longest_continuous_hold;
        float shortestHeld = longestHeld;
        for (int i = high + 1; i < numPlayers; i++)
        {
          float current = activePlayers[i].rt_longest_continuous_hold;
          if (current > longestHeld)
          {
            high = i;
            longestHeld = current;
            highDuplicate = false;
          }
          else if (current < shortestHeld)
          {
            low = i;
            shortestHeld = current;
            lowDuplicate = false;
          }
          else if (current == longestHeld)
          {
            highDuplicate = true;
          }
          else if (current == shortestHeld)
          {
            lowDuplicate = true;
          }
        }
        if (!highDuplicate) activePlayers[high].medals.Add("IRON GRIP");
        if (!lowDuplicate) activePlayers[low].medals.Add("BUTTERFINGERS");

        // Most Steals
        // Least Steals

        // First Touch
      }

      // Participant: had fun (couldn't find an award for them)
      for (int i = 0; i < numPlayers; i++){
        if(activePlayers[i].medals.Capacity == 0){
          activePlayers[i].medals.Add("PARTICIPANT");
        }
      }
    }
  }

  void DisplayResults(){
    if (PostGameOb != null) PostGameOb.SetActive(true);
    //if(PostGameUI != null) PostGameUI.enabled = true;

    // show the end game UI
    Transform place1, place2, place3, place4;

    place1 = PostGameOb.transform.Find("1place");
    place2 = PostGameOb.transform.Find("2place");
    place3 = PostGameOb.transform.Find("3place");
    place4 = PostGameOb.transform.Find("4place");

    if(first != null){
      float acc = first.GetComponent<player>().acceleration;
      first.GetComponent<player>().acceleration = 0f;
      place1.FindChild("trophy").GetComponent<Image>().sprite = gold;
      first.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      first.GetComponent<player>().acceleration = acc;
      first.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      first.transform.position = podiumPositions[0];
      first.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place1, first.GetComponent<player>(), first.GetComponent<player>().medals);
    }
    else{
      place1.gameObject.SetActive(false);
    }

    if(second != null){
      float acc = second.GetComponent<player>().acceleration;
      second.GetComponent<player>().acceleration = 0f;
      place2.FindChild("trophy").GetComponent<Image>().sprite = silver;
      second.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      second.GetComponent<player>().acceleration = acc;
      second.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      second.transform.position = podiumPositions[1];
      second.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place2, second.GetComponent<player>(), second.GetComponent<player>().medals);
    }
    else{
      place2.gameObject.SetActive(false);
    }

    if(third != null){
      float acc = third.GetComponent<player>().acceleration;
      third.GetComponent<player>().acceleration = 0f;
      place3.FindChild("trophy").GetComponent<Image>().sprite = bronze;
      third.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      third.GetComponent<player>().acceleration = acc;
      third.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      third.transform.position = podiumPositions[2];
      third.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      third.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place3, third.GetComponent<player>(), third.GetComponent<player>().medals);
    }
    else{
      place3.gameObject.SetActive(false);
    }

    if(fourth != null){
      float acc = fourth.GetComponent<player>().acceleration;
      fourth.GetComponent<player>().acceleration = 0f;
      place4.FindChild("trophy").GetComponent<Image>().enabled = false;
      fourth.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      fourth.GetComponent<player>().acceleration = acc;
      fourth.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      fourth.transform.position = podiumPositions[3];
      fourth.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place4, fourth.GetComponent<player>(), fourth.GetComponent<player>().medals);
    }
    else{
      place4.gameObject.SetActive(false);
    }
  }

  void setResultsUI(Transform place, player p, List<string> earnedMedals){
    string medal1 = "", medal2 = "", medal3 = "";
    string info1 = "", info2 = "", info3 = "";

    if(gamemode == GameMode.DEATHMATCH || gamemode == GameMode.SURVIVAL){
      place.FindChild("killsText").GetComponent<Text>().text = "Kills: " + p.playersKilled.Count;
      place.FindChild("deathsText").GetComponent<Text>().text = "Deaths: " + (10 - p.lives);
    }
    else if(gamemode == GameMode.REVERSE_TAG){
      place.FindChild("killsText").GetComponent<Text>().text = "Points: " + p.rt_points;
      place.FindChild("deathsText").GetComponent<Text>().text = "";
    }

    int medalCount = earnedMedals.Count;

    if(medalCount > 0){
      medal1 = earnedMedals[Random.Range(0, medalCount)];
      info1 = medals[medal1];
      medal1 += ":";
      medalCount--;
    }
    if(medalCount > 0){
      medal2 = earnedMedals[Random.Range(0, medalCount)];
      info2 = medals[medal2];
      medal2 += ":";
      medalCount--;
    }
    if(medalCount > 0){
      medal3 = earnedMedals[Random.Range(0, medalCount)];
      info3 = medals[medal3];
      medal3 += ":";
      medalCount--;
    }

    place.FindChild("medal1").GetComponent<Text>().text = medal1;
    place.FindChild("info1").GetComponent<Text>().text = info1;
    place.FindChild("medal2").GetComponent<Text>().text = medal2;
    place.FindChild("info2").GetComponent<Text>().text = info2;
    place.FindChild("medal3").GetComponent<Text>().text = medal3;
    place.FindChild("info3").GetComponent<Text>().text = info3;
  }

  void SetReady(int controller){
    if(first.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("1place").FindChild("AButton").GetComponent<Image>().enabled = false;
      PostGameOb.transform.Find("1place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
    else if(second.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("2place").FindChild("AButton").GetComponent<Image>().enabled = false;
      PostGameOb.transform.Find("2place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
    else if(third.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("3place").FindChild("AButton").GetComponent<Image>().enabled = false;
      PostGameOb.transform.Find("3place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
    else if(fourth.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("4place").FindChild("AButton").GetComponent<Image>().enabled = false;
      PostGameOb.transform.Find("4place").FindChild("readyText").GetComponent<Text>().enabled = true;
    }
  }
}
