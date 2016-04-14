using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode{ NONE, SURVIVAL, DEATHMATCH, REVERSE_TAG };

public class level : MonoBehaviour {
  public string mac = "";

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
  public GameObject PostGameBoxes;
  public Canvas PostGameUI;

  public GameObject gravity_stone;
  public Vector3 stone_position;

  public Sprite gold, silver, bronze;
  List<Vector3> podiumPositions = new List<Vector3>(){ new Vector3(-1.56f, 5f, 0f), new Vector3(-0.29f, 5f, 0f), new Vector3(0.98f, 5f, 0f), new Vector3(2.22f, 5f, 0f) };

  public List<GameObject> alive_players;
  public Vector3 target_position;

  public GameObject killStreak;
  GameObject streak;
  public bool pause = false, running = false;

  float shakeIntensity = 0.2f;
  bool camera_shaking = false;
  public bool block_camera_shake;

    AudioSource music;
    public AudioClip victoryMusic;
    bool playingVictory = false;

  public Dictionary<string, string> medals = new Dictionary<string, string>(){
    { "SKY DIVER","most consecutive\nairtime" },
    { "TELEPORTER","most border\nteleports" },
    { "CAUTIOUS","least border\nteleports" },
    { "AIRBORNE","most time\nin air" },
    { "GROUNDED","least\nairtime" },
    { "HOARDER","most bullets\npicked up" },
    { "POVERTY","least bullets\npicked up" },
    { "ASTRONAUT","most gravity\nswaps" },
    { "STEADY","least gravity\nswaps" },
    { "SURVIVOR","longest\nlife" },
    { "RECKLESS","shortest\nlife" },
    { "LICH KING","most\npoisons" },
    { "SAMURAI","best sword\naccuracy" },
    { "SNIPER","best bullet\naccuracy" },
    { "ASSASSIN","most sword\nkills" },
    { "GUNSLINGER","most gun\nkills" },
    { "PACIFIST","least\nkills" },
    { "ATHLETE","most distance\ntraveled" },
    { "CAMPER","least distance\ntraveled" },
    { "WHOOPS","most\nsuicides" },
    { "PARTICIPANT","you had\nfun :)" },
    { "THUG LIFE","most\nsteals" },
    { "GOOD PERSON","least\nsteals" },
    { "IRON GRIP","longest single\npossession" },
    { "BUTTERFINGERS","shortest single\npossession" },
    { "QUICKDRAW","first\npossession" }
  };

  void Start(){

    // Mac Check
    if(Application.platform == RuntimePlatform.OSXEditor ||
       Application.platform == RuntimePlatform.OSXPlayer ||
       Application.platform == RuntimePlatform.OSXPlayer ){
      mac = "Mac ";
    }
    else{
      mac = "";
    }

    if(SceneManager.GetActiveScene().name == "_city"   ||
       SceneManager.GetActiveScene().name == "_forest" ||
       SceneManager.GetActiveScene().name == "_galaxy" ||
       SceneManager.GetActiveScene().name == "_desert" ||
       SceneManager.GetActiveScene().name == "_volcano"){
      isMap = true;
    }
    else{
      isMap = false;
    }

    block_camera_shake = true;

    // ==[game mode]============================================================
    // =========================================================================

    S = this;
        music = GetComponent<AudioSource>();

    if(isMap){

      // rt_point_limit = PlayerPrefs.GetInt("rt_point_limit");
      podium = GameObject.Find("Podium");
      if(podium != null){
        podium.SetActive(false);
      }
      PostGameOb = GameObject.Find("PostGameCanvas");
      PostGameBoxes = GameObject.Find("PostGameBoxes");
      if(PostGameOb != null){
        PostGameOb.SetActive(false);
      }
      if(PostGameBoxes != null){
        PostGameBoxes.SetActive(false);
      }
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
			else{
        Debug.Log("INCORRECT GAMEMODE STRING PASSED IN FROM PLAYERPREFS. CHECK level SCRIPT");
      }
		}

    // ==[players]============================================================
    // =========================================================================

    // PlayerPrefs.SetString("P1", "red_player");
    // PlayerPrefs.SetString("P2", "black_player");
    // PlayerPrefs.SetString("P3", "yellow_player");
    // PlayerPrefs.SetString("P4", "green_player");

    Vector3 rot = new Vector3(0, 0, 0);

    if(isMap){
      numPlayers = 0;
      // p1      
      if(PlayerPrefs.GetString("P1") != "none"){
        player1 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P1")), returnPosition(0), Quaternion.Euler(rot)) as GameObject;
        player1.SendMessage("SetPlayerNumber", 1);
        numPlayers++;
        alive_players.Insert(0, player1);
      }
      // p2
      if(PlayerPrefs.GetString("P2") != "none"){
        player2 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P2")), returnPosition(1), Quaternion.Euler(rot)) as GameObject;
        player2.SendMessage("SetPlayerNumber", 2);
        numPlayers++;
        alive_players.Insert(0, player2);
      }
      // p3
      if(PlayerPrefs.GetString("P3") != "none"){
        player3 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P3")), returnPosition(2), Quaternion.Euler(rot)) as GameObject;
        player3.SendMessage("SetPlayerNumber", 3);
        numPlayers++;
        alive_players.Insert(0, player3);
      }
      // p4
      if(PlayerPrefs.GetString("P4") != "none"){
        player4 = Instantiate(Resources.Load("__Prefabs/_players/" + PlayerPrefs.GetString("P4")), returnPosition(3), Quaternion.Euler(rot)) as GameObject;
        player4.SendMessage("SetPlayerNumber", 4);
        numPlayers++;
        alive_players.Insert(0, player4);
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
      /*
            if (!playingVictory)
            {
        music.clip = victoryMusic;
                //music.PlayOneShot(victoryMusic);
                playingVictory = true;
            }*/
                
      // pressed a to ready up
      if(!player1Ready && Input.GetButtonDown(mac + "Controller 1 A Button")){
        player1Ready = true;
        SetReady(1);
        readyCount++;
      }
      if(!player2Ready && Input.GetButtonDown(mac + "Controller 2 A Button")){
        player2Ready = true;
        SetReady(2);
        readyCount++;
      }
      if(!player3Ready && Input.GetButtonDown(mac + "Controller 3 A Button")){
        player3Ready = true;
        SetReady(3);
        readyCount++;
      }
      if(!player4Ready && Input.GetButtonDown(mac + "Controller 4 A Button")){
        player4Ready = true;
        SetReady(4);
        readyCount++;
      }

      // pressed b to unconfirm ready
      if(player1Ready && Input.GetButtonDown(mac + "Controller 1 B Button")){
        player1Ready = false;
        SetUnready(1);
        readyCount--;
      }
      if(player2Ready && Input.GetButtonDown(mac + "Controller 2 B Button")){
        player2Ready = false;
        SetUnready(2);
        readyCount--;
      }
      if(player3Ready && Input.GetButtonDown(mac + "Controller 3 B Button")){
        player3Ready = false;
        SetUnready(3);
        readyCount--;
      }
      if(player4Ready && Input.GetButtonDown(mac + "Controller 4 B Button")){
        player4Ready = false;
        SetUnready(4);
        readyCount--;
      }

      // check if all available players are ready
      if(readyCount >= numPlayers){
        Debug.Log("ReadyCount: " + readyCount);
        Debug.Log("numPlayers: " + numPlayers);
        SceneManager.LoadScene("_map_game_select");
      }
    }
    else{

      if(!camera_shaking){
        MoveCamera();
      }

      if(gamemode == GameMode.SURVIVAL){
        // check number of dead players
        int deadCount = 0;
        foreach (player p in ranking){
          Debug.Log(p);
          deadCount++;
        }

        if(numPlayers - deadCount <= 1){
          Debug.Log("GAMEOVER COUNT");
          gameOver = true;
          music.clip = victoryMusic;
          if (!music.isPlaying) music.Play();

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

          foreach (player p in activePlayers)
          {
            p.gameOver = true;
          }

          GameObject.Find("ui").SetActive(false);
          GameObject.Find("border").SetActive(false);
          GameObject.Find("colliders").SetActive(false);
          podium.SetActive(true);

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
        }
      }
      else if(gamemode == GameMode.DEATHMATCH){
        if(ranking.Capacity > 0){
          gameOver = true;
          music.clip = victoryMusic;
          if (!music.isPlaying) music.Play();

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

          foreach (player p in activePlayers)
          {
            p.gameOver = true;
          }

          GameObject.Find("ui").SetActive(false);
          GameObject.Find("border").SetActive(false);
          GameObject.Find("colliders").SetActive(false);
          podium.SetActive(true);

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
        }
      }
      else if(gamemode == GameMode.REVERSE_TAG){
        if(ranking.Count > 0){
          gameOver = true;
          music.clip = victoryMusic;
          if (!music.isPlaying) music.Play();
          // point limit reached, end the game
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

          foreach (player p in activePlayers)
          {
            p.gameOver = true;
          }

          GameObject.FindGameObjectWithTag("TagBall").SetActive(false);
          GameObject.Find("ui").SetActive(false);
          GameObject.Find("border").SetActive(false);
          GameObject.Find("colliders").SetActive(false);
          podium.SetActive(true);

          // sort by rt_points, ties broken by longest continuous hold
          for (int i = 0; i < numPlayers; i++){
            for(int j = i + 1; j < numPlayers; j++){
              if(activePlayers[i].rt_points < activePlayers[j].rt_points){
                player temp = activePlayers[i];
                activePlayers[i] = activePlayers[j];
                activePlayers[j] = temp;
              }
              else if(activePlayers[i].rt_points == activePlayers[j].rt_points){
                if(activePlayers[i].rt_longest_continuous_hold < activePlayers[j].rt_longest_continuous_hold){
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
        }
      }
    }
  }

  public Vector3 returnPosition(int i){
    return respawnPoints[i];
  }
  
  public void KillPause(Vector3 playerPos, Color player_color, bool create_streak){
    if(!running){
      running = true;
      camera_shaking = true;
      StartCoroutine(Pause(playerPos, player_color, create_streak));
    }
  }

  IEnumerator Pause(Vector3 pos, Color player_color, bool create_streak){
    Time.timeScale = 0.1f;
    Vector3 rot = transform.rotation.eulerAngles, originalPos = gameObject.transform.position;

    rot.z = Random.Range(0, 90);
    if(gamemode == GameMode.SURVIVAL){
      streak = Instantiate(killStreak, pos, Quaternion.Euler(rot)) as GameObject;
			//print(streak.GetComponent<MeshRenderer>().material.color);
			streak.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", player_color);
			//print(streak.GetComponent<MeshRenderer>().material.color);
    }
    
    CameraShake();
    yield return new WaitForSeconds(0.005f);
    gameObject.transform.position = originalPos;

    CameraShake();
    yield return new WaitForSeconds(0.005f);
    gameObject.transform.position = originalPos;

      CameraShake();
      yield return new WaitForSeconds(0.005f);
      gameObject.transform.position = originalPos;

      CameraShake();
      yield return new WaitForSeconds(0.005f);
      gameObject.transform.position = originalPos;

    CameraShake();
    gameObject.transform.position = originalPos;
    camera_shaking = false;
    
    if(gamemode == GameMode.SURVIVAL && create_streak){
      Destroy(streak);
    }
    Time.timeScale = 1;
    running = false;
  }
  
  void CameraShake(){
    Vector3 pos = gameObject.transform.position;
    pos.x = Random.Range(pos.x - shakeIntensity, pos.x + shakeIntensity);
    pos.y = Random.Range(pos.y - shakeIntensity, pos.y + shakeIntensity);
    gameObject.transform.position = pos;
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
        else if(current == longestAirTime){
          duplicate = true;
        }
      }
      if(!duplicate){
        activePlayers[high].medals.Add("SKY DIVER");
      }

      // Teleporter: most border swaps
      // Cautious: least border swap
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostBorderSwaps = activePlayers[high].borderSwaps;
      int leastBorderSwaps = mostBorderSwaps;
      for (int i = high + 1; i < numPlayers; i++){
        int current = activePlayers[i].borderSwaps;
        if(current > mostBorderSwaps){
          high = i;
          mostBorderSwaps = current;
          highDuplicate = false;
        }
        else if(current < leastBorderSwaps){
          low = i;
          leastBorderSwaps = current;
          lowDuplicate = false;
        }
        else if(current == mostBorderSwaps){
          highDuplicate = true;
        }
        else if(current == leastBorderSwaps){
          lowDuplicate = true;
        }
      }
      if(!highDuplicate){
        activePlayers[high].medals.Add("TELEPORTER");
      }
      if(!lowDuplicate){
        //activePlayers[low].medals.Add("CAUTIOUS");
      }

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
        else if(current == mostAirTime){
          highDuplicate = true;
        }
        else if(current == leastAirTime){
          lowDuplicate = true;
        }
      }
      if(!highDuplicate){
        activePlayers[high].medals.Add("AIRBORNE");
      }
      if(!lowDuplicate){
        //activePlayers[low].medals.Add("GROUNDED");
      }

      // Astronaut: most gravity swaps
      // Steady: least gravity 
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostGravitySwaps = activePlayers[high].gravitySwapCount;
      int leastGravitySwaps = mostGravitySwaps;
      for (int i = high + 1; i < numPlayers; i++){
        int current = activePlayers[i].gravitySwapCount;
        if(current > mostGravitySwaps){
          high = i;
          mostGravitySwaps = current;
          highDuplicate = false;
        }
        else if(current < leastGravitySwaps){
          low = i;
          leastGravitySwaps = current;
          lowDuplicate = false;
        }
        else if(current == mostGravitySwaps){
          highDuplicate = true;
        }
        else if(current == leastGravitySwaps){
          lowDuplicate = true;
        }
      }
      if(!highDuplicate){
        activePlayers[high].medals.Add("ASTRONAUT");
      }
      if(!lowDuplicate){
        //activePlayers[low].medals.Add("STEADY");
      }

      // Athlete: most distance traveled
      // Camper: least distance traveled
      lowDuplicate = false; highDuplicate = false;
      high = 0;
      low = 0;
      int mostDistance = activePlayers[high].steps;
      int leastDistance = mostDistance;
      for (int i = high + 1; i < numPlayers; i++){
        int current = activePlayers[i].steps;
        if(current > mostDistance){
          high = i;
          mostDistance = current;
          highDuplicate = false;
        }
        else if(current < leastDistance){
          low = i;
          leastDistance = current;
          lowDuplicate = false;
        }
        else if(current == mostDistance){
          highDuplicate = true;
        }
        else if(current == leastDistance){
          lowDuplicate = true;
        }
      }
      if(!highDuplicate){
        activePlayers[high].medals.Add("ATHLETE");
      }
      if(!lowDuplicate){
        //activePlayers[low].medals.Add("CAMPER");
      }

      //////////////////////// COMBAT ONLY MEDALS //////////////////////////////
      if(gamemode == GameMode.SURVIVAL || gamemode == GameMode.DEATHMATCH){
        
        // First Blood

        // Hoarder: most bullets picked up
        // Poverty: least bullets picked up
        lowDuplicate = false; highDuplicate = false;
        high = 0;
        low = 0;
        int mostBullets = activePlayers[high].bulletPickUps;
        int leastBullets = mostBullets;
        for (int i = high + 1; i < numPlayers; i++){
          int current = activePlayers[i].bulletPickUps;
          if(current > mostBullets){
            high = i;
            mostBullets = current;
            highDuplicate = false;
          }
          else if(current < leastBullets){
            low = i;
            leastBullets = current;
            lowDuplicate = false;
          }
          else if(current == mostBullets){
            highDuplicate = true;
          }
          else if(current == leastBullets){
            lowDuplicate = true;
          }
        }
        if(!highDuplicate){
          activePlayers[high].medals.Add("HOARDER");
        }
        if(!lowDuplicate){
          //activePlayers[low].medals.Add("POVERTY");
        }

        // Survivor: longest life
        duplicate = false;
        high = 0;
        int longestLife = activePlayers[high].longestLife;
        if (longestLife == 0) longestLife = int.MaxValue;
        for (int i = high + 1; i < numPlayers; i++){
          // set 0's to max because they neve died
          int current = activePlayers[i].longestLife;
          if (current == 0) current = int.MaxValue;

          if(current > longestLife){
            high = i;
            longestLife = current;
            duplicate = false;
          }
          else if(current == longestLife){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[high].medals.Add("SURVIVOR");
        }

        // Reckless: shortest life
        duplicate = false;
        low = 0;
        int shortestLife = activePlayers[high].shortestLife;
        for (int i = low + 1; i < numPlayers; i++){
          int current = activePlayers[i].shortestLife;
          if(current < shortestLife){
            low = i;
            shortestLife = current;
            duplicate = false;
          }
          else if(current == shortestLife){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[low].medals.Add("RECKLESS");
        }

        // Samurai: best sword accuracy
        duplicate = false;
        high = 0;
        float bestSwordAccuracy = 0;
        if (activePlayers[high].numSwordSwipes > 0)
        {
          bestSwordAccuracy = (float)(activePlayers[high].numSwordHits) / (activePlayers[high].numSwordSwipes);
        }
        for (int i = high + 1; i < numPlayers; i++){
          float current = 0;
          if (activePlayers[i].numSwordSwipes > 0) current = (float)(activePlayers[i].numSwordHits) / (activePlayers[i].numSwordSwipes);
          if(current > bestSwordAccuracy){
            high = i;
            bestSwordAccuracy = current;
            duplicate = false;
          }
          else if(current == bestSwordAccuracy){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[high].medals.Add("SAMURAI");
        }

        // Sniper: bullet accuracy
        duplicate = false;
        high = 0;
        float bestBulletAccuracy = 0;
        if (activePlayers[high].numBulletShots > 0)
        {
          bestBulletAccuracy = (float)(activePlayers[high].numBulletHits) / (activePlayers[high].numBulletShots);
        }
        for (int i = high + 1; i < numPlayers; i++){
          float current = 0;
          if (activePlayers[i].numBulletShots > 0) current = (float)(activePlayers[i].numBulletHits) / (activePlayers[i].numBulletShots);
          if(current > bestBulletAccuracy){
            high = i;
            bestBulletAccuracy = current;
            duplicate = false;
          }
          else if(current == bestBulletAccuracy){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[high].medals.Add("SNIPER");
        }

        // Assassin: most sword kills
        duplicate = false;
        high = 0;
        int mostSwordKills = activePlayers[high].numSwordHits;
        for (int i = high + 1; i < numPlayers; i++){
          int current = activePlayers[i].numSwordHits;
          if(current > mostSwordKills){
            high = i;
            mostSwordKills = current;
            duplicate = false;
          }
          else if(current == mostSwordKills){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[high].medals.Add("ASSASSIN");
        }

        // Gunslinger: most bullet kills
        duplicate = false;
        high = 0;
        int mostBulletKills = activePlayers[high].numBulletHits;
        for (int i = high + 1; i < numPlayers; i++){
          int current = activePlayers[i].numBulletHits;
          if(current > mostBulletKills){
            high = i;
            mostBulletKills = current;
            duplicate = false;
          }
          else if(current == mostBulletKills){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[high].medals.Add("GUNSLINGER");
        }

        // Pacifist: least kills
        duplicate = false;
        low = 0;
        int leastKills = activePlayers[low].playersKilled.Count;
        for (int i = low + 1; i < numPlayers; i++){
          int current = activePlayers[i].playersKilled.Count;
          if(current < leastKills){
            low = i;
            leastKills = current;
            duplicate = false;
          }
          else if(current == leastKills){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[low].medals.Add("PACIFIST");
        }

        // Whoops: most suicides
        duplicate = false;
        high = 0;
        int mostSuicides = activePlayers[high].suicides;
        for (int i = high + 1; i < numPlayers; i++){
          int current = activePlayers[i].suicides;
          if(current > mostSuicides){
            high = i;
            mostSuicides = current;
            duplicate = false;
          }
          else if(current == mostSuicides){
            duplicate = true;
          }
        }
        if(!duplicate){
          activePlayers[high].medals.Add("WHOOPS");
        }

        // // Lich King: most poisons
        // high = 0;
        // int mostPoisons = activePlayers[high].totalPoisoned;
        // for (int i = high + 1; i < numPlayers; i++){
        //   int current = activePlayers[i].totalPoisoned;
        //   if(current > mostPoisons){
        //     high = i;
        //     mostPoisons = current;
        //   }
        // }
        // activePlayers[high].medals.Add("LICH KING");

      }
      /////////////////////////// RT ONLY MEDALS ///////////////////////////////
      else if(gamemode == GameMode.REVERSE_TAG){
        
        // Longest Held
        // Shortest Held
        lowDuplicate = false; highDuplicate = false;
        high = 0;
        low = 0;
        float longestHeld = activePlayers[high].rt_longest_continuous_hold;
        float shortestHeld = longestHeld;
        for (int i = high + 1; i < numPlayers; i++){
          float current = activePlayers[i].rt_longest_continuous_hold;
          if(current > longestHeld){
            high = i;
            longestHeld = current;
            highDuplicate = false;
          }
          else if(current < shortestHeld){
            low = i;
            shortestHeld = current;
            lowDuplicate = false;
          }
          else if(current == longestHeld){
            highDuplicate = true;
          }
          else if(current == shortestHeld){
            lowDuplicate = true;
          }
        }
        if(!highDuplicate){
          activePlayers[high].medals.Add("IRON GRIP");
        }
        if(!lowDuplicate){
          activePlayers[low].medals.Add("BUTTERFINGERS");
        }

        // Most Steals
        // Least Steals
        lowDuplicate = false; highDuplicate = false;
        high = 0;
        low = 0;
        float mostSteals = activePlayers[high].steals;
        float leastSteals = mostSteals;
        for (int i = high + 1; i < numPlayers; i++){
          float current = activePlayers[i].steals;
          if(current > mostSteals){
            high = i;
            mostSteals = current;
            highDuplicate = false;
          }
          else if(current < leastSteals){
            low = i;
            leastSteals = current;
            lowDuplicate = false;
          }
          else if(current == mostSteals){
            highDuplicate = true;
          }
          else if(current == leastSteals){
            lowDuplicate = true;
          }
        }
        if(!highDuplicate){
          activePlayers[high].medals.Add("THUG LIFE");
        }
        if(!lowDuplicate){
          activePlayers[low].medals.Add("GOOD PERSON");
        }

        // First Touch
        low = 0;
        for (int i = low; i < numPlayers; i++){
          if(activePlayers[low].firstTouch){
            activePlayers[low].medals.Add("QUICKDRAW");
            break;
          }
        }
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

    // stop moving camera
    alive_players.Clear();
    gameObject.transform.position = new Vector3(0f, 0f, -10f);
    camera_shaking = true;

    if(PostGameOb != null){
      PostGameOb.SetActive(true);
    }
    if(PostGameBoxes != null){
      PostGameBoxes.SetActive(true);
    }

    //if(PostGameUI != null) PostGameUI.enabled = true;

    // show the end game UI
    Transform place1, place2, place3, place4;

    place1 = PostGameOb.transform.Find("1place");
    place2 = PostGameOb.transform.Find("2place");
    place3 = PostGameOb.transform.Find("3place");
    place4 = PostGameOb.transform.Find("4place");

    if(first != null){
      first.GetComponent<player>().player_number *= -1;
      first.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      place1.FindChild("trophy").GetComponent<Image>().sprite = gold;
      first.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      first.transform.position = podiumPositions[0];
      first.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place1, first.GetComponent<player>(), first.GetComponent<player>().medals);
    }
    else{
      place1.gameObject.SetActive(false);
    }

    if(second != null){
      second.GetComponent<player>().player_number *= -1;
      second.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      place2.FindChild("trophy").GetComponent<Image>().sprite = silver;
      second.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      second.transform.position = podiumPositions[1];
      second.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place2, second.GetComponent<player>(), second.GetComponent<player>().medals);
    }
    else{
      place2.gameObject.SetActive(false);
    }

    if(third != null){
      third.GetComponent<player>().player_number *= -1;
      third.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      place3.FindChild("trophy").GetComponent<Image>().sprite = bronze;
      third.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
      third.transform.position = podiumPositions[2];
      third.GetComponent<SpriteRenderer>().sortingOrder = 100;
      setResultsUI(place3, third.GetComponent<player>(), third.GetComponent<player>().medals);
    }
    else{
      place3.gameObject.SetActive(false);
    }

    if(fourth != null){
      fourth.GetComponent<player>().player_number *= -1;
      fourth.GetComponent<player>().Gravity(player.orientation.down, 0f, 0f);
      place4.FindChild("trophy").GetComponent<Image>().enabled = false;
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

    string medalString = (p.player_number * -1) + "'s Medals:";
    foreach (string medal in earnedMedals)
    {
      medalString += " " + medal + " ";
    }
    Debug.Log(medalString);

    if(earnedMedals.Count > 0){
      medal1 = earnedMedals[Random.Range(0, earnedMedals.Count)];
      info1 = medals[medal1];
      earnedMedals.Remove(medal1);
      medal1 += ":";
    }
    if(earnedMedals.Count > 0){
      medal2 = earnedMedals[Random.Range(0, earnedMedals.Count)];
      info2 = medals[medal2];
      earnedMedals.Remove(medal2);
      medal2 += ":";
    }
    if(earnedMedals.Count > 0){
      medal3 = earnedMedals[Random.Range(0, earnedMedals.Count)];
      info3 = medals[medal3];
      earnedMedals.Remove(medal3);
      medal3 += ":";
    }

    place.FindChild("medal1").GetComponent<Text>().text = medal1;
    place.FindChild("info1").GetComponent<Text>().text = info1;
    place.FindChild("medal2").GetComponent<Text>().text = medal2;
    place.FindChild("info2").GetComponent<Text>().text = info2;
    place.FindChild("medal3").GetComponent<Text>().text = medal3;
    place.FindChild("info3").GetComponent<Text>().text = info3;
  }

  void MoveCamera(){

    float centroid_x = 0;
    float centroid_y = 0;
    float count = 0;

    foreach(GameObject player in alive_players){
      centroid_x += player.transform.position.x;
      centroid_y += player.transform.position.y;
      ++count;
    }

    target_position = new Vector3(centroid_x / count, centroid_y / count, -10f);
    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target_position, 1.5f * Time.deltaTime);
  }

  void SetReady(int controller){
    controller *= -1; // set opposite due to player's losing conrol of characters
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

  void SetUnready(int controller){
    controller *= -1; // FOR DISABLING THE PLAYER'S CONTROLS
    if(first.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("1place").FindChild("AButton").GetComponent<Image>().enabled = true;
      PostGameOb.transform.Find("1place").FindChild("readyText").GetComponent<Text>().enabled = false;
    }
    else if(second.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("2place").FindChild("AButton").GetComponent<Image>().enabled = true;
      PostGameOb.transform.Find("2place").FindChild("readyText").GetComponent<Text>().enabled = false;
    }
    else if(third.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("3place").FindChild("AButton").GetComponent<Image>().enabled = true;
      PostGameOb.transform.Find("3place").FindChild("readyText").GetComponent<Text>().enabled = false;
    }
    else if(fourth.GetComponent<player>().player_number == controller){
      PostGameOb.transform.Find("4place").FindChild("AButton").GetComponent<Image>().enabled = true;
      PostGameOb.transform.Find("4place").FindChild("readyText").GetComponent<Text>().enabled = false;
    }
  }
}
