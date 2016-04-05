using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameMode{ NONE, SURVIVAL, DEATHMATCH, REVERSE_TAG };

public class Level : MonoBehaviour{

  public static Level S;
    public GameMode gamemode; // You can use this for selecting the GameMode right from the Map Screen
  public int rt_point_limit = 100;

  public Vector3[] respawnPoints;
    public List<player> ranking = new List<player>();
    int numPlayers;
  Texture2D black = new Texture2D(1, 1);

  void Start(){

    // ==[game mode]============================================================
    // =========================================================================

    S = this;

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
    // if(PlayerPrefs.GetString("P1") != "none"){
    //   GameObject player1 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P1")), returnPosition(0), Quaternion.Euler(rot)) as GameObject;
    //   player1.SendMessage("SetPlayerNumber", 1);
    // }
    // // p2
    // if(PlayerPrefs.GetString("P2") != "none"){
    //   GameObject player2 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P2")), returnPosition(1), Quaternion.Euler(rot)) as GameObject;
    //   player2.SendMessage("SetPlayerNumber", 2);
    // }
    // // p3
    // if(PlayerPrefs.GetString("P3") != "none"){
    //   GameObject player3 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P3")), returnPosition(2), Quaternion.Euler(rot)) as GameObject;
    //   player3.SendMessage("SetPlayerNumber", 3);
    // }
    // // p4
    // if(PlayerPrefs.GetString("P4") != "none"){
    //   GameObject player4 = Instantiate(Resources.Load("__Prefabs/_players" + PlayerPrefs.GetString("P4")), returnPosition(3), Quaternion.Euler(rot)) as GameObject;
    //   player4.SendMessage("SetPlayerNumber", 4);
    // }

  }

    void Update()
    {
        int count = 0;
        foreach (player p in ranking)
            count++;
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

    cubePos.y = pos.y - 4.3f;
    bottomWall = Instantiate(blackSquare, cubePos, transform.rotation) as GameObject;
    cubePos.y = pos.y + 4.3f;
    topWall = Instantiate(blackSquare, cubePos, transform.rotation) as GameObject;
    cubePos.x = pos.x - 4.3f;
    rot.z = 90;
    leftWall = Instantiate(blackSquare, cubePos, Quaternion.Euler(rot)) as GameObject;
    cubePos.x = pos.x + 4.3f;
    rightWall = Instantiate(blackSquare, cubePos, Quaternion.Euler(rot)) as GameObject;

    yield return new WaitForSeconds(0.029f);
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
}
