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

	void Start() {
		attached_to_player = false;
		ResetTimers();
	}

	void Update() {
		invincibility_time -= Time.deltaTime;
		if (attached_to_player) {
			total_current_time_with_player += Time.deltaTime;
			transform.parent.GetComponent<player>().rt_total_time += Time.deltaTime;
			transform.parent.GetComponent<player>().rt_points = (int)(transform.parent.GetComponent<player>().rt_total_time / time_until_point);
			transform.localPosition = new Vector3(-0.06f, 0.15f, 0f);
			transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			if (total_current_time_with_player >= transform.parent.GetComponent<player>().rt_longest_continuous_hold)
			{
				transform.parent.GetComponent<player>().rt_longest_continuous_hold = total_current_time_with_player;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject != transform.parent && (col.gameObject.tag == "Player") && (invincibility_time <= 0)){
      
      GameObject g = GameObject.Find(col.name);
      player other = g.GetComponent<player>();

      if(!attached_to_player && !beenTouched){
        beenTouched = true;
        other.firstTouch = true;
      }


			attached_to_player = true;
			ResetTimers();
			transform.position = col.transform.position;
			this.transform.parent = col.gameObject.transform;
			StartCoroutine(ActivatePulses());
      other.steals++;
    }

	}

	void ResetTimers() {
		total_current_time_with_player = 0;
		invincibility_time = 1F;
	}

	IEnumerator ActivatePulses() {
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

	void AddPulse() {
		pulse_instance_1 = Instantiate(pulse, transform.parent.transform.position, Quaternion.Euler(new Vector3(0,0,0))) as GameObject;
		pulse_instance_1.transform.parent = transform.parent;
	}

}