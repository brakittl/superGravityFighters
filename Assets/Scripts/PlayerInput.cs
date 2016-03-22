using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    public int player = 1;
    GameObject xbutton, text;

	// Use this for initialization
	void Start () {
        xbutton = GameObject.Find("P" + player + " X Button Image");
        text = GameObject.Find("P" + player + " Text");
        Debug.Log(player);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("Controller " + player + " X Button"))
        {
            if (xbutton != null)
            {
                xbutton.SetActive(false);
            }
            if (text != null)
            {
                text.SetActive(false);
            }
        }
        if (Input.GetButtonDown("Controller " + player + " A Button"))
        {
            
        }
        if (Input.GetButtonDown("Controller " + player + " B Button"))
        {
            if (xbutton != null)
            {
                xbutton.SetActive(true);
            }
            if (text != null)
            {
                text.SetActive(true);
            }
        }
        if (Input.GetButtonDown("Controller " + player + " Y Button"))
        {
            
        }
    }
}
