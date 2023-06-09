﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class title : MonoBehaviour {

  public string mac = "";

  int i;

  public List<Text> menu_objects;

  Color red = new Color();
  Color white = new Color();

  public GameObject camera;

  bool axis_held;

	// Use this for initialization
	void Start(){
    // Mac Check
    if (Application.platform == RuntimePlatform.OSXEditor
      || Application.platform == RuntimePlatform.OSXPlayer
      || Application.platform == RuntimePlatform.OSXPlayer){
      mac = "Mac ";
    }
    else{
      mac = "";
    }

    i = 0;
    axis_held = false;

    ColorUtility.TryParseHtmlString("#ec393d", out red);
    ColorUtility.TryParseHtmlString("#ebebeb", out white);

    if(!PlayerPrefs.HasKey("screen")){
      PlayerPrefs.SetString("screen", "MONITOR");
    }
    if(!PlayerPrefs.HasKey("tutorial")){
      PlayerPrefs.SetFloat("tutorial", 1);
    }
    if(!PlayerPrefs.HasKey("music")){
      PlayerPrefs.SetFloat("music", 1);
    }
    if(!PlayerPrefs.HasKey("sfx")){
      PlayerPrefs.SetFloat("sfx", 1);
    }
    if(!PlayerPrefs.HasKey("lives")){
      PlayerPrefs.SetInt("lives", 5);
      PlayerPrefs.SetInt("lives_index", 4);
    }
    if(!PlayerPrefs.HasKey("rt_point_limit")){
      PlayerPrefs.SetInt("rt_point_limit", 50);
      PlayerPrefs.SetInt("rt_index", 1);
    }
	
	}
	
	// Update is called once per frame
	void Update(){

    if(Input.GetKeyDown(KeyCode.UpArrow) ||
       (!axis_held && Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") <= -0.9f)){
      --i;
      if(i < 0){
        i = menu_objects.Count - 1;
      }
      axis_held = true;
    }

    else if(Input.GetKeyDown(KeyCode.DownArrow) ||
            (!axis_held && Input.GetAxis(mac + "Controller 1 Left Stick Y Axis") >= 0.9f)){
      ++i;
      if(i >= menu_objects.Count){
        i = 0;
      }
      axis_held = true;
    }

    if(Input.GetButtonDown(mac + "Controller 1 A Button") ||
       Input.GetButtonDown(mac + "Controller 2 A Button") ||
       Input.GetButtonDown(mac + "Controller 3 A Button") ||
       Input.GetButtonDown(mac + "Controller 4 A Button") ||
       Input.GetKeyDown(KeyCode.Return)){

      if(i == 0){

        PlayerPrefs.SetInt("P1Num", -2);
        PlayerPrefs.SetInt("P2Num", -2);
        PlayerPrefs.SetInt("P3Num", -2);
        PlayerPrefs.SetInt("P4Num", -2);
        PlayerPrefs.SetString("P1", "none");
        PlayerPrefs.SetString("P2", "none");
        PlayerPrefs.SetString("P3", "none");
        PlayerPrefs.SetString("P4", "none");

        if(PlayerPrefs.GetString("screen") == "MONITOR"){
          PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
          SceneManager.LoadScene("_character_select");
        }
        else{
          PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
          SceneManager.LoadScene("_character_select_tabletop");
        }
      }
      if(i == 1){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_options");
      }
      if(i == 2){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_controls");
      }
      if(i == 3){
        PlayerPrefs.SetFloat("audio_time", camera.GetComponent<AudioSource>().time);
        SceneManager.LoadScene("_credits");
      }

    }

    if(Input.GetAxisRaw(mac + "Controller 1 Left Stick Y Axis") == 0){
      axis_held = false;
    }

    foreach(Text menu_GO in menu_objects){
      menu_GO.color = white;
    }
    menu_objects[i].color = red;
	
	}
}
