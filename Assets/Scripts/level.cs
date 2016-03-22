using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

    public static Level S;
    public Vector3[] respawnPoints;

	// Use this for initialization
	void Start () {
        S = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 findRespawn()
    {
        Vector3 respawn = new Vector3(0, 0, 0);
        float closestPlayer = 10000f;
        float closest = 0;
        GameObject closestP = null;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(Vector3 point in respawnPoints)
        {
            
            closestP = null;
            foreach(GameObject player in players) //Find closest player to this point
            {
                print(Mathf.Abs(Vector3.Distance(point, player.transform.position)));
                if((Mathf.Abs(Vector3.Distance(point, player.transform.position)) < closestPlayer) || (closestP = null))
                {
                    closestPlayer = Mathf.Abs(Vector3.Distance(point, player.transform.position));
                    closestP = player;
                    print("closest player to respawn point " + point.ToString() + " is " + closestP.name);
                }
            }
            //If the closest player is further than the closest so far set that as respawn
            if (closestP != null)
            {
                if (Mathf.Abs(Vector3.Distance(point, closestP.transform.position)) > closest)
                {
                    respawn = point;
                    closest = Mathf.Abs(Vector3.Distance(point, closestP.transform.position));
                }
            }
        }
        return respawn;
    }
}
