using UnityEngine;
using System.Collections;

public class start : MonoBehaviour {

	// Use this for initialization
	void Start () {
    // p1
    if(PlayerPrefs.GetString("P1") != "none"){
      GameObject player1 = Instantiate(Resources.Load(PlayerPrefs.GetString("P1"))) as GameObject;
    }
    // p2
    if(PlayerPrefs.GetString("P2") != "none"){
      GameObject player2 = Instantiate(Resources.Load(PlayerPrefs.GetString("P2"))) as GameObject;
    }
    // p3
    if(PlayerPrefs.GetString("P3") != "none"){
      GameObject player3 = Instantiate(Resources.Load(PlayerPrefs.GetString("P3"))) as GameObject;
    }
    // p4
    if(PlayerPrefs.GetString("P4") != "none"){
      GameObject player4 = Instantiate(Resources.Load(PlayerPrefs.GetString("P4"))) as GameObject;
    }

	}
}
