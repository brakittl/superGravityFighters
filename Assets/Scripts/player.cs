using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour{
    
  // ==[variables]==============================================================
  // ===========================================================================

    public string mac = "";

    // endgame
    public bool gameOver;

  	// player information
  	public int player_number;
  	public string player_color;

  	// gamemode specific info: survival 
  	public int lives = 3;
  	public bool dead = false;
    public float deathTime = 0;

  	// gamemode specific info: rt (reverse tag)
  	public int rt_points = 0;
  	public float rt_total_time = 0; // total time holding the "gem"
  	public float rt_longest_continuous_hold = 0; // longest time holding the "gem" without losing it
  	public float hit_by_pulse_times = 0;
    float pulse_hit_count = 0;
	public bool five_points_left, ten_points_left, twenty_points_left, thirty_points_left;
    // point limit is in level script, call with level.S.rt_point_limit

  	// orientation
  	public enum orientation {up, down, left, right};
  	public orientation player_orientation;

  	// animation elements
  	Animator player_animator;
  	public Animator slash_animator;
  	public Animator side_slash_animator;
  	public Animator up_slash_animator;
  	public Animator down_slash_animator;
  	public Animator shield_animator;

  	// slashes
  	public GameObject slash;
  	public GameObject side_slash;
  	public GameObject up_slash;
  	public GameObject down_slash;
  	public GameObject shield;
    public GameObject poisonGO;
    public GameObject halo;

  	// movement information
  	Rigidbody2D body;
  	public float speed, thrust, acceleration;
  	float run_speed, jump_speed;
  	public int grounded;

  	// movement bools
  	bool move_left = false;
  	bool move_right = false;
  	bool move_up = false;
  	bool move_down = false;

  	// gravity
  	Vector2 right, left, down, up;
    public float terminal_velocity = 10f;
	bool tabletopGravity1stSpawn = false;

  	// sounds
  	AudioSource sound;
  	public AudioClip gunshot, block, death, swordSlash, shieldPulse, defeat;

    // bullet information
    public GameObject bullet, extraBullet;
  	GameObject bullet_instance;
  	public float shotVelocity = 3f, numBullets = 1, bulletLimit = 3;
    float fireRate = 0.5f;
  	float nextFire = 0f, bulletCreationDist = 0.25f;
  	string lastDirection = "right";

  	// respawns
  	public bool respawn = false, respawning = false, dying = false;
  	Vector3 offscreen = new Vector3(-1000, -1000, -1000);  

  	// poison
  	// float poisonSpeed = 0.75f, poisonJump = 8f, poisonTime, poisonRate = 10;
  	public int poisonButtonTaps = 10, curButtonTaps;
  	public bool poisoned = false;

    // tracking statistics
    public int gravitySwapCount = 0, totalPoisoned = 0, numBulletShots = 0,
    numBulletHits = 0, numSwordSwipes = 0, numSwordHits = 0, numBlocks = 0,
    steps = 0, longestLife = 0, shortestLife = 1000000, bulletPickUps = 0,
    airTime = 0, longestAirTime = 0, borderSwaps = 0, suicides = 0, steals = 0;
    public bool firstTouch = false;
    float lastDeath, curAirTime;
  	public List<String> playersKilled;
    bool moving = false, airStart = false;

    // blocking
    bool swipeBlock = true;
    public bool invincible = false;
    float swipeBlockStart = 0f, swipeBlockTime = 0.25f, 
    invincibleStart = 0f, invincibleTime = 0.3f;
    // float delay = 0;

    // ui
    public Sprite[] bullet_sprites;
    public Sprite[] hearts_skulls;
    public GameObject[] hearts;
    public GameObject bullet_image;
    public GameObject rt_text_GO;
    public Text bullet_text;
    public Text rt_text;
    public bool ui_found;

    // medals earned
    public List<String> medals = new List<string>();

    // colors
    Color red = new Color();
    Color orange = new Color();
    Color yellow = new Color();
    Color green = new Color();
    Color blue = new Color();
    Color purple = new Color();
    Color black_ui = new Color();
    Color black = new Color();
    public Dictionary<string, Color> colors;

    // character select
    bool is_character_select;

  // ==[helper functions]=======================================================
  // ===========================================================================

    // get sprite by name
    public Sprite get_sprite_by_name(Sprite[]sprites, string sprite_name){
      for(int i = 0; i < sprites.Length; i++){
        if(sprites[i].name.Equals(sprite_name)){
          return sprites[i];
        }
      }
      return null;
    }

    void SetPlayerNumber(int sent_number){
      player_number = sent_number;
		if (level.S.tabletop)
		{
			if(sent_number == 4){
				Gravity("left", 0f, -90f, true);
			}
			else if(sent_number == 3){
				Gravity("up", 0f, 180f, true);
			}
			else if(sent_number == 2) {
				Gravity("right", 0f, 90f, true);
			}


		}
    }

    void SetGravityFromCharacterSelect(string orientation){
      if(orientation == "up"){
        Gravity("up", transform.localEulerAngles.y, 180f, true);
      }
      if(orientation == "down"){
        Gravity("down", -transform.localEulerAngles.y, 0f, true);
      }
      if(orientation == "left"){
        Gravity("left", 0f, -90f, true);
      }
      if(orientation == "right"){
        Gravity("right", 0f, 90f, true);
      }
    }

  // ==[start]==================================================================
  // ===========================================================================

  	void Start(){
      // Mac Check
		five_points_left = ten_points_left = twenty_points_left = thirty_points_left = false;

      if(Application.platform == RuntimePlatform.OSXEditor
        || Application.platform == RuntimePlatform.OSXPlayer
        || Application.platform == RuntimePlatform.OSXPlayer){
        mac = "Mac ";
      }
      else{
        mac = "";
      }

      if(SceneManager.GetActiveScene().name == "_character_select" ||
         SceneManager.GetActiveScene().name == "_character_select_tabletop"){
        is_character_select = true;
      }
      else{
        is_character_select = false;
      }

      // initialize variables
      gameOver = false;
      grounded = 0;

      // colors
      ColorUtility.TryParseHtmlString("#c04342", out red);
      ColorUtility.TryParseHtmlString("#cd5c19", out orange);
      ColorUtility.TryParseHtmlString("#c59522", out yellow);
      ColorUtility.TryParseHtmlString("#45a349", out green);
      ColorUtility.TryParseHtmlString("#2c5d99", out blue);
      ColorUtility.TryParseHtmlString("#854db5", out purple);
      ColorUtility.TryParseHtmlString("#3f3f45", out black);
      ColorUtility.TryParseHtmlString("#ebebeb", out black_ui);
      colors = new Dictionary<string, Color>(){
        {"red", red},
        {"orange", orange},
        {"yellow", yellow},
        {"green", green},
        {"blue", blue},
        {"purple", purple},
        {"black", black},
        {"black_ui", black_ui},
      };

      // get components
  		player_animator = GetComponent<Animator>();
  		body = gameObject.GetComponent<Rigidbody2D>();

      // gravity
  		player_orientation = orientation.down;
  		body.gravityScale = 0;   

      // slashes 
  		slash.GetComponent<BoxCollider2D>().enabled = false;
  		side_slash.GetComponent<BoxCollider2D>().enabled = false;
  		up_slash.GetComponent<BoxCollider2D>().enabled = false;
  		down_slash.GetComponent<BoxCollider2D>().enabled = false;
  		shield.GetComponent<CircleCollider2D>().enabled = false;
  		sound = GetComponent<AudioSource>();
  		jump_speed = thrust;
  		run_speed = speed;

      // get sprites
      hearts_skulls = Resources.LoadAll<Sprite>("general/hearts_skulls");
      bullet_sprites = Resources.LoadAll<Sprite>("_player_" + player_color.ToLower() + "/" + player_color.ToLower() + "_bullet");

      // test if ui element is present
      GameObject ui = GameObject.Find("ui");
      if(ui != null){
        ui_found = true;
      }
      else{
        ui_found = false;
      }

      if(ui_found){

        // get bullet information
        string bullet_image_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_bullet";
        if(PlayerPrefs.GetString("screen") == "TABLETOP"){
          bullet_image_string = "tabletop_" + bullet_image_string;
        }
        bullet_image = GameObject.Find(bullet_image_string);
        bullet_image.GetComponent<Image>().sprite = get_sprite_by_name(bullet_sprites, player_color.ToLower() + "_bullet_1");
        string bullet_text_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_bullet_text";
        if(PlayerPrefs.GetString("screen") == "TABLETOP"){
          bullet_text_string = "tabletop_" + bullet_text_string;
        }
        bullet_text = GameObject.Find(bullet_text_string).GetComponent<Text>();
        bullet_image.SetActive(true);
        GameObject.Find(bullet_text_string).SetActive(true);

        // get rt score text
        string rt_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_rt_score";
        if(PlayerPrefs.GetString("screen") == "TABLETOP"){
          rt_string = "tabletop_" + rt_string;
        }
        rt_text_GO = GameObject.Find(rt_string);
        rt_text = rt_text_GO.GetComponent<Text>();

        if(level.S.gamemode != GameMode.REVERSE_TAG){
          rt_text_GO.SetActive(false);
        }
        else{
          rt_text_GO.SetActive(true);
        }

        lives = PlayerPrefs.GetInt("lives");

        // initialize hearts/skulls
        hearts = new GameObject[10];
        for(int i = 0; i < 10; ++i){
          string heart_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_" + (i + 1).ToString();
          if(PlayerPrefs.GetString("screen") == "TABLETOP"){
            heart_string = "tabletop_" + heart_string;
          }
          GameObject current_heart = GameObject.Find(heart_string);
          if(current_heart != null){
            if(level.S.gamemode == GameMode.SURVIVAL){
              current_heart.GetComponent<Image>().sprite = get_sprite_by_name(hearts_skulls, player_color.ToLower() + "_heart");
            }
            else{
              current_heart.SetActive(false);
            }
          }
          hearts[i] = current_heart;
        }
        

      }

        
    }

  // ==[updates]================================================================
  // ===========================================================================

  	void Update(){


		if (!tabletopGravity1stSpawn)
		{
			tabletopGravity1stSpawn = true;
			if (level.S.tabletop)
			{
				if(player_number == 4){
					Gravity("left", 0f, -90f, false);
				}
				else if(player_number == 3){
					Gravity("up", 0f, 180f, false);
				}
				else if(player_number == 2) {
					Gravity("right", 0f, 90f, false);
				}



			}

			Vector3 respawn_position = transform.position;
			Vector3 circle_position = new Vector3(respawn_position.x - 0.02f, respawn_position.y + 0.01f, 0f);
			Instantiate(Resources.Load<GameObject>("__Prefabs/_respawn_circles/respawn_circle_" + player_color.ToLower()), circle_position, transform.rotation);
			player_animator.Play("Appear");
		}


      // ==[win conditions]=====================================================
      // =======================================================================
      
      // gameOver check to disable character control
      if(gameOver){
      if(!dead) deathTime = Time.time;
        return;
      }

      // reverse tag point count check
      if(level.S.gamemode == GameMode.REVERSE_TAG){
        if(rt_points >= level.S.rt_point_limit){
          level.S.ranking.Add(this);
        }
      }

      // deathmatch kill count of 10
      if(level.S.gamemode == GameMode.DEATHMATCH){
        if(playersKilled.Capacity >= 10){
          level.S.ranking.Add(this);
        }
      }

      // ==[ui management]======================================================
      // =======================================================================

      if(ui_found){

        bullet_text.text = "x " + numBullets.ToString();

        // survival ui
        if(level.S.gamemode == GameMode.SURVIVAL){
          for(int i = 1; i <= lives; ++i){
            hearts[i - 1].SetActive(true);
          }
          for(int i = lives + 1; i <= 10; ++i){
            hearts[i - 1].SetActive(false);
          }
        }

        // deathmatch ui
        else if(level.S.gamemode == GameMode.DEATHMATCH){
          int i = 0;
          foreach(string player_string in playersKilled){
            Debug.Log(player_string);
            // char[] delimiters = {'_'};
            // string[] splits = player_string.Split(delimiters);
            hearts[i].SetActive(true);
            // hearts[i].GetComponent<Image>().sprite = get_sprite_by_name(hearts_skulls, splits[0] + "_skull");
            hearts[i].GetComponent<Image>().sprite = get_sprite_by_name(hearts_skulls, player_color.ToLower() + "_skull");
            ++i;
          }
          for(; i < 10; ++i){
            hearts[i].SetActive(false);
          }
        }

        // reverse tag ui
        else if(level.S.gamemode == GameMode.REVERSE_TAG){
          rt_text.text = rt_points.ToString();
          rt_text.color = colors[player_color.ToLower()];
          if(player_color == "Black"){
            rt_text.color = colors[player_color.ToLower() + "_ui"];
          }
        }

      }

      // ==[pausing]============================================================
      // =======================================================================

    
      if(Input.GetButtonDown(mac + "Controller " + player_number + " Start Button") && !level.S.pause){
        level.S.pause = true;
      }
      else if(Input.GetButtonDown(mac + "Controller " + player_number + " Start Button") && level.S.pause){
        level.S.pause = false;
      }

      // ==[gravity swap]=======================================================
      // =======================================================================

      // gravity vectors
      right = new Vector2(acceleration, 0f);
  		left = new Vector2(-acceleration, 0f);
  		down = new Vector2(0f, -acceleration);
  		up = new Vector2(0f, acceleration);
  		
      // swap gravity orientation
  		if(!poisoned && !dead && !dying){
  			// up
  			if(((Input.GetAxis(mac + "Controller " + player_number + " Right Stick Y Axis") < -0.4f && Math.Abs(Input.GetAxis(mac + "Controller " + player_number + " Right Stick X Axis")) < 0.4f) || Input.GetButtonDown(mac + "Controller " + player_number + " Y Button") || Input.GetKey(KeyCode.W))){
  				SwapGravity("up");
          gravitySwapCount++;
        }
  			// down
  			if(((Input.GetAxis(mac + "Controller " + player_number + " Right Stick Y Axis") > 0.4f && Math.Abs(Input.GetAxis(mac + "Controller " + player_number + " Right Stick X Axis")) < 0.4f) || Input.GetButtonDown(mac + "Controller " + player_number + " A Button") || Input.GetKey(KeyCode.S))){
  				SwapGravity("down");
          gravitySwapCount++;

        }
  			// left
  			if(((Input.GetAxis(mac + "Controller " + player_number + " Right Stick X Axis") < -0.4f && Math.Abs(Input.GetAxis(mac + "Controller " + player_number + " Right Stick Y Axis")) < 0.4f) || Input.GetButtonDown(mac + "Controller " + player_number + " X Button") || Input.GetKey(KeyCode.A))){
  				SwapGravity("left");
          gravitySwapCount++;

        }
  			// right
  			if(((Input.GetAxis(mac + "Controller " + player_number + " Right Stick X Axis") > 0.4f && Math.Abs(Input.GetAxis(mac + "Controller " + player_number + " Right Stick Y Axis")) < 0.4f) || Input.GetButtonDown(mac + "Controller " + player_number + " B Button") || Input.GetKey(KeyCode.D))){
  				SwapGravity("right");
          gravitySwapCount++;

        }
  		}

  		// ==[actions]============================================================
  		// =======================================================================

  		// if alive, allow attack, shoot, and block action
  		if(!dead && !dying){
  			if(level.S.gamemode != GameMode.REVERSE_TAG){
          
          // attack
          // Input.GetAxis("Controller " + player_number + " Both Triggers") < -0.25f || Input.GetAxis(mac + "Controller " + player_number + " Right Trigger") > 0.25f || 
          if((Input.GetButtonDown(mac + "Controller " + player_number + " Right Bumper") ||
              Input.GetKey(KeyCode.Space)) && Time.time > nextFire){
            numSwordSwipes++;
  				  Block();
  			  }

          // shoot
          // Input.GetAxis("Controller " + player_number + " Both Triggers") > 0.25f || Input.GetAxis(mac + "Controller " + player_number + " Left Trigger") > 0.25f || 
          else if((Input.GetButtonDown(mac + "Controller " + player_number + " Left Bumper") ||
                   Input.GetKey(KeyCode.Q)) && Time.time > nextFire){
            Shoot();
  			  }
  			}
  		}

  		// if dead, allow poison action
      // else{
      //   GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); //Ghost Body
      //   // poison
      //   if((Input.GetAxis(mac + "Controller " + player_number + " Right Bumper") >= 0.9 || Input.GetKey(KeyCode.LeftShift)) && Time.time > poisonTime){
      //   	Poison();
      //   }
      //   if((Input.GetButtonDown(mac + "Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.LeftShift)) && Time.time > poisonTime){
      // 		Attack();
      // 	}
      // }

      // ==[movement bools]=====================================================
      // =======================================================================
      
      // movement tracking
      if(moving && player_animator.GetBool("grounded")){
        steps++;
        moving = false;
      }

      // airtime tracking
      if(!player_animator.GetBool("grounded") && !airStart ){
        curAirTime = Time.time;
        airStart = true; 
      }
      else if(player_animator.GetBool("grounded")  && airStart){
        if((int)((Time.time - curAirTime) * 100) > longestAirTime){
          longestAirTime = (int)((Time.time - curAirTime) * 100);
        }
        airTime += (int)((Time.time - curAirTime) * 100);
        airStart = false;
      }

      // reset movement bools
      move_left = false;
  		move_right = false;
  		move_up = false;
  		move_down = false;

  		// move right
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick X Axis") >= 0.9f || Input.GetKey(KeyCode.RightArrow)){
        moving = true;
        if(player_orientation == orientation.up){
  				move_left = true;
  			}
  			else if(player_orientation == orientation.left){
  				move_up = true;
  			}
  			else if(player_orientation == orientation.right){
  				move_down = true;
  			}
  			else{
  				move_right = true;
  			}
  		}

  		// move left
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick X Axis") <= -0.9f || Input.GetKey(KeyCode.LeftArrow)){
        moving = true;
        if(player_orientation == orientation.up){
  				move_right = true;
  			}
  			else if(player_orientation == orientation.left){
  				move_down = true;
  			}
  			else if(player_orientation == orientation.right){
  				move_up = true;
  			}
  			else{
  				move_left = true;
  			}
  		}

  		// move up
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick Y Axis") <= -0.9f || Input.GetKey(KeyCode.UpArrow)){
        moving = true;
        if(player_orientation == orientation.up){
  				move_down = true;
  			}
  			else if(player_orientation == orientation.left){
  				move_left = true;
  			}
  			else if(player_orientation == orientation.right){
  				move_right = true;
  			}
  			else{
  				move_up = true;
  			}
  		}

  		// move down
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick Y Axis") >= 0.9f || Input.GetKey(KeyCode.DownArrow)){
        moving = true;
        if(player_orientation == orientation.up){
  				move_up = true;
  			}
  			else if(player_orientation == orientation.left){
  				move_right = true;
  			}
  			else if(player_orientation == orientation.right){
  				move_left = true;
  			}
  			else{
  				move_down = true;
  			}
  		}

      // shift movement bools for tabletop mode
      TabletopMovementShift();

      // set last direction based on movement bools
      SetLastDirection();

      // ==[resets]=============================================================
      // =======================================================================

      if(!move_left && !move_right){
        player_animator.SetBool("run", false);
      }
      else if(player_animator.GetBool("jump")){
        player_animator.SetBool("run", false);
      }

      if(!player_animator.GetBool("attack")){
        slash.GetComponent<BoxCollider2D>().enabled = false;
        side_slash.GetComponent<BoxCollider2D>().enabled = false;
        up_slash.GetComponent<BoxCollider2D>().enabled = false;
        down_slash.GetComponent<BoxCollider2D>().enabled = false;
      }
      
      // ==[crouch]=============================================================
      // =======================================================================

  		player_animator.SetBool("crouched", false);
  		if(move_down){
        if(!dying){
  			 Crouch();
        }
  		}
      
  		// ==[respawn and death]==================================================
  		// =======================================================================

  		if(!player_animator.GetBool("block")){
  			shield.GetComponent<PolygonCollider2D>().enabled = false;
  		}

  		// poison effect
      // if(poisoned){

      // 	// reduce jump and speed
      // 	thrust = poisonJump;
      // 	speed = poisonSpeed;

      // 	// remove poison with button tap
      // 	if((Input.GetKeyDown(KeyCode.LeftShift) || 
      //       Input.GetButtonDown(mac + "Controller " + player_number + " A Button") ||
      //       Input.GetButtonDown(mac + "Controller " + player_number + " B Button") || 
      //       Input.GetButtonDown(mac + "Controller " + player_number + " X Button") || 
      //       Input.GetButtonDown(mac + "Controller " + player_number + " Y Button")) && !dead){
      // 		curButtonTaps++;
      //    poisonGO.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, (1 - 0.1f * curButtonTaps));
      // 	}

      // 	// remove poison after 10 taps
      // 	if(!dead && curButtonTaps == poisonButtonTaps){
      // 		poisoned = false;
      // 		thrust = jump_speed;
      // 		speed = run_speed;
      // 		curButtonTaps = 0;
      //     poisonGO.SetActive(false);
      // 	}
      // }

      // block
      if(!swipeBlock && Time.time - swipeBlockStart > swipeBlockTime){
        swipeBlock = true;
        body.velocity = new Vector2(0f, 0f);
      }

      // invincibility
      if(invincible && Time.time > invincibleStart){
        invincible = false;
      }

      // set invincible if in character_select
      if(is_character_select){
        invincible = true;
      }
    }

  	void FixedUpdate(){

		  float speed = body.velocity.magnitude;

      // ==[run]================================================================
      // =======================================================================
  		
      if(!dying){

        if(move_right && !player_animator.GetBool("landing")){
    			Run(true);
    		}
    		if(move_left && !player_animator.GetBool("landing")){
    			Run(false);
    		}

      }

      // ==[grounded]===========================================================
      // =======================================================================

      if(!checkGround()){
        grounded = 1;
        player_animator.SetBool("grounded", true);
      }
      else{
        grounded = 0;
        player_animator.SetBool("grounded", false);
        if(!player_animator.GetBool("swapping") && player_number > 0 && !dying){
          player_animator.Play("Falling");
        }
      }

  		// ==[gravity force]======================================================
  		// =======================================================================

      if(!dying){
    		if(player_orientation == orientation.down){
    			if(speed < terminal_velocity){
    			  body.AddForce(down);
          }
    		}
    		else if(player_orientation == orientation.up){
    			if(speed < terminal_velocity){
            body.AddForce(up);
          }
    		}
    		else if(player_orientation == orientation.left){
    			if(speed < terminal_velocity){
            body.AddForce(left);
          }
    		}
    		else if(player_orientation == orientation.right){
    			if(speed < terminal_velocity){
            body.AddForce(right);
          }
    		}
      }

  		// ==[jump]===============================================================
  		// =======================================================================

      // if(!dying){
      // 	if(move_up && grounded == 1 && (delay < 0)){
      // 		Jump();
      // 	}
      // }
  	}

  // ==[raycasts]===============================================================
  // ===========================================================================

  	bool checkSides(){

      // offsets
  		float bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  		float bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  		float player_length = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;

  		// rotate the ray based on orientation
      switch (player_orientation){
  		case orientation.down:
  			if(transform.rotation.y == 0){
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			else {
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			break;
  		case orientation.up:
  			if(transform.rotation.y == 0){
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			else {
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			break;
  		case orientation.left:
  			if(transform.rotation.y == 0){
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			else {
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			break;
  		case orientation.right:
  			if(transform.rotation.y == 0){
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			else {
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			break;
  		default:
  			print("Hey, why isn't the orientation set?");
  			break;
  		}

  		float length_ray_leftright = (player_length * .9F);
  		Vector2 left = transform.TransformDirection(new Vector2(length_ray_leftright, 0));

      // layers raycast should ignore
  		LayerMask ignoreplayer_layerMask = ~(LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Border") | LayerMask.NameToLayer("TagBall") | LayerMask.NameToLayer("Attack")) ;
  		ignoreplayer_layerMask = ~ignoreplayer_layerMask;

  		if(player_orientation == orientation.up || player_orientation == orientation.down){  
  			// draw raycast
        Debug.DrawRay(new Vector2(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  + (player_length / 2)), left, Color.green);
  			Debug.DrawRay(new Vector2(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  - (player_length / 2)), left, Color.green);
  			// actual raycast
        return(!Physics2D.Raycast(new Vector3(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  + (player_length / 2)),left,length_ray_leftright,ignoreplayer_layerMask) && 
  				     !Physics2D.Raycast(new Vector3(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  - (player_length / 2)),left,length_ray_leftright, ignoreplayer_layerMask));
  		}

  		else {
  			length_ray_leftright = (player_length * 1.7F);
  			left = transform.TransformDirection(new Vector2(length_ray_leftright, 0));
  			// draw raycast
        Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x  + (player_length / 2), transform.position.y + bc_offset_y), left, Color.green);
  			Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x  - (player_length / 2), transform.position.y + bc_offset_y), left, Color.green);
  			// actual raycast
        return(!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x  + (player_length / 2), transform.position.y + bc_offset_y),left,length_ray_leftright,ignoreplayer_layerMask) && 
  				     !Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x  - (player_length / 2), transform.position.y + bc_offset_y),left,length_ray_leftright, ignoreplayer_layerMask));
  		}
  	}
      
  	bool checkGround(){

      // offsets
  		float bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  		float bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  		float player_length = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
  		float player_height = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;

  		// rotate the ray based on orientation
      switch (player_orientation){
  		case orientation.down:
  			if(transform.rotation.y == 0){
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			else{
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			break;
  		case orientation.up:
  			if(transform.rotation.y == 0){
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			else{
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  			}
  			break;
  		case orientation.left:
  			if(transform.rotation.y == 0){
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.y * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.y;
  			}
  			else{
  				bc_offset_x = GetComponent<BoxCollider2D>().offset.y * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.x * transform.localScale.y;
  			}
  			break;
  		case orientation.right:
  			if(transform.rotation.y == 0){
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.x;
  				bc_offset_y = GetComponent<BoxCollider2D>().offset.x * transform.localScale.y;
  			}
  			else{
  				bc_offset_x = -GetComponent<BoxCollider2D>().offset.y * transform.localScale.x;
  				bc_offset_y = -GetComponent<BoxCollider2D>().offset.x * transform.localScale.y;
  			}
  			break;
  		default:
  			print("Hey, why isn't the orientation set?");
  			break;
  		}

  		float length_ray_updw = (player_height / 2) + (player_height * .2F);
  		Vector2 below = transform.TransformDirection(new Vector2(0F, -length_ray_updw));

      // layers raycast should ignore// layers raycast should ignore
  		LayerMask ignoreplayer_layerMask = ~(LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Border") | LayerMask.NameToLayer("TagBall") | LayerMask.NameToLayer("Attack"));
  		ignoreplayer_layerMask = ~ignoreplayer_layerMask;

      // offset compared to box collider
      float ray_offset = 0.011F;

  		if(player_orientation == orientation.up || player_orientation == orientation.down){ 
  			// draw raycast
        Debug.DrawRay(new Vector2(transform.position.x + (ray_offset + player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y), below, Color.green);
			  Debug.DrawRay(new Vector2(transform.position.x - (ray_offset + player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y), below, Color.green);
        // actual raycast
        return(!Physics2D.Raycast(new Vector3(transform.position.x + (ray_offset + player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask) && 
				       !Physics2D.Raycast(new Vector3(transform.position.x - (ray_offset + player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw, ignoreplayer_layerMask));
  		}
  		else{
        // draw raycast
  			Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x, transform.position.y + (ray_offset + player_length / 2) + bc_offset_y), below, Color.green);
  			Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x, transform.position.y  - (ray_offset + player_length / 2) + bc_offset_y), below, Color.green);
  			// actual raycast
        return(!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x, transform.position.y + (ray_offset + player_length / 2) + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask) && 
  				     !Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x, transform.position.y - (ray_offset + player_length / 2) + bc_offset_y),below,length_ray_updw, ignoreplayer_layerMask));
  		}
  	}

  // ==[gravity]================================================================
  // ===========================================================================

  	public void Gravity(string new_orientation, float y_rot, float z_rot, bool char_select){

      // set orientation
      if(new_orientation == "up"){
        player_orientation = orientation.up;
      }
      else if(new_orientation == "left"){
        player_orientation = orientation.left;
      }
      else if(new_orientation == "right"){
        player_orientation = orientation.right;
      }
      else{
        player_orientation = orientation.down;
      }

      // causes error on character_select screen
      if(!char_select){
        body.velocity = new Vector2(0f, 0f); // set velocity to zero at swap initializaiton
      }

  		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y_rot, z_rot); // rotate for new orientation
  		player_animator.Play("Swap"); // play animation
  	}

  // ==[movement functions]=====================================================
  // ===========================================================================

  	void Crouch(){
  		if(player_animator.GetBool("grounded")){
  			player_animator.SetBool("crouched", true);
  			player_animator.Play("Down");
  		}
  	}

  	void Run(bool right){

      // check if not attacking or blocking
  		bool can_move = true;
  		can_move = !(player_animator.GetBool("attack") && grounded == 1);
  		can_move = can_move && !(player_animator.GetBool("block") && grounded == 1);

  		if(can_move){

        // initialize rotation and flip
  			float rotation = 0f;
  			float flip = 1f;

        // if up or down
  			if(player_orientation == orientation.up || player_orientation == orientation.down){
  				if(!right){
  					rotation = 180f;
  				}
  				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotation, transform.localEulerAngles.z);
  			}

        // if right or left
  			else{
  				if(!right){
  					rotation = 180f;
  					if(player_orientation == orientation.right){
  						flip = -1f;
  					}
  				}
  				else if(player_orientation == orientation.left){
  					flip = -1f;
  				}
  				transform.localEulerAngles = new Vector3(0f, rotation, 90f * flip);
  			}

        // apply movement if not crouched or side raycasts are clear
  			if(!player_animator.GetBool("crouched") && checkSides()){
  				transform.localPosition += transform.right * speed * Time.deltaTime;
  			}

        // play running animation if on ground
  			if(player_animator.GetBool("grounded")){
  				player_animator.SetBool("run", true);
  			}
  		}
  	}

    void Jump(){
      player_animator.Play("Jump");
      player_animator.SetBool("run", false);
      player_animator.SetBool("jump", true);
      body.AddForce(transform.up * thrust);   
    }

  // ==[actions]================================================================
  // ===========================================================================

  	void Attack(){

  		if(!player_animator.GetBool("attack") && !respawn && !player_animator.GetBool("crouched")){

        if(PlayerPrefs.GetFloat("sfx") != 0){
          sound.PlayOneShot(swordSlash);
        }

        numSwordSwipes++; // statistics count
        nextFire = Time.time + fireRate;
        player_animator.SetBool("attack", true);

  			if(!player_animator.GetBool("grounded")){
  				if(move_left || move_right){
  					player_animator.Play("Side_Attack");
  					side_slash_animator.Play("Slash");
  					side_slash.GetComponent<BoxCollider2D>().enabled = true;
  				}
  				else if(move_down){
  					player_animator.Play("Down_Attack");
  					down_slash_animator.Play("Slash");
  					down_slash.GetComponent<BoxCollider2D>().enabled = true;
  				}
  				else if(move_up){
  					player_animator.Play("Up_Attack");
  					up_slash_animator.Play("Slash");
  					up_slash.GetComponent<BoxCollider2D>().enabled = true;
  				}
  				else{
  					player_animator.Play("Attack");
  					slash_animator.Play("Slash");
  					slash.GetComponent<BoxCollider2D>().enabled = true;
  				}
  			}
  			else{
  				player_animator.Play("Attack");
  				slash_animator.Play("Slash");
  				slash.GetComponent<BoxCollider2D>().enabled = true;
  			}
  		}
  	}

  	void Shoot(){

      if(numBullets > 0){
    		if(PlayerPrefs.GetFloat("sfx") != 0){
          sound.PlayOneShot(gunshot);
        }

    		nextFire = Time.time + fireRate;
    		numBulletShots++;
        if(!is_character_select){
    		  numBullets--;
        }

    		Vector3 pos = transform.position, rot = transform.rotation.eulerAngles;
    		rot.x = 0;
    		rot.y = 0;

    		// bullet rotation
    		if(lastDirection == "up" && player_orientation == orientation.down ||
    			(lastDirection == "up" && player_orientation == orientation.up) ||
    			(lastDirection == "left" && player_orientation == orientation.left) ||
    			(lastDirection == "right" && player_orientation == orientation.right)){
    			rot.z = 90;
    		}
    		else if(lastDirection == "left" && player_orientation == orientation.down ||
    			(lastDirection == "right" && player_orientation == orientation.up) ||
    			(lastDirection == "down" && player_orientation == orientation.left) ||
    			(lastDirection == "up" && player_orientation == orientation.right)){
    			rot.z = 180;
    		}
    		else if(lastDirection == "down" && player_orientation == orientation.down ||
    			(lastDirection == "down" && player_orientation == orientation.down) ||
    			(lastDirection == "right" && player_orientation == orientation.left) ||
    			(lastDirection == "left" && player_orientation == orientation.right)){
    			rot.z = 270;
    		}
    		else if((lastDirection == "down" && player_orientation == orientation.down) ||
    			(lastDirection == "left" && player_orientation == orientation.up) ||
    			(lastDirection == "up" && player_orientation == orientation.left) ||
    			(lastDirection == "down" && player_orientation == orientation.right)){
    			rot.z = 0;
    		}

    		// bullet initiation
    		if((lastDirection == "right" && player_orientation == orientation.down) || 
    			(lastDirection == "left" && player_orientation == orientation.up) ||
    			(lastDirection == "down" && player_orientation == orientation.right) ||
    			(lastDirection == "up" && player_orientation == orientation.left)){
    			pos.x = transform.position.x + bulletCreationDist;
    			bullet_instance = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
    			bullet_instance.GetComponent<Rigidbody2D>().velocity = Vector3.right * shotVelocity;
    		}
    		else if((lastDirection == "left" && player_orientation == orientation.down) || 
    			(lastDirection == "right" && player_orientation == orientation.up) ||
    			(lastDirection == "up" && player_orientation == orientation.right) ||
    			(lastDirection == "down" && player_orientation == orientation.left)){
    			pos.x = transform.position.x - bulletCreationDist;
    			bullet_instance = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
    			bullet_instance.GetComponent<Rigidbody2D>().velocity = Vector3.left * shotVelocity;
    		}
    		else if((lastDirection == "up" && player_orientation == orientation.down) ||
    			(lastDirection == "down" && player_orientation == orientation.up) ||
    			(lastDirection == "right" && player_orientation == orientation.right) ||
    			(lastDirection == "left" && player_orientation == orientation.left)){
    			pos.y = transform.position.y + bulletCreationDist;
    			bullet_instance = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
    			bullet_instance.GetComponent<Rigidbody2D>().velocity = Vector3.up * shotVelocity;
    		}
    		else if((lastDirection == "down" && player_orientation == orientation.down) || 
    			(lastDirection == "up" && player_orientation == orientation.up) ||
    			(lastDirection == "left" && player_orientation == orientation.right) ||
    			(lastDirection == "right" && player_orientation == orientation.left)){
    			pos.y = transform.position.y - bulletCreationDist;
    			bullet_instance = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
    			bullet_instance.GetComponent<Rigidbody2D>().velocity = Vector3.down * shotVelocity;
    		}
      }
  	}

  	void Block(){
      if(PlayerPrefs.GetFloat("sfx") != 0){
        sound.PlayOneShot(shieldPulse);
      }
      nextFire = Time.time + fireRate;
      invincibleStart = Time.time + invincibleTime;
      invincible = true;
      player_animator.Play("Attack");
  		player_animator.SetBool("block", true);
  		shield_animator.Play("Shield");
  	}

    void SuperSlash(){
      down_slash_animator.Play("Slash");
      up_slash_animator.Play("Slash");
      slash_animator.Play("Slash");
      player_animator.Play("Attack");
      slash.GetComponent<BoxCollider2D>().enabled = true;
      up_slash.GetComponent<BoxCollider2D>().enabled = true;
      down_slash.GetComponent<BoxCollider2D>().enabled = true;
    }

  	void Poison(){
      poisoned = true;
      curButtonTaps = 0;
      poisonGO.SetActive(true);
      poisonGO.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
  	}

  // ==[death helper functions]=================================================
  // ===========================================================================

  	public void FindKiller(GameObject collideObject, bool bulletAttack){

      string killer_color = "none";

      // loop through all players to find who killed this gameObject
  		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
  		foreach(GameObject p in players){

  			player other = (player)p.GetComponent(typeof(player));

        // check if bullet killed player
        if(bulletAttack && other.bullet_instance == collideObject){
          other.numBulletHits++;
          // other player is not this player
          if(this.name != other.name){
            other.playersKilled.Add(this.gameObject.name);
            killer_color = other.player_color;
          }
          // else, suicide
          else{
            suicides++;
            killer_color = gameObject.GetComponent<player>().player_color;
          }  
        }

        // check if burst attack killed this gameObject
        else if(!bulletAttack && other.shield == collideObject){
          // don't kill if the other player is currently dying
          if(other.dying){
            return;
          }
	        other.playersKilled.Add(this.gameObject.name);
          other.numSwordHits++;
          killer_color = other.player_color;
        }
  		}

      // kill player with correct slash color
      KillPlayer(killer_color);
  	}

  	public void KillPlayer(string killer_color){
      if(PlayerPrefs.GetFloat("sfx") != 0){
        sound.PlayOneShot(death);
        sound.PlayOneShot(swordSlash);
      }

      // reset colliders
      respawn = true;
      slash.GetComponent<BoxCollider2D>().enabled = false;
  	  side_slash.GetComponent<BoxCollider2D>().enabled = false;
  	  up_slash.GetComponent<BoxCollider2D>().enabled = false;
  	  down_slash.GetComponent<BoxCollider2D>().enabled = false;

      // decrement lives
      lives--;
        if(lives <= 0 && level.S.numPlayers - level.S.ranking.Count - 1 <= 1)
        {
            level.S.lastKill(this, colors[killer_color.ToLower()], 0.4f);
            sound.PlayOneShot(defeat, 5.5f);
        }
        else if (lives <= 0)
        {
            level.S.KillPause(transform.position, colors[killer_color.ToLower()], !is_character_select, 0.05f);
            sound.PlayOneShot(defeat, 5.5f);
        }
        else
            level.S.KillPause(transform.position, colors[killer_color.ToLower()], !is_character_select, 0f);

      // turn off poison
      poisoned = false;
      poisonGO.SetActive(false);
      thrust = jump_speed;
      speed = run_speed;

      // reset bullet count
      numBullets = 1;

      // play death animation
      player_animator.Play("Death");

      // remove from camera tracking
      level.S.alive_players.Remove(gameObject);

      // life length statistics
      if(Time.time - lastDeath > longestLife){
        longestLife = (int)((Time.time - lastDeath) *100);
      }
      if(Time.time - lastDeath < shortestLife){
        shortestLife = (int)((Time.time - lastDeath) * 100);
      }
      lastDeath = Time.time;
      
      // reset
      body.velocity = new Vector2(0f, 0f);
  	  StartCoroutine(Wait());
  	}

  	IEnumerator Wait(){

      Time.timeScale = 0.1f;

      // dying animation
      dying = true;
      body.velocity = new Vector3(0f, 0f, 0f);
      yield return new WaitForSeconds(0.3f);
        dying = false;
      // set position off screen
      Vector3 pos = transform.position;


       transform.position = offscreen;

  		
      Gravity("down", -transform.localEulerAngles.y, 0f, false);

      // instantiate bullet pickup if player has extra bullets
      if(level.S.gamemode == GameMode.SURVIVAL && numBullets > 0){
        Instantiate(extraBullet, pos, transform.rotation);
      }      

      // respawn after 1 second
      yield return new WaitForSeconds(1f);

    if(lives == 0 && (level.S.gamemode == GameMode.SURVIVAL)){
        dead = true;
        level.S.alive_players.Remove(this.gameObject);
        transform.position = offscreen;
        deathTime = Time.time;
        level.S.ranking.Add(this);
        Debug.Log("dead player");
      }

        if (!dead){
			Vector3 respawn_position = level.S.findRespawn(player_number);
          Vector3 circle_position = new Vector3(respawn_position.x - 0.02f, respawn_position.y + 0.01f, 0f);
          Instantiate(Resources.Load<GameObject>("__Prefabs/_respawn_circles/respawn_circle_" + player_color.ToLower()), circle_position, transform.rotation);
          transform.position = respawn_position;
          transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, 0f);
          body.velocity = new Vector2(0f, 0f);
          player_orientation = orientation.down;
			if (level.S.tabletop)
			{
				if(player_number == 4){
					Gravity("left", 0f, -90f, true);
				}
				else if(player_number == 3){
					Gravity("up", 0f, 180f, true);
				}
				else if(player_number == 2) {
					Gravity("right", 0f, 90f, true);
				}


			}
          level.S.alive_players.Insert(0, gameObject);
          player_animator.Play("Appear");
        }      
      respawning = true;
        respawn = false;
  	}

  	IEnumerator Blink(){
  		// transform.GetComponent<Renderer>().enabled = false;
  		yield return new WaitForSeconds(0.1f);
  		// transform.GetComponent<Renderer>().enabled = true;
  		yield return new WaitForSeconds(1f);
  		respawn = false;
  	}

  // ==[collisions and triggers]================================================
  // ===========================================================================
      
  	void OnTriggerEnter2D(Collider2D col){
      
      // slash triggers
      // if(col.tag == "slash" && !respawn && !dead && !invincible){
      //   player p = col.transform.parent.GetComponent<player>();
      //   if(p.dead){
      //     if(!poisoned){
      //       p.totalPoisoned++;
      //     }
      //     Poison();
      //     p.poisonTime = Time.time + poisonRate;
      //     return;
      //   }

      //   if(player_animator.GetBool("block") || player_animator.GetBool("attack")){
      //     if(swipeBlock){
      //       swipeBlockStart = Time.time;
      //       if(PlayerPrefs.GetFloat("sfx") != 0){
      //         sound.PlayOneShot(block);
      //       }

      //       body.AddForce(transform.right * -1 * 0.1f, ForceMode2D.Impulse);
      //       numBlocks++;
      //     }
      //     swipeBlock = false;
      //     return;
      //   }

      //   FindKiller(col.gameObject, false);
      //   KillPlayer();
      //   slash.GetComponent<BoxCollider2D>().enabled = false;
      //   side_slash.GetComponent<BoxCollider2D>().enabled = false;
      //   up_slash.GetComponent<BoxCollider2D>().enabled = false;
      //   down_slash.GetComponent<BoxCollider2D>().enabled = false;
      // }
      
      // shield trigger
      if(col.tag == "shield" && !respawn && !dead){

        // attack collisions
        if(player_animator.GetBool("block") || player_animator.GetBool("attack")){
          if(swipeBlock){
            swipeBlockStart = Time.time;
            if(PlayerPrefs.GetFloat("sfx") != 0){
              sound.PlayOneShot(block);
            }
            body.AddForce(transform.right * -1 * 0.05f, ForceMode2D.Impulse);
            numBlocks++;
          }
          swipeBlock = false;
          return;
        }

        // kill if not invincible
        if(!invincible){
          FindKiller(col.gameObject, false);
        }   
      }
      
      // bullet pick up
      else if(col.tag == "extraBullets" && !player_animator.GetBool("attack")){
        if(numBullets < bulletLimit){
          numBullets++;
          bulletPickUps++;
        }
        Destroy(col.gameObject);
      }
    }

  	public void HitByPulse(float time_of_pulse){
  		StartCoroutine(PlayerHit(time_of_pulse));
      ++pulse_hit_count;
  	}

  	IEnumerator PlayerHit(float time_of_pulse) {
  		yield return new WaitForSeconds(time_of_pulse * 3);
  		hit_by_pulse_times = 0;
  	}

    void SetLastDirection(){
      if(move_left){
        lastDirection = "left";
      }
      else if(move_right){
        lastDirection = "right";
      }
      else if(move_up){
        lastDirection = "up";
      }
      else if(move_down){
        lastDirection = "down";
      }
    }

    void TabletopMovementShift(){
      if(PlayerPrefs.GetString("screen") == "TABLETOP"){
        if(player_number == 1){
          return;
        }
        else if(player_number == 2){
          if(move_left){
            move_down = true;
            move_left = false;
          }
          else if(move_right){
            move_up = true;
            move_right = false;
          }
          else if(move_up){
            move_left = true;
            move_up = false;
          }
          else if(move_down){
            move_right = true;
            move_down = false;
          }
        }
        else if(player_number == 3){
          if(move_left){
            move_right = true;
            move_left = false;
          }
          else if(move_right){
            move_left = true;
            move_right = false;
          }
          else if(move_up){
            move_down = true;
            move_up = false;
          }
          else if(move_down){
            move_up = true;
            move_down = false;
          }
        }
        else if(player_number == 4){
          if(move_left){
            move_up = true;
            move_left = false;
          }
          else if(move_right){
            move_down = true;
            move_right = false;
          }
          else if(move_up){
            move_right = true;
            move_up = false;
          }
          else if(move_down){
            move_left = true;
            move_down = false;
          }
        }
      }
    }

    void SwapGravity(string direction){

      if(PlayerPrefs.GetString("screen") == "MONITOR" || player_number == 1){
        if(direction == "up" && player_orientation != orientation.up){
          Gravity("up", transform.localEulerAngles.y, 180f, false);
        }
        if(direction == "down" && player_orientation != orientation.down){
          Gravity("down", -transform.localEulerAngles.y, 0f, false);
        }
        if(direction == "left" && player_orientation != orientation.left){
          Gravity("left", 0f, -90f, false);
        }
        if(direction == "right" && player_orientation != orientation.right){
          Gravity("right", 0f, 90f, false);
        }
      }
      else if(PlayerPrefs.GetString("screen") == "TABLETOP"){
        if(player_number == 4){
          if(direction == "left" && player_orientation != orientation.up){
            Gravity("up", transform.localEulerAngles.y, 180f, false);
          }
          if(direction == "right" && player_orientation != orientation.down){
            Gravity("down", -transform.localEulerAngles.y, 0f, false);
          }
          if(direction == "down" && player_orientation != orientation.left){
            Gravity("left", 0f, -90f, false);
          }
          if(direction == "up" && player_orientation != orientation.right){
            Gravity("right", 0f, 90f, false);
          }
        }
        else if(player_number == 3){
          if(direction == "down" && player_orientation != orientation.up){
            Gravity("up", transform.localEulerAngles.y, 180f, false);
          }
          if(direction == "up" && player_orientation != orientation.down){
            Gravity("down", -transform.localEulerAngles.y, 0f, false);
          }
          if(direction == "right" && player_orientation != orientation.left){
            Gravity("left", 0f, -90f, false);
          }
          if(direction == "left" && player_orientation != orientation.right){
            Gravity("right", 0f, 90f, false);
          }
        }
        else if(player_number == 2){
          if(direction == "right" && player_orientation != orientation.up){
            Gravity("up", transform.localEulerAngles.y, 180f, false);
          }
          if(direction == "left" && player_orientation != orientation.down){
            Gravity("down", -transform.localEulerAngles.y, 0f, false);
          }
          if(direction == "up" && player_orientation != orientation.left){
            Gravity("left", 0f, -90f, false);
          }
          if(direction == "down" && player_orientation != orientation.right){
            Gravity("right", 0f, 90f, false);
          }
        }
      }
    }
}