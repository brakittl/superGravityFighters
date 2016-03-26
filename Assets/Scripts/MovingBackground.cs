using UnityEngine;
using System.Collections;

public class MovingBackground : MonoBehaviour {
	// Use this for initialization
	void Start () {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
        {
            Debug.Log("Windows detected");
        }
        else
        {
            Debug.Log("Mac detected");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.y >= 1)
        {
            transform.localScale = transform.localScale / 1.0025f;
        }
        
	}
}
