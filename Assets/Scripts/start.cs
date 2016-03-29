using UnityEngine;
using System.Collections;

public class start : MonoBehaviour {

	// Use this for initialization
	void Start () {
    // p1
    if(PlayerPrefs.GetString("P1") != "none"){
      // (GameObject)Instantiate(Resources.Load(PlayerPrefs.GetString("P1")));
    }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
