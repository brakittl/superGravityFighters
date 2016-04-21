using UnityEngine;
using System.Collections;

public class ReverseTag : MonoBehaviour {

	//Amount of time that should pass before player gets a point
	public float time_until_point;
	public GameObject pulse;

	public bool attached_to_player, beenTouched = false;
	float invincibility_time; // Time when gem switches players that the player gets to keep the gem at the least
	public float total_current_time_with_player;
	GameObject pulse_instance_1;

	public AudioClip five_pts_mp3, ten_pts_mp3, twenty_pts_mp3, thirty_pts_mp3;

  AudioSource sound;
  public AudioClip stealSound;

  public float speed;

  public Vector3 target_position;
  float min_distance;
  public bool can_move;
  LayerMask ignore_layers = 1 << LayerMask.NameToLayer("Default"); // only check for collisions with layerX

	void Start(){
		attached_to_player = false;
		ResetTimers();
    sound = GetComponent<AudioSource>();
    StartCoroutine(StopMovement());
  }

	void Update(){
    speed = .75f;
    min_distance = 0.05f;
		invincibility_time -= Time.deltaTime;
		if(attached_to_player){
			total_current_time_with_player += Time.deltaTime;
			transform.parent.GetComponent<player>().rt_total_time += Time.deltaTime;
			transform.parent.GetComponent<player>().rt_points = (int)(transform.parent.GetComponent<player>().rt_total_time / time_until_point);
			CheckPoints(transform.parent.GetComponent<player>().rt_points);
			transform.localPosition = new Vector3(-0.06f, 0.15f, 0f);
			transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			if(total_current_time_with_player >= transform.parent.GetComponent<player>().rt_longest_continuous_hold){
				transform.parent.GetComponent<player>().rt_longest_continuous_hold = total_current_time_with_player;
			}
		}
    else{
      if(Vector3.Distance(transform.position, target_position) > min_distance && can_move){
        transform.position = Vector3.Lerp(transform.position, target_position, Time.deltaTime * speed);
      }
      else{
        StartCoroutine(StopMovement());
      }
    }
    float dist = 0.01f;
    Vector2 left = transform.TransformDirection(new Vector2(dist, 0));
    Debug.DrawRay(target_position, left, Color.green);
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject != transform.parent && (col.gameObject.tag == "Player") && (invincibility_time <= 0)){
      
      GameObject g = GameObject.Find(col.name);
      player other = g.GetComponent<player>();

      if(!attached_to_player && !beenTouched){
        beenTouched = true;
        other.firstTouch = true;
      }

			this.transform.parent = col.gameObject.transform;

			string player_color_string = transform.parent.GetComponent<player>().player_color.ToLower();
			Color player_color_Color = transform.parent.GetComponent<player>().colors[player_color_string];

			attached_to_player = true;
			ResetTimers();
			transform.position = col.transform.position;
			GetComponent<TrailRenderer>().material.SetColor("_EmissionColor", player_color_Color);
			StartCoroutine(ActivatePulses());
      if(PlayerPrefs.GetFloat("sfx") != 0){
        sound.PlayOneShot(stealSound);
      }
      
      other.steals++;
    }

	}

	void ResetTimers(){
		total_current_time_with_player = 0;
		invincibility_time = 1F;
	}

  bool PointInCollider(Vector3 point){
    float dist = 0.001f;
    Vector2 left = transform.TransformDirection(new Vector2(dist, 0));
    RaycastHit2D hit = Physics2D.Raycast(point, left, dist, ignore_layers);
    if(hit.collider != null){
      return true;
    }
    return false;
  }

	void CheckPoints(int points)
	{
		if (((level.S.rt_point_limit - points) == 30) && !transform.parent.GetComponent<player>().thirty_points_left)
		{
			sound.PlayOneShot(thirty_pts_mp3);
			transform.parent.GetComponent<player>().thirty_points_left = true;
		}
		else if (((level.S.rt_point_limit - points) == 20) && !transform.parent.GetComponent<player>().twenty_points_left)
		{
			sound.PlayOneShot(twenty_pts_mp3);
			transform.parent.GetComponent<player>().twenty_points_left = true;
		}
		else if (((level.S.rt_point_limit - points) == 10) && !transform.parent.GetComponent<player>().ten_points_left)
		{
			sound.PlayOneShot(ten_pts_mp3);
			transform.parent.GetComponent<player>().ten_points_left = true;
		}
		else if (((level.S.rt_point_limit - points) == 5) && !transform.parent.GetComponent<player>().five_points_left)
		{
			sound.PlayOneShot(five_pts_mp3);
			transform.parent.GetComponent<player>().five_points_left = true;
		}
	}

  void SetNewTarget(){
    target_position = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);
    while(Vector3.Distance(transform.position, target_position) < 2f || PointInCollider(target_position)){
      target_position = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);
    }
  }

	IEnumerator ActivatePulses(){
		AddPulse();
		yield return new WaitForSeconds(pulse.GetComponent<Pulse>().time_of_pulse / 4);
		AddPulse();
		yield return new WaitForSeconds(pulse.GetComponent<Pulse>().time_of_pulse / 4);
		AddPulse();
		yield return new WaitForSeconds(pulse.GetComponent<Pulse>().time_of_pulse / 4);
		AddPulse();
		yield return new WaitForSeconds(pulse.GetComponent<Pulse>().time_of_pulse / 4);
		AddPulse();
		yield return new WaitForSeconds(pulse.GetComponent<Pulse>().time_of_pulse / 4);
		AddPulse();
	}

  IEnumerator StopMovement(){
    SetNewTarget();
    can_move = false;
    yield return new WaitForSeconds(0.2f);
    can_move = true;
  }

	void AddPulse(){
		pulse_instance_1 = Instantiate(pulse, transform.parent.transform.position, Quaternion.Euler(new Vector3(0,0,0))) as GameObject;
		pulse_instance_1.transform.parent = transform.parent;
		pulse_instance_1.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>("general/pulse_" + transform.parent.GetComponent<player>().player_color.ToLower());
	}

}