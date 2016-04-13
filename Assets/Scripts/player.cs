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

  	
    //point limit is in level script, call with level.S.rt_point_limit

  	// orientation
  	public enum orientation{up, down, left, right};
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

  	// sounds
  	AudioSource sound;
  	public AudioClip gunshot, block, death, swordSlash, gravitySwap, shieldPulse;
    float gravVolume = 0.1f;

    // bullet information
    public GameObject bullet, extraBullet;
  	GameObject bullet_instance;
  	public float shotVelocity = 3f, numBullets = 1;
    public float fireRate = 1f;
  	float nextFire = 0f, bulletCreationDist = 0.25f;
  	string lastDirection = "right";

  	// respawns
  	public bool respawn = false, respawning = false;
  	Vector3 offscreen = new Vector3(-1000, -1000, -1000);  

  	// poison
  	//float poisonSpeed = 0.75f, poisonJump = 8f, poisonTime, poisonRate = 10;
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
        invincibleStart = 0f, invincibleTime = 1f;
    float delay = 0;

    // ui
    public Sprite[] bullet_sprites;
    public Sprite[] hearts_skulls;
    public GameObject[] hearts;
    public GameObject bullet_image;
    public GameObject rt_text_GO;
    public Text bullet_text;
    public Text rt_text;
    bool ui_found;

    // medals earned
    public List<String> medals = new List<string>();

    // colors
    Color red = new Color();
    Color orange = new Color();
    Color yellow = new Color();
    Color green = new Color();
    Color blue = new Color();
    Color purple = new Color();
    Color black = new Color();
    public Dictionary<string, Color> colors;

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
      // print(level.S.respawnPoints[sent_number - 1]);
      // transform.position = level.S.findRespawn();
    }

  // ==[start]==================================================================
  // ===========================================================================

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

      if(SceneManager.GetActiveScene().name == "_character_select"){
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
      ColorUtility.TryParseHtmlString("#ebebeb", out black);
      colors = new Dictionary<string, Color>(){
        {"red", red},
        {"orange", orange},
        {"yellow", yellow},
        {"green", green},
        {"blue", blue},
        {"purple", purple},
        {"black", black},
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
        bullet_image = GameObject.Find(bullet_image_string);
        bullet_image.GetComponent<Image>().sprite = get_sprite_by_name(bullet_sprites, player_color.ToLower() + "_bullet_1");
        string bullet_text_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_bullet_text";
        bullet_text = GameObject.Find(bullet_text_string).GetComponent<Text>();
        bullet_image.SetActive(true);
        GameObject.Find(bullet_text_string).SetActive(true);

        // get rt score text
        string rt_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_rt_score";
        rt_text_GO = GameObject.Find(rt_string);
        rt_text = rt_text_GO.GetComponent<Text>();

        if(level.S.gamemode != GameMode.REVERSE_TAG){
          rt_text_GO.SetActive(false);
        }
        else{
          rt_text_GO.SetActive(true);
        }

        // initialize hearts/skulls
        hearts = new GameObject[10];
        for(int i = 0; i < 10; ++i){
          string heart_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_" + (i + 1).ToString();
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
        }

      }

      // ==[gravity swap]=======================================================
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
  		if(!poisoned && !dead){
  			// up
  			if((Input.GetAxis(mac + "Controller " + player_number + " Right Stick Y Axis") < -0.4f || Input.GetButtonDown(mac + "Controller " + player_number + " Y Button") || Input.GetKey(KeyCode.W)) && player_orientation != orientation.up){
  				Gravity(orientation.up, transform.localEulerAngles.y, 180f);
          gravitySwapCount++;
          if(PlayerPrefs.GetFloat("sfx") != 0){
            sound.PlayOneShot(gravitySwap, gravVolume);
          }

        }
  			// down

  			if((Input.GetAxis(mac + "Controller " + player_number + " Right Stick Y Axis") > 0.4f || Input.GetButtonDown(mac + "Controller " + player_number + " A Button") || Input.GetKey(KeyCode.S)) && player_orientation != orientation.down){
  				Gravity(orientation.down, -transform.localEulerAngles.y, 0f);
          gravitySwapCount++;
          if(PlayerPrefs.GetFloat("sfx") != 0){
            sound.PlayOneShot(gravitySwap, gravVolume);
          }

        }
  			// left
  			if((Input.GetAxis(mac + "Controller " + player_number + " Right Stick X Axis") < -0.4f || Input.GetButtonDown(mac + "Controller " + player_number + " X Button") || Input.GetKey(KeyCode.A)) && player_orientation != orientation.left){
  				Gravity(orientation.left, 0f, -90f);
          gravitySwapCount++;
          if(PlayerPrefs.GetFloat("sfx") != 0){
            sound.PlayOneShot(gravitySwap, gravVolume);
          }

        }
  			// right
  			if((Input.GetAxis(mac + "Controller " + player_number + " Right Stick X Axis") > 0.4f || Input.GetButtonDown(mac + "Controller " + player_number + " B Button") || Input.GetKey(KeyCode.D)) && player_orientation != orientation.right){
  				Gravity(orientation.right, 0f, 90f);
          gravitySwapCount++;
          if(PlayerPrefs.GetFloat("sfx") != 0){
            sound.PlayOneShot(gravitySwap, gravVolume);
          }

        }
  		}

  		// ==[actions]============================================================
  		// =======================================================================

  		// if alive, allow attack, shoot, and block action
  		if(!dead){
  			if(level.S.gamemode != GameMode.REVERSE_TAG){

          // shoot
          //if((Input.GetButtonDown(mac + "Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.LeftShift)) && Time.time > nextFire && numBullets > 0)
          /*if((Input.GetButtonDown(mac + "Controller " + player_number + " Right Bumper") &&
              Input.GetButtonDown(mac + "Controller " + player_number + " Left Bumper") ||
              Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > nextFire){
          	Shoot();
          }*/
          
          // attack
          if((Input.GetAxis(mac + "Controller " + player_number + " Right Trigger") > 0.25f || Input.GetButtonDown(mac + "Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.Space)) && Time.time > nextFire){
            numSwordSwipes++;
  				  Block();
  			  }
          
          // block
          else if((Input.GetAxis(mac + "Controller " + player_number + " Left Trigger") > 0.25f || Input.GetButtonDown(mac + "Controller " + player_number + " Left Bumper") || Input.GetKey(KeyCode.Q)) && Time.time > nextFire){
            Shoot();
  			  }

  			}
  		}

  		// if dead, allow poison action
  		/*else{

        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); //Ghost Body
        
        // poison
        // if((Input.GetAxis(mac + "Controller " + player_number + " Right Bumper") >= 0.9 || Input.GetKey(KeyCode.LeftShift)) && Time.time > poisonTime){
        // 	Poison();
        // }

        if((Input.GetButtonDown(mac + "Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.LeftShift)) && Time.time > poisonTime){
  				Attack();
  			}

  		}*/

      // ==[movement bools]=====================================================
      // =======================================================================
      if(moving && player_animator.GetBool("grounded")){
        steps++;
        moving = false;
      }


      if(!player_animator.GetBool("grounded") && !airStart ){
          curAirTime = Time.time;
          airStart = true; 
      }
      else if(player_animator.GetBool("grounded")  && airStart){
          if((int)((Time.time - curAirTime) * 100) > longestAirTime)
              longestAirTime = (int)((Time.time - curAirTime) * 100);
          airTime += (int)((Time.time - curAirTime) * 100);
          airStart = false;
      }

      move_left = false;
  		move_right = false;
  		move_up = false;
  		move_down = false;

  		// move right
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick X Axis") >= 0.9f || Input.GetKey(KeyCode.RightArrow)){
        
        moving = true;
  			
        if(player_orientation == orientation.up){
  				move_left = true;
  				lastDirection = "left";
  			}
  			else if(player_orientation == orientation.left){
  				move_up = true;
  				lastDirection = "up";
  			}
  			else if(player_orientation == orientation.right){
  				move_down = true;
  				lastDirection = "down";
  			}
  			else{
  				move_right = true;
  				lastDirection = "right";
  			}

  		}

  		// move left
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick X Axis") <= -0.9f || Input.GetKey(KeyCode.LeftArrow)){
        
        moving = true;
  			
        if(player_orientation == orientation.up){
  				move_right = true;
  				lastDirection = "right";
  			}
  			else if(player_orientation == orientation.left){
  				move_down = true;
  				lastDirection = "down";
  			}
  			else if(player_orientation == orientation.right){
  				move_up = true;
  				lastDirection = "up";
  			}
  			else{
  				move_left = true;
  				lastDirection = "left";
  			}

  		}

  		// move up
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick Y Axis") <= -0.9f || Input.GetKey(KeyCode.UpArrow)){
        
        moving = true;
  			
        if(player_orientation == orientation.up){
  				move_down = true;
  				lastDirection = "down";
  			}
  			else if(player_orientation == orientation.left){
  				move_left = true;
  				lastDirection = "left";
  			}
  			else if(player_orientation == orientation.right){
  				move_right = true;
  				lastDirection = "right";
  			}
  			else{
  				move_up = true;
  				lastDirection = "up";
  			}

  		}

  		// move down
  		if(Input.GetAxis(mac + "Controller " + player_number + " Left Stick Y Axis") >= 0.9f || Input.GetKey(KeyCode.DownArrow)){
        
        moving = true;
  			
        if(player_orientation == orientation.up){
  				move_up = true;
  				lastDirection = "up";
  			}
  			else if(player_orientation == orientation.left){
  				move_right = true;
  				lastDirection = "right";
  			}
  			else if(player_orientation == orientation.right){
  				move_left = true;
  				lastDirection = "left";
  			}
  			else{
  				move_down = true;
  				lastDirection = "down";
  			}

  		}

      TabletopMovementShift();

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
      
      // ==[movement]===========================================================
      // =======================================================================

      // apply movement

  		// crouch
  		player_animator.SetBool("crouched", false);
  		if(move_down){
  			Crouch();
  		}
      
  		// ==[respawn and death]==================================================
  		// =======================================================================

  		// respawn
  		if(respawn && respawning){
  			respawning = false;
  			StartCoroutine(Blink());
  		}

  		if(!player_animator.GetBool("block")){
  			shield.GetComponent<PolygonCollider2D>().enabled = false;
  		}

  		// if has been attacked by Ghost
  		/*if(poisoned){

  			// reduce jump and speed
  			thrust = poisonJump;
  			speed = poisonSpeed;

  			// remove poison with button tap
  			if((Input.GetKeyDown(KeyCode.LeftShift) || 
            Input.GetButtonDown(mac + "Controller " + player_number + " A Button") ||
            Input.GetButtonDown(mac + "Controller " + player_number + " B Button") || 
            Input.GetButtonDown(mac + "Controller " + player_number + " X Button") || 
            Input.GetButtonDown(mac + "Controller " + player_number + " Y Button")) && !dead){
  				curButtonTaps++;
          poisonGO.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, (1 - 0.1f * curButtonTaps));
  			}

  			// remove poison after 10 taps
  			if(!dead && curButtonTaps == poisonButtonTaps){
  				poisoned = false;
  				thrust = jump_speed;
  				speed = run_speed;
  				curButtonTaps = 0;
          poisonGO.SetActive(false);
  			}
  		}*/

      if(!swipeBlock && Time.time - swipeBlockStart > swipeBlockTime){
        swipeBlock = true;
        body.velocity = new Vector2(0f, 0f);
      }

      if(invincible && Time.time > invincibleStart){
            invincible = false;
        }
  	}

  	void FixedUpdate(){

      // ==[run]================================================================
      // =======================================================================
  		
      if(move_right && !player_animator.GetBool("landing")){
  			Run(true);
  		}
  		if(move_left && !player_animator.GetBool("landing")){
  			Run(false);
  		}

      // ==[grounded]===========================================================
      // =======================================================================
  		
  		if(!checkGround()){
  			if(grounded == 0){
  				delay = 0.08F; // need a little delay to have the player land completely
  			}
  			grounded = 1;
  			player_animator.SetBool("grounded", true);
  		}

  		else{
  			grounded = 0;
  			player_animator.SetBool("grounded", false);
  			if(!player_animator.GetBool("jump") && player_animator.GetBool("run")){
  				player_animator.Play("Falling");
  			}
  		}

  		// ==[gravity force]======================================================
  		// =======================================================================

      float speed = body.velocity.magnitude;

  		if(player_orientation == orientation.down){
  			if(speed < terminal_velocity && grounded == 0){
  			  body.AddForce(down);
        }
  		}
  		else if(player_orientation == orientation.up){
  			if(speed < terminal_velocity && grounded == 0){
          body.AddForce(up);
        }
  		}
  		else if(player_orientation == orientation.left){
  			if(speed < terminal_velocity && grounded == 0){
          body.AddForce(left);
        }
  		}
  		else if(player_orientation == orientation.right){
  			if(speed < terminal_velocity && grounded == 0){
          body.AddForce(right);
        }
  		}

  		// ==[jump]===============================================================
  		// =======================================================================

  		// apply jump
  		if(move_up && grounded == 1 && (delay < 0)){
  			//Jump();
  		}
  		else if(grounded == 1 && (player_orientation == orientation.up || player_orientation == orientation.down)){
  			body.velocity = new Vector2(body.velocity.x, 0);
  			delay -= Time.deltaTime;
  		}
  		else if(grounded == 1 && (player_orientation == orientation.left || player_orientation == orientation.right)){
  			body.velocity = new Vector2(0, body.velocity.y);
  			delay -= Time.deltaTime;
  		}
  	}

  // ==[raycasts]=============================================================
  // =========================================================================

  	bool checkSides(){

  		float bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  		float bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;

  		float player_length = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
  		// float player_height = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;

  		// due to the box collider's position being off (due to rotation and offset), also need to "rotate" the ray		
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
  		// Vector2 right = transform.TransformDirection(new Vector2(-length_ray_leftright, 0));

  		LayerMask ignoreplayer_layerMask = ~(LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Border") | LayerMask.NameToLayer("TagBall"));
  		ignoreplayer_layerMask = ~ignoreplayer_layerMask;

  		//RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 4) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask);
  		//print(Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 2), transform.position.y),below,length_ray_updw,ignoreplayer_layerMask));
  		//print(hit.collider);
  		if(player_orientation == orientation.up || player_orientation == orientation.down){  
  			//Debug.DrawRay(new Vector2(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  + (player_length / 2)), left, Color.green);
  			//Debug.DrawRay(new Vector2(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  - (player_length / 2)), left, Color.green);
  			return(!Physics2D.Raycast(new Vector3(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  + (player_length / 2)),left,length_ray_leftright,ignoreplayer_layerMask) && 
  				!Physics2D.Raycast(new Vector3(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  - (player_length / 2)),left,length_ray_leftright, ignoreplayer_layerMask));
  		}

  		else {
  			length_ray_leftright = (player_length * 1.7F);
  			left = transform.TransformDirection(new Vector2(length_ray_leftright, 0));
  			//Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x  + (player_length / 2), transform.position.y + bc_offset_y), left, Color.green);
  			//Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x  - (player_length / 2), transform.position.y + bc_offset_y), left, Color.green);
  			return(!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x  + (player_length / 2), transform.position.y + bc_offset_y),left,length_ray_leftright,ignoreplayer_layerMask) && 
  				!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x  - (player_length / 2), transform.position.y + bc_offset_y),left,length_ray_leftright, ignoreplayer_layerMask));
  		}
  	}
      
  	bool checkGround(){

      // if(is_character_select){
      //   return true;
      // }
	
  		float bc_offset_x = GetComponent<BoxCollider2D>().offset.x * transform.localScale.x;
  		float bc_offset_y = GetComponent<BoxCollider2D>().offset.y * transform.localScale.y;
  		float player_length = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
  		float player_height = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;

  		// due to the box collider's position being off (due to rotation and offset), also need to "rotate" the ray		
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

      if(is_character_select){
        if(player_orientation == orientation.up || player_orientation == orientation.down){
          length_ray_updw -= 0.04f;
        }
        else{
          length_ray_updw -= 0.038f;
        }
      }

  		Vector2 below = transform.TransformDirection(new Vector2(0F, -length_ray_updw));

  		LayerMask ignoreplayer_layerMask = ~(LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Border") | LayerMask.NameToLayer("TagBall") | LayerMask.NameToLayer("Attack"));
  		ignoreplayer_layerMask = ~ignoreplayer_layerMask;

  		if(player_orientation == orientation.up || player_orientation == orientation.down){ 
  			//Debug.DrawRay(new Vector2(transform.position.x + (player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y), below, Color.green);
  			//Debug.DrawRay(new Vector2(transform.position.x - (player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y), below, Color.green);
  			return(!Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask) && 
  				!Physics2D.Raycast(new Vector3(transform.position.x - (player_length / 2) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw, ignoreplayer_layerMask));
  		}
  		else{
  			//Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x, transform.position.y + (player_length / 2) + bc_offset_y), below, Color.green);
  			//Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x, transform.position.y  - (player_length / 2) + bc_offset_y), below, Color.green);
  			return(!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x, transform.position.y + (player_length / 2) + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask) && 
  				!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x, transform.position.y - (player_length / 2) + bc_offset_y),below,length_ray_updw, ignoreplayer_layerMask));
  		}
  	}

  // ==[gravity]================================================================
  // ===========================================================================

  	public void Gravity(orientation new_orientation, float y_rot, float z_rot){
  		body.velocity = new Vector2(0f, 0f); // set velocity to zero at swap initializaiton
  		player_orientation = new_orientation; // set new orientation
  		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y_rot, z_rot); // rotate for new orientation
  		player_animator.Play("Swap"); // play animation
  	}

  // ==[movement]===============================================================
  // ===========================================================================

  	void Crouch(){
  		if(player_animator.GetBool("grounded")){
  			player_animator.SetBool("crouched", true);
  			player_animator.Play("Down");
  		}
  	}

  	void Run(bool right){

  		bool can_move = true;
  		can_move = !(player_animator.GetBool("attack") && grounded == 1);
  		can_move = can_move && !(player_animator.GetBool("block") && grounded == 1);

  		if(can_move){

  			float rotation = 0f;
  			float flip = 1f;

  			if(player_orientation == orientation.up || player_orientation == orientation.down){
  				if(!right){
  					rotation = 180f;
  				}
  				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotation, transform.localEulerAngles.z);
  			}

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

  			if(!player_animator.GetBool("crouched") && checkSides()){
  				transform.localPosition += transform.right * speed * Time.deltaTime;
  			}

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
    		numBullets--;

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
        if (PlayerPrefs.GetFloat("sfx") != 0)
            sound.PlayOneShot(shieldPulse);

      nextFire = Time.time + fireRate;
        invincibleStart = Time.time + invincibleTime;
        invincible = true;
      player_animator.Play("Attack");
  		player_animator.SetBool("block", true);
  		shield_animator.Play("Shield");
  		// shield.GetComponent<CircleCollider2D>().enabled = true;
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

  	public void FindKiller(GameObject collideObject, bool bulletAttack, string killer_color){
      // Vector3 killerPos = new Vector3(0,0,0);
  		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
  		foreach(GameObject p in players){
  			player other = (player)p.GetComponent(typeof(player));
        if(bulletAttack && other.bullet_instance == collideObject){
          other.numBulletHits++;
          // killerPos = other.transform.position;
          if(this.name != other.name){
            other.playersKilled.Add(this.gameObject.name);
          }
          else{
            suicides++;
          }  
        }

        else if(!bulletAttack && other.shield == collideObject){
	        other.playersKilled.Add(this.gameObject.name);
          other.numSwordHits++;
          // killerPos = other.transform.position;
        }
  		}
      KillPlayer(killer_color);
  	}

  	public void KillPlayer(string killer_color){
      
      if(PlayerPrefs.GetFloat("sfx") != 0){
        sound.PlayOneShot(death);
      }

      respawn = true;
      slash.GetComponent<BoxCollider2D>().enabled = false;
  	  side_slash.GetComponent<BoxCollider2D>().enabled = false;
  	  up_slash.GetComponent<BoxCollider2D>().enabled = false;
  	  down_slash.GetComponent<BoxCollider2D>().enabled = false;
      lives--;
      level.S.KillPause(transform.position, colors[killer_color.ToLower()]);

      // turn off poison
      poisoned = false;
      poisonGO.SetActive(false);
      thrust = jump_speed;
      speed = run_speed;

      numBullets = 1;

      player_animator.Play("Death");

      level.S.alive_players.Remove(gameObject);

      if(Time.time - lastDeath > longestLife)
        longestLife = (int)((Time.time - lastDeath) *100);
      if(Time.time - lastDeath < shortestLife)
        shortestLife = (int)((Time.time - lastDeath) * 100);
      lastDeath = Time.time;
        
      body.velocity = new Vector2(0f, 0f);
  	  StartCoroutine(Wait());
  	}

  	IEnumerator Wait(){

      Time.timeScale = 0.1f;
      yield return new WaitForSeconds(0.3f);

      Vector3 pos = transform.position;
  		transform.position = offscreen;
      if(level.S.gamemode == GameMode.SURVIVAL){
        Instantiate(extraBullet, pos, transform.rotation);
      }
      Gravity(orientation.down, -transform.localEulerAngles.y, 0f);
      player_orientation = orientation.down;

      yield return new WaitForSeconds(1f);

      transform.position = level.S.findRespawn();
  		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, 0f);
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.down;
      level.S.alive_players.Insert(0, gameObject);
      player_animator.Play("Appear");
  		
      if(lives == 0 && (level.S.gamemode == GameMode.SURVIVAL)){
        //halo.SetActive(true);
        dead = true;
        transform.position = offscreen;
        deathTime = Time.time;
        level.S.ranking.Add(this);
        Debug.Log("dead player");
      }
      
      respawning = true;
  	}

  	IEnumerator Blink(){
  		transform.GetComponent<Renderer>().enabled = false;
  		yield return new WaitForSeconds(0.1f);
  		transform.GetComponent<Renderer>().enabled = true;
  		yield return new WaitForSeconds(1f);
  		respawn = false;
  	}

  // ==[collisions and triggers]================================================
  // ===========================================================================
      
  	void OnTriggerEnter2D(Collider2D col){
      
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
      
      if(col.tag == "shield" && !respawn && !dead){
        if(player_animator.GetBool("block") || player_animator.GetBool("attack")){
          if(swipeBlock){
            swipeBlockStart = Time.time;
            if(PlayerPrefs.GetFloat("sfx") != 0){
              sound.PlayOneShot(block);
            }
            body.AddForce(transform.right * -1 * 0.02f, ForceMode2D.Impulse);
            numBlocks++;
          }
          swipeBlock = false;
          return;
        }
        if(!invincible){
          FindKiller(col.gameObject, false, player_color);
        }
            
      }
      
      else if(col.tag == "extraBullets" && !player_animator.GetBool("attack")){
        numBullets++;
        bulletPickUps++;
        Destroy(col.gameObject);
      }
    }

	public void HitByPulse(float time_of_pulse)
	{
		StartCoroutine(PlayerHit(time_of_pulse));

	}

	IEnumerator PlayerHit(float time_of_pulse) {
		yield return new WaitForSeconds(time_of_pulse * 3);
		hit_by_pulse_times = 0;
	}



  void TabletopMovementShift(){
    if(PlayerPrefs.GetString("screen") == "TABLETOP"){

      if(player_number == 1){
        return;
      }
      
      else if(player_number == 2){

      }

      else if(player_number == 3){

      }

      else if(player_number == 4){

      }

    }

  }


}