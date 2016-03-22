using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

  public enum orientation { up, down, left, right };

  // public GameObject slash;

  Animator player_animator;
  public Animator slash_animator;
  public Animator side_slash_animator;
  public Animator up_slash_animator;
  public Animator down_slash_animator;
  public Animator shield_animator;
  Rigidbody2D body;
  
  
    public GameObject slash;
    public GameObject side_slash;
    public GameObject up_slash;
    public GameObject down_slash;

  public float speed;
  public float thrust;
  public float acceleration;

  public int grounded;
  bool move_left = false;
  bool move_right = false;
  bool move_up = false;
  bool move_down = false;

  Vector2 right, left, down, up;

  public orientation player_orientation;

  void Start(){
    player_animator = GetComponent<Animator>();
    body = gameObject.GetComponent<Rigidbody2D>();
    grounded = 0;
    // Physics2D.gravity = down;
    player_orientation = orientation.down;
    body.gravityScale = 0;
    
    

        slash.GetComponent<BoxCollider2D>().enabled = false;
        side_slash.GetComponent<BoxCollider2D>().enabled = false;
        up_slash.GetComponent<BoxCollider2D>().enabled = false;
        down_slash.GetComponent<BoxCollider2D>().enabled = false;
  }
  
  void Update(){

    right = new Vector2(acceleration, 0f);
    left = new Vector2(-acceleration, 0f);
    down = new Vector2(0f, -acceleration);
    up = new Vector2(0f, acceleration);

    // swap gravity orientation
    if(Input.GetKey(KeyCode.W) && player_orientation != orientation.up){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.up;
      // Physics2D.gravity = up;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 180f);
      player_animator.Play("Swap");
    }
    if(Input.GetKey(KeyCode.S) && player_orientation != orientation.down){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.down;
      // Physics2D.gravity = down;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, 0f);
      player_animator.Play("Swap");
    }
    if(Input.GetKey(KeyCode.A) && player_orientation != orientation.left){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.left;
      // Physics2D.gravity = left;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, -90f);
      player_animator.Play("Swap");
    }
    if(Input.GetKey(KeyCode.D) && player_orientation != orientation.right){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.right;
      // Physics2D.gravity = right;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, 90f);
      player_animator.Play("Swap");
    }

    // attack
    if(Input.GetKey(KeyCode.Space)){
      Attack();
    }

    // block
    if(Input.GetKey(KeyCode.Q)){
      Block();
    }

    // super slash for shits and gigs
    if(Input.GetKey(KeyCode.F)){
      down_slash_animator.Play("Slash");
      up_slash_animator.Play("Slash");
      slash_animator.Play("Slash");
      player_animator.Play("Attack");
    }

    move_left = false;
    move_right = false;
    move_up = false;
    move_down = false;

    // move right
    if(Input.GetKey(KeyCode.RightArrow)){
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
    if(Input.GetKey(KeyCode.LeftArrow)){
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
    if(Input.GetKey(KeyCode.UpArrow)){
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
    if(Input.GetKey(KeyCode.DownArrow)){
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

    // apply left movement
    if(move_right){
      Run(true);
    }
    if(move_left){
      Run(false);
    }

    player_animator.SetBool("crouched", false);
    if(move_down){
      Crouch();
    }

    if(Input.GetKey(KeyCode.B)){
      player_animator.Play("Death");
      
    }
    
    if(!Input.anyKey){
      player_animator.SetBool("run", false);
    }

        if (!player_animator.GetBool("attack"))
        {
            slash.GetComponent<BoxCollider2D>().enabled = false;
            side_slash.GetComponent<BoxCollider2D>().enabled = false;
            up_slash.GetComponent<BoxCollider2D>().enabled = false;
            down_slash.GetComponent<BoxCollider2D>().enabled = false;
        }

        if (respawn && respawning)
        {
            respawning = false;
            StartCoroutine(Blink());
        }

  }

  void FixedUpdate(){

    if(player_orientation == orientation.down){
      body.AddForce(down);
    }
    else if(player_orientation == orientation.up){
      body.AddForce(up);
    }
    else if(player_orientation == orientation.left){
      body.AddForce(left);
    }
    else if(player_orientation == orientation.right){
      body.AddForce(right);
    }
    
    if(move_up && grounded == 1){
      Jump();
    }
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

      if(!player_animator.GetBool("crouched")){
        transform.localPosition += transform.right * speed * Time.deltaTime;
      }

      if(player_animator.GetBool("grounded")){
        player_animator.SetBool("run", true);
      }

    }
  }

   void Attack(){

        if (!player_animator.GetBool("attack"))
        {

            player_animator.SetBool("attack", true);

            if (!player_animator.GetBool("grounded"))
            {
                if (move_left || move_right)
                {
                    player_animator.Play("Side_Attack");
                    side_slash_animator.Play("Slash");
                    side_slash.GetComponent<BoxCollider2D>().enabled = true;
                }
                else if (move_down)
                {
                    player_animator.Play("Down_Attack");
                    down_slash_animator.Play("Slash");
                    down_slash.GetComponent<BoxCollider2D>().enabled = true;
                }
                else if (move_up)
                {
                    player_animator.Play("Up_Attack");
                    up_slash_animator.Play("Slash");
                    up_slash.GetComponent<BoxCollider2D>().enabled = true;
                }
                else {
                    player_animator.Play("Attack");
                    slash_animator.Play("Slash");
                    slash.GetComponent<BoxCollider2D>().enabled = true;
                }
            }
            else {
                player_animator.Play("Attack");
                slash_animator.Play("Slash");
                slash.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

    }

  void Block(){
    player_animator.Play("Block");
    player_animator.SetBool("block", true);
    shield_animator.Play("Shield");
  }

  void Jump(){
    player_animator.Play("Jump");
    player_animator.SetBool("run", false);
    player_animator.SetBool("jump", true);
    body.AddForce(transform.up * thrust);   
  }

  void OnCollisionEnter2D(Collision2D coll){
    GameObject other = coll.gameObject;

    if(other.tag == "ground"){
      grounded += 1;
      player_animator.SetBool("grounded", true);
    }
  }

  void OnCollisionExit2D(Collision2D coll){
    // leaving ground from jump
    if(coll.gameObject.tag == "ground"){
      grounded -= 1;
      if(grounded <= 0){
        grounded = 0;
        player_animator.SetBool("grounded", false);
        if(!player_animator.GetBool("jump")){
          player_animator.Play("Falling");
          print("yo");
        }
      }
    }
  }


    bool respawn = false, respawning = false;
    void KillPlayer()
    {
        player_animator.Play("Death");
        StartCoroutine(Wait());
    }

    public Vector3 offscreen = new Vector3(-1000, -1000, -1000);
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.75f);
        transform.position = offscreen;
        yield return new WaitForSeconds(2f);
        //transform.position = Level.S.findRespawn();
        transform.position = Level.S.respawnPoints[UnityEngine.Random.Range(0, Level.S.respawnPoints.Length)];
        respawn = true;
        respawning = true;
    }

    IEnumerator Blink()
    {
        transform.GetComponent<Renderer>().enabled = false; ;
        yield return new WaitForSeconds(0.2f);
        transform.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.5f);

        transform.GetComponent<Renderer>().enabled = false; ;
        yield return new WaitForSeconds(0.2f);
        transform.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.5f);

        transform.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        transform.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.75f);

        transform.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        transform.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(1f);
        respawn = false;
    }

}
