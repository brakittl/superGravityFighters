using UnityEngine;
using System.Collections;

public class MovingBackground : MonoBehaviour {
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.y >= 1)
        {
            transform.localScale = transform.localScale / 1.0025f;
        }
        
	}
}
