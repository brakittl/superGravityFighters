using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class player : MonoBehaviour{

	// player information
	public int player_number;
	public string player_color;
	public int lives = 3;
	public bool dead = false;
  public GameObject[] hearts;

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
	public AudioClip gunshot, block, death, swordSlash, gravitySwap;

	// bullet information
	public GameObject bullet, extraBullet;
	GameObject bullet_instance;
	public float shotVelocity = 5f, numBullets = 1;
  public float fireRate = 1f;
	float nextFire = 0f, bulletCreationDist = 0.25f;
	string lastDirection = "right";

	// respawns
	public bool respawn = false, respawning = false;
	Vector3 offscreen = new Vector3(-1000, -1000, -1000);  

	// poison
	float poisonSpeed = 0.75f, poisonJump = 8f, poisonTime, poisonRate = 10;
	public int poisonButtonTaps = 10, curButtonTaps;
	public bool poisoned = false;
	bool playerContact = false;
	player playerInContact = null;

	// tracking statistics
	public int gravitySwapCount = 0, totalPoisoned = 0, numBulletShots = 0,
	numBulletHits = 0, numSwordSwipes = 0, numSwordHits = 0;
	public List<String> playersKilled;

  // blocking
  bool swipeBlock = true;
  float swipeBlockStart = 0f, swipeBlockTime = 0.25f;
  float delay = 0;

  // hearts/skulls
  public Sprite[] hearts_skulls;

  // get sprite by name
  public Sprite get_sprite_by_name(Sprite[]sprites, string sprite_name){
    for(int i = 0; i < sprites.Length; i++){
      if(sprites[i].name.Equals(sprite_name)){
        return sprites[i];
      }
    }
    return null;
  }

	void Start(){
		player_animator = GetComponent<Animator>();
		body = gameObject.GetComponent<Rigidbody2D>();
		grounded = 0;
		player_orientation = orientation.down;
		body.gravityScale = 0;    
		slash.GetComponent<BoxCollider2D>().enabled = false;
		side_slash.GetComponent<BoxCollider2D>().enabled = false;
		up_slash.GetComponent<BoxCollider2D>().enabled = false;
		down_slash.GetComponent<BoxCollider2D>().enabled = false;
		shield.GetComponent<CircleCollider2D>().enabled = false;
		sound = GetComponent<AudioSource>();
		jump_speed = thrust;
		run_speed = speed;
    hearts_skulls = Resources.LoadAll<Sprite>("hearts_skulls");
    hearts = new GameObject[10];
    for(int i = 0; i < 10; ++i){
      string heart_string = "ui/p" + player_number.ToString() + "/" + player_number.ToString() + "_" + (i + 1).ToString();
      GameObject current_heart = GameObject.Find(heart_string);
      current_heart.GetComponent<Image>().sprite = get_sprite_by_name(hearts_skulls, player_color.ToLower() + "_heart");
      hearts[i] = current_heart;
    }
	}

  void SetPlayerNumber(int sent_number){
    player_number = sent_number;
    // print(Level.S.respawnPoints[sent_number - 1]);
    // transform.position = Level.S.findRespawn();
  }

	void Update(){

    // ==[show hearts]==========================================================
    // =========================================================================

    if(hearts[0] != null)
        {
        for(int i = 1; i <= lives; ++i){
            hearts[i - 1].SetActive(true);
        }
        for(int i = lives + 1; i <= 10; ++i){
            hearts[i - 1].SetActive(false);
        }
      }
    

		// ==[gravity swap]=========================================================
		// =========================================================================

		// gravity vectors
		right = new Vector2(acceleration, 0f);
		left = new Vector2(-acceleration, 0f);
		down = new Vector2(0f, -acceleration);
		up = new Vector2(0f, acceleration);
        float volume = 0.25f;
		// swap gravity orientation
		if(!poisoned){
			// up
			if((Input.GetButtonDown("Controller " + player_number + " Y Button") || Input.GetKey(KeyCode.W)) && player_orientation != orientation.up){
				Gravity(orientation.up, transform.localEulerAngles.y, 180f);
                sound.PlayOneShot(gravitySwap, volume);
            }
			// down
			if((Input.GetButtonDown("Controller " + player_number + " A Button") || Input.GetKey(KeyCode.S)) && player_orientation != orientation.down){
				Gravity(orientation.down, -transform.localEulerAngles.y, 0f);
                sound.PlayOneShot(gravitySwap, volume);
            }
			// left
			if((Input.GetButtonDown("Controller " + player_number + " X Button") || Input.GetKey(KeyCode.A)) && player_orientation != orientation.left){
				Gravity(orientation.left, 0f, -90f);
                sound.PlayOneShot(gravitySwap, volume);
            }
			// right
			if((Input.GetButtonDown("Controller " + player_number + " B Button") || Input.GetKey(KeyCode.D)) && player_orientation != orientation.right){
				Gravity(orientation.right, 0f, 90f);
                sound.PlayOneShot(gravitySwap, volume);
            }
		}

		// ==[actions]==============================================================
		// =========================================================================

		// if alive, allow attack, shoot, and block action
		if(!dead){
            
            // shoot
            //if((Input.GetButtonDown("Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.LeftShift)) && Time.time > nextFire && numBullets > 0)
            if((Input.GetButtonDown("Controller " + player_number + " Right Bumper") &&
                Input.GetButtonDown("Controller " + player_number + " Left Bumper")) && Time.time > nextFire)
            {
            	Shoot();
            }
            // attack
            //if((Input.GetAxis("Controller " + player_number + " Right Trigger") >= 0.9 || Input.GetKey(KeyCode.Space)) && Time.time > nextFire)
            else if ((Input.GetButtonDown("Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.Space)) && Time.time > nextFire)
            {
				Attack();
			}
            // block
            //if((Input.GetAxis("Controller " + player_number + " Left Trigger") >= 0.9 || Input.GetKey(KeyCode.Q)) && Time.time > nextFire)
            else if ((Input.GetButtonDown("Controller " + player_number + " Left Bumper") || Input.GetKey(KeyCode.Q)) && Time.time > nextFire)
            {
                Block();
			}
			// super slash for shits and gigs
			//if(Input.GetButtonDown("Controller " + player_number + " Left Bumper") || Input.GetKey(KeyCode.F)){
			//	SuperSlash();
			//}
		}
		// if dead, allow poison action
		else{
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); //Ghost Body
            // poison
            //if ((Input.GetAxis("Controller " + player_number + " Right Bumper") >= 0.9 || Input.GetKey(KeyCode.LeftShift)) && Time.time > poisonTime){
			//	Poison();
			//}
            if((Input.GetButtonDown("Controller " + player_number + " Right Bumper") || Input.GetKey(KeyCode.LeftShift)) && Time.time > poisonTime)
            {
				Attack();
			}
		}

		// ==[movement bools]=======================================================
		// =========================================================================

		move_left = false;
		move_right = false;
		move_up = false;
		move_down = false;

		// move right
		if(Input.GetAxis("Controller " + player_number + " Left Stick X Axis") >= 0.9f || Input.GetKey(KeyCode.RightArrow)){
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
		if(Input.GetAxis("Controller " + player_number + " Left Stick X Axis") <= -0.9f || Input.GetKey(KeyCode.LeftArrow)){
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
		if(Input.GetAxis("Controller " + player_number + " Left Stick Y Axis") <= -0.9f || Input.GetKey(KeyCode.UpArrow)){
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
		if(Input.GetAxis("Controller " + player_number + " Left Stick Y Axis") >= 0.9f || Input.GetKey(KeyCode.DownArrow)){
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

    // ==[resets]===============================================================
    // =========================================================================

    if(!move_left && !move_right){
      player_animator.SetBool("run", false);
    }
    else if(player_animator.GetBool("jump")){
      player_animator.SetBool("run", false);
    }

    if (!player_animator.GetBool("attack")){
      slash.GetComponent<BoxCollider2D>().enabled = false;
      side_slash.GetComponent<BoxCollider2D>().enabled = false;
      up_slash.GetComponent<BoxCollider2D>().enabled = false;
      down_slash.GetComponent<BoxCollider2D>().enabled = false;
    }
    
    // ==[movement]=============================================================
    // =========================================================================

    // apply movement
    if(move_right && !player_animator.GetBool("landing")){
			Run(true);
		}
		if(move_left && !player_animator.GetBool("landing")){
			Run(false);
		}

		// crouch
		player_animator.SetBool("crouched", false);
		if(move_down){
			Crouch();
		}
    
		// ==[respawn and death]====================================================
		// =========================================================================

		// respawn
		if(respawn && respawning){
			respawning = false;
			StartCoroutine(Blink());
		}

		if(!player_animator.GetBool("block")){
			shield.GetComponent<CircleCollider2D>().enabled = false;
		}

		// if has been attacked by Ghost
		if(poisoned){

			// reduce jump and speed
			thrust = poisonJump;
			speed = poisonSpeed;

			// remove poison with A button tap
			if((Input.GetKeyDown(KeyCode.LeftShift) || 
                Input.GetButtonDown("Controller " + player_number + " A Button") ||
                Input.GetButtonDown("Controller " + player_number + " B Button") || 
                Input.GetButtonDown("Controller " + player_number + " X Button") || 
                Input.GetButtonDown("Controller " + player_number + " Y Button")) && !dead){
				curButtonTaps++;
                poisonGO.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, (1 - 0.1f * curButtonTaps));
                //50% transperency good for ghost
                //Transperency drop
			}

			// remove poison after 10 taps or time limit
			if(!dead && curButtonTaps == poisonButtonTaps){
				poisoned = false;
				thrust = jump_speed;
				speed = run_speed;
				curButtonTaps = 0;
                poisonGO.SetActive(false);
			}
		}

        if (!swipeBlock && Time.time - swipeBlockStart > swipeBlockTime)
        {
            swipeBlock = true;
            body.velocity = new Vector2(0f, 0f);
        }
	}

	void FixedUpdate(){

		// grounded
		if(!checkGround()){
			if(grounded == 0){
				// need a little delay to have the player land completely
				delay = 0.05F;
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

		// ==[gravity force]========================================================
		// =========================================================================

    float speed = body.velocity.magnitude;

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

		// ==[jump]=================================================================
		// =========================================================================

		// apply jump
		if(move_up && grounded == 1 && (delay < 0)){
			Jump();
		}
		else{
			delay -= Time.deltaTime;
		}
	}

	bool checkSides() 
	{

		float bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
		float bc_offset_y = GetComponent<BoxCollider2D>().offset.y;



		float player_length = GetComponent<BoxCollider2D>().size.x;
		float player_height = GetComponent<BoxCollider2D>().size.y;


		//Due to the box collider's position being off (due to rotation and offset), also need to "rotate" the ray
		//Ex. when player turns right, the bc's y looks like it is rotated to y, effectively transforming the position by -2*offset.x (to -offset.x)
		//		However, this transformation isn't shown anywhere (offset is still the old one, +offset.x)
		switch (player_orientation)
		{
		case orientation.down:
			if (transform.rotation.y == 0)
			{
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			else 
			{
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		case orientation.up:
			if (transform.rotation.y == 0)
			{
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			else 
			{
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		case orientation.left:
			if (transform.rotation.y == 0)
			{
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			else 
			{
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		case orientation.right:
			if (transform.rotation.y == 0)
			{
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			else 
			{
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		default:
			print("Hey, why isn't the orientation set?");
			break;
		}
		//print(bc_offset_x);
		//print(bc_offset_y);

		float length_ray_leftright = (player_length * 1.1F);

		// Vector2 left = transform.TransformDirection(new Vector2(length_ray_leftright, 0));
		// Vector2 right = transform.TransformDirection(new Vector2(-length_ray_leftright, 0));

		LayerMask ignoreplayer_layerMask = ~(LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Border"));
		//print(ignoreplayer_layerMask);
		ignoreplayer_layerMask = ~ignoreplayer_layerMask;

		//RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 4) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask);
		//print(Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 2), transform.position.y),below,length_ray_updw,ignoreplayer_layerMask));
		//print(hit.collider);
		if (player_orientation == orientation.up || player_orientation == orientation.down)
		{  
			//Debug.DrawRay(new Vector2(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  + (player_length / 2)), left, Color.green);
			//Debug.DrawRay(new Vector2(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  - (player_length / 2)), left, Color.green);
			return(!Physics2D.Raycast(new Vector3(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  + (player_length / 2)),left,length_ray_leftright,ignoreplayer_layerMask) && 
				!Physics2D.Raycast(new Vector3(transform.position.x - bc_offset_x, transform.position.y + bc_offset_y  - (player_length / 2)),left,length_ray_leftright, ignoreplayer_layerMask));

		}
		else 
		{
			length_ray_leftright = (player_length * 1.7F);
			left = transform.TransformDirection(new Vector2(length_ray_leftright, 0));
			//Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x  + (player_length / 2), transform.position.y + bc_offset_y), left, Color.green);
			//Debug.DrawRay(new Vector2(transform.position.x + bc_offset_x  - (player_length / 2), transform.position.y + bc_offset_y), left, Color.green);
			return(!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x  + (player_length / 2), transform.position.y + bc_offset_y),left,length_ray_leftright,ignoreplayer_layerMask) && 
				!Physics2D.Raycast(new Vector3(transform.position.x + bc_offset_x  - (player_length / 2), transform.position.y + bc_offset_y),left,length_ray_leftright, ignoreplayer_layerMask));

		}
	}
    
	bool checkGround(){
		
		float bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
		float bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
		float player_length = GetComponent<BoxCollider2D>().size.x;
		float player_height = GetComponent<BoxCollider2D>().size.y;

		// due to the box collider's position being off (due to rotation and offset), also need to "rotate" the ray
		// ex. when player turns right, the bc's y looks like it is rotated to y, effectively transforming the position by -2*offset.x (to -offset.x)
		//		 however, this transformation isn't shown anywhere (offset is still the old one, +offset.x)
		
    switch (player_orientation){
		case orientation.down:
			if(transform.rotation.y == 0){
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			else{
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		case orientation.up:
			if(transform.rotation.y == 0){
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			else{
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		case orientation.left:
			if(transform.rotation.y == 0){
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			else{
				bc_offset_x = GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		case orientation.right:
			if(transform.rotation.y == 0){
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = GetComponent<BoxCollider2D>().offset.y;
			}
			else{
				bc_offset_x = -GetComponent<BoxCollider2D>().offset.x;
				bc_offset_y = -GetComponent<BoxCollider2D>().offset.y;
			}
			break;
		default:
			print("Hey, why isn't the orientation set?");
			break;
		}

		float length_ray_updw = (player_height / 2) + (player_height * 0.08F);

		Vector2 below = transform.TransformDirection(new Vector2(0F, -length_ray_updw));

		LayerMask ignoreplayer_layerMask = ~(LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Border"));
		//print(ignoreplayer_layerMask);
		ignoreplayer_layerMask = ~ignoreplayer_layerMask;

		//RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 4) + bc_offset_x, transform.position.y + bc_offset_y),below,length_ray_updw,ignoreplayer_layerMask);
		//print(Physics2D.Raycast(new Vector3(transform.position.x + (player_length / 2), transform.position.y),below,length_ray_updw,ignoreplayer_layerMask));
		//print(hit.collider);
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

	void Gravity(orientation new_orientation, float y_rot, float z_rot){
		body.velocity = new Vector2(0f, 0f); // set velocity to zero at swap initializaiton
		gravitySwapCount++; // increment count for statistics
		player_orientation = new_orientation; // set new orientation
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y_rot, z_rot); // rotate for new orientation
		player_animator.Play("Swap"); // play animation
	}

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

	void Attack(){

		if(!player_animator.GetBool("attack") && !respawn && !player_animator.GetBool("crouched")){

            sound.PlayOneShot(swordSlash);
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

	void SuperSlash(){
		down_slash_animator.Play("Slash");
		up_slash_animator.Play("Slash");
		slash_animator.Play("Slash");
		player_animator.Play("Attack");
		slash.GetComponent<BoxCollider2D>().enabled = true;
		up_slash.GetComponent<BoxCollider2D>().enabled = true;
		down_slash.GetComponent<BoxCollider2D>().enabled = true;
	}

	void Shoot(){
		sound.PlayOneShot(gunshot);
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

	void Block(){
    nextFire = Time.time + fireRate;
    player_animator.Play("Block");
		player_animator.SetBool("block", true);
		shield_animator.Play("Shield");
		shield.GetComponent<CircleCollider2D>().enabled = true;
	}

	void Jump(){
		player_animator.Play("Jump");
		player_animator.SetBool("run", false);
		player_animator.SetBool("jump", true);
		body.AddForce(transform.up * thrust);   
	}

	void Poison(){
        poisoned = true;
        curButtonTaps = 0;
        poisonGO.SetActive(true);
        poisonGO.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
	}

	public void FindKiller(GameObject collideObject, bool bulletAttack){

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject p in players){
			if(p.gameObject == this.gameObject)
				continue;

			player other = (player)p.GetComponent(typeof(player));
			if(bulletAttack && other.bullet_instance == collideObject){
				other.playersKilled.Add(this.gameObject.name);
				other.numBulletHits++;
			}

			else if(!bulletAttack && other.slash == collideObject){
				other.playersKilled.Add(this.gameObject.name);
				other.numSwordHits++;
			}
		}
	}

	public void KillPlayer()
    {
        sound.PlayOneShot(death);
        respawn = true;
		slash.GetComponent<BoxCollider2D>().enabled = false;
		side_slash.GetComponent<BoxCollider2D>().enabled = false;
		up_slash.GetComponent<BoxCollider2D>().enabled = false;
		down_slash.GetComponent<BoxCollider2D>().enabled = false;
        //lives--;
        lives = 0;
        numBullets = 1;
		Gravity(orientation.down, -transform.localEulerAngles.y, 0f);

        Level.S.KillPause(transform.position);
        poisoned = false;
		player_animator.Play("Death");
		body.velocity = new Vector2(0f, 0f);
		player_orientation = orientation.down;
		StartCoroutine(Wait());
	}

	IEnumerator Wait(){
		yield return new WaitForSeconds(0.3f);

        Vector3 pos = transform.position;

		transform.position = offscreen;

        Instantiate(extraBullet, pos, transform.rotation);

		yield return new WaitForSeconds(2f);
		transform.position = Level.S.findRespawn();
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, 0f);
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.down;
      player_animator.Play("Appear");
        if (lives == 0)
        {
            halo.SetActive(true);
            dead = true;
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

	void OnCollisionEnter2D(Collision2D coll){
		GameObject other = coll.gameObject;
		if(other.tag == "Player" && other.name != this.gameObject.name){
			playerContact = true;
			playerInContact = (player)other.GetComponent(typeof(player));
		}
	}

	void OnCollisionExit2D(Collision2D coll){
		if(coll.gameObject.tag == "Player" && coll.gameObject.name != this.gameObject.name){
			playerContact = false;
			playerInContact = null;
		}
	}
    
	void OnTriggerEnter2D(Collider2D col){
        if(col.tag == "slash" && !respawn && !dead)
        {
            player p = col.transform.parent.GetComponent<player>();
            if (p.dead)
            {
                if (!poisoned)
                {
                    p.totalPoisoned++;
                }
                Poison();
                p.poisonTime = Time.time + poisonRate;
                return;
            }

            if (player_animator.GetBool("block") || player_animator.GetBool("attack"))
            {
                if (swipeBlock)
                {
                    //If attack one players back while they are attacking
                    //pushes both players backwards same direction
                    //kills player in front
                    swipeBlockStart = Time.time;
                    sound.PlayOneShot(block);
                    body.AddForce(transform.right * -1 * 0.1f, ForceMode2D.Impulse);
                }
                swipeBlock = false;
				return;
			}
			FindKiller(col.gameObject, false);
			KillPlayer();
			slash.GetComponent<BoxCollider2D>().enabled = false;
			side_slash.GetComponent<BoxCollider2D>().enabled = false;
			up_slash.GetComponent<BoxCollider2D>().enabled = false;
			down_slash.GetComponent<BoxCollider2D>().enabled = false;
		}
        else if(col.tag == "extraBullets" && !player_animator.GetBool("attack"))
        {
            numBullets++;
            Destroy(col.gameObject);
        }

  }
}