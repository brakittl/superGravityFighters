using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controllers : MonoBehaviour {
  public string mac = "";

  public Dictionary<string, string> controlInput = new Dictionary<string, string>()
  {
    {"1 A Button","Controller 1 A Button" },
    {"1 B Button","Controller 1 B Button" },
    {"1 X Button","Controller 1 X Button" },
    {"1 Y Button","Controller 1 Y Button" },
    {"1 Right Bumper","Controller 1 Right Bumper" },
    {"1 Left Bumper","Controller 1 Left Bumper" },
    {"1 Right Stick Click","Controller 1 Right Stick Click" },
    {"1 Left Stick Click","Controller 1 Left Stick Click" },
    {"1 Back Button","Controller 1 Back Button" },
    {"1 Start Button","Controller 1 Start Button" },
    {"1 Left Trigger","Controller 1 Left Trigger" },
    {"1 Right Trigger","Controller 1 Right Trigger" },
    {"1 Left Stick X Axis","Controller 1 Left Stick X Axis" },
    {"1 Left Stick Y Axis","Controller 1 Left Stick Y Axis" },
    {"1 Right Stick X Axis","Controller 1 Right Stick X Axis" },
    {"1 Right Stick Y Axis","Controller 1 Right Stick Y Axis" },
    {"2 A Button","Controller 2 A Button" },
    {"2 B Button","Controller 2 B Button" },
    {"2 X Button","Controller 2 X Button" },
    {"2 Y Button","Controller 2 Y Button" },
    {"2 Right Bumper","Controller 2 Right Bumper" },
    {"2 Left Bumper","Controller 2 Left Bumper" },
    {"2 Right Stick Click","Controller 2 Right Stick Click" },
    {"2 Left Stick Click","Controller 2 Left Stick Click" },
    {"2 Back Button","Controller 2 Back Button" },
    {"2 Start Button","Controller 2 Start Button" },
    {"2 Left Trigger","Controller 2 Left Trigger" },
    {"2 Right Trigger","Controller 2 Right Trigger" },
    {"2 Left Stick X Axis","Controller 2 Left Stick X Axis" },
    {"2 Left Stick Y Axis","Controller 2 Left Stick Y Axis" },
    {"2 Right Stick X Axis","Controller 2 Right Stick X Axis" },
    {"2 Right Stick Y Axis","Controller 2 Right Stick Y Axis" },
    {"3 A Button","Controller 3 A Button" },
    {"3 B Button","Controller 3 B Button" },
    {"3 X Button","Controller 3 X Button" },
    {"3 Y Button","Controller 3 Y Button" },
    {"3 Right Bumper","Controller 3 Right Bumper" },
    {"3 Left Bumper","Controller 3 Left Bumper" },
    {"3 Right Stick Click","Controller 3 Right Stick Click" },
    {"3 Left Stick Click","Controller 3 Left Stick Click" },
    {"3 Back Button","Controller 3 Back Button" },
    {"3 Start Button","Controller 3 Start Button" },
    {"3 Left Trigger","Controller 3 Left Trigger" },
    {"3 Right Trigger","Controller 3 Right Trigger" },
    {"3 Left Stick X Axis","Controller 3 Left Stick X Axis" },
    {"3 Left Stick Y Axis","Controller 3 Left Stick Y Axis" },
    {"3 Right Stick X Axis","Controller 3 Right Stick X Axis" },
    {"3 Right Stick Y Axis","Controller 3 Right Stick Y Axis" },
    {"4 A Button","Controller 4 A Button" },
    {"4 B Button","Controller 4 B Button" },
    {"4 X Button","Controller 4 X Button" },
    {"4 Y Button","Controller 4 Y Button" },
    {"4 Right Bumper","Controller 4 Right Bumper" },
    {"4 Left Bumper","Controller 4 Left Bumper" },
    {"4 Right Stick Click","Controller 4 Right Stick Click" },
    {"4 Left Stick Click","Controller 4 Left Stick Click" },
    {"4 Back Button","Controller 4 Back Button" },
    {"4 Start Button","Controller 4 Start Button" },
    {"4 Left Trigger","Controller 4 Left Trigger" },
    {"4 Right Trigger","Controller 4 Right Trigger" },
    {"4 Left Stick X Axis","Controller 4 Left Stick X Axis" },
    {"4 Left Stick Y Axis","Controller 4 Left Stick Y Axis" },
    {"4 Right Stick X Axis","Controller 4 Right Stick X Axis" },
    {"4 Right Stick Y Axis","Controller 4 Right Stick Y Axis" },
  };

  // Use this for initialization
  void Start () {
	  if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
    {
      // Windows controls
      Debug.Log("Windows Controls Set");
      mac = "";
    }
    else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
    {
      // Mac Controls
      Debug.Log("Mac Controls Set");
      mac = "Mac ";
    }
    else
    {
      Debug.LogError("Unknown RuntimePlatform: Windows Controls Set");
      mac = "";
    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
