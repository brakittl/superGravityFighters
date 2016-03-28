using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

  public static Level S;
  public Vector3[] respawnPoints;

  // Use this for initialization
  void Start(){
    S = this;
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