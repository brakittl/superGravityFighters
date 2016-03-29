using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

  public static Level S;
  public Vector3[] respawnPoints;

  // Use this for initialization
  void Start(){

    S = this;

    // PlayerPrefs.SetString("P1", "orange_player");
    // PlayerPrefs.SetString("P2", "none");
    // PlayerPrefs.SetString("P3", "red_player");
    // PlayerPrefs.SetString("P4", "none");

    Vector3 rot = new Vector3(0, 0, 0);

    // p1
    if(PlayerPrefs.GetString("P1") != "none"){
      GameObject player1 = Instantiate(Resources.Load("__Prefabs/" + PlayerPrefs.GetString("P1")), returnPosition(0), Quaternion.Euler(rot)) as GameObject;
      player1.SendMessage("SetPlayerNumber", 1);
    }
    // p2
    if(PlayerPrefs.GetString("P2") != "none"){
      GameObject player2 = Instantiate(Resources.Load("__Prefabs/" + PlayerPrefs.GetString("P2")), returnPosition(1), Quaternion.Euler(rot)) as GameObject;
      player2.SendMessage("SetPlayerNumber", 2);
    }
    // p3
    if(PlayerPrefs.GetString("P3") != "none"){
      GameObject player3 = Instantiate(Resources.Load("__Prefabs/" + PlayerPrefs.GetString("P3")), returnPosition(2), Quaternion.Euler(rot)) as GameObject;
      player3.SendMessage("SetPlayerNumber", 3);
    }
    // p4
    if(PlayerPrefs.GetString("P4") != "none"){
      GameObject player4 = Instantiate(Resources.Load("__Prefabs/" + PlayerPrefs.GetString("P4")), returnPosition(3), Quaternion.Euler(rot)) as GameObject;
      player4.SendMessage("SetPlayerNumber", 4);
    }

  }

  public Vector3 returnPosition(int i){
    return respawnPoints[i];
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
          print("closest player to respawn point " + point.ToString() + " is " + closestP.name);
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