using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    public int player = 1;
    GameObject xbutton, text;
    public GameObject playerPrefab;

	// Use this for initialization
	void Start () {
        xbutton = GameObject.Find("P" + player + " X Button Image");
        text = GameObject.Find("P" + player + " Text");
        Debug.Log(player);
        if (player > Input.GetJoystickNames().Length)
        {
            this.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("Controller " + player + " X Button"))
        {
            if (xbutton != null)
            {
                xbutton.SetActive(false);
                GameObject playerObject = Instantiate(playerPrefab) as GameObject;
                playerObject.GetComponent<Rigidbody2D>().gravityScale = 0;
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
