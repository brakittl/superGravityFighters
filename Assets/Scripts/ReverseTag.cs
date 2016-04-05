using UnityEngine;
using System.Collections;

public class ReverseTag : MonoBehaviour {

	//Amount of time that should pass before player gets a point
	public float time_until_point;

	public bool attached_to_player;
	float time_before_points;
	float invincibility_time; // Time when gem switches players that the player gets to keep the gem at the least
	public float total_current_time_with_player;

	void Start() {
		attached_to_player = false;
		ResetTimers();
	}

	void Update() {
		invincibility_time -= Time.deltaTime;
		if (attached_to_player) {
			if (time_before_points > time_until_point) {
				transform.parent.GetComponent<player>().rt_points++;
				time_before_points -= time_until_point;
			}
			time_before_points += Time.deltaTime;
			transform.parent.GetComponent<player>().rt_total_time += Time.deltaTime;
			total_current_time_with_player += Time.deltaTime;
			if (total_current_time_with_player >= transform.parent.GetComponent<player>().rt_longest_continuous_hold)
			{
				transform.parent.GetComponent<player>().rt_longest_continuous_hold = total_current_time_with_player;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject != transform.parent && (col.gameObject.tag == "Player") && (invincibility_time <= 0)) {
			attached_to_player = true;
			ResetTimers();
			transform.position = col.transform.position;
			this.transform.parent = col.gameObject.transform;
		}

	}

	void ResetTimers() {
		time_before_points = 0;
		total_current_time_with_player = 0;
		invincibility_time = 1F;
	}


}
