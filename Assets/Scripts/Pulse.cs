using UnityEngine;
using System.Collections;

public class Pulse : MonoBehaviour {

	public float max_size = 1;
	public float time_of_pulse = 2;
	public float current_time = 0;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3(0,0,0);
		current_time = 0;
	}

	void Reset () {
		current_time = 0;

	}
		
	void Update () {
		current_time += Time.deltaTime;
		if (current_time < time_of_pulse)
		{
			transform.localScale = new Vector3((max_size * (current_time / time_of_pulse)), (max_size * (current_time / time_of_pulse)), 0);  
		}
		else
		{
			Destroy(this.gameObject);
		}
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if ((other.gameObject != transform.parent.gameObject) && (other.tag == "Player") && (other.gameObject.GetComponent<player>().hit_by_pulse_times < 5))
		{
			//print(transform.parent.gameObject + "just stole the gem!");
			//print(other.gameObject + "is about to go bye-bye");
			other.gameObject.GetComponent<player>().hit_by_pulse_times++;
			other.GetComponent<Rigidbody2D>().velocity = new Vector2(other.transform.position.x - (transform.position.x)*20/other.gameObject.GetComponent<player>().hit_by_pulse_times, 20*(other.transform.position.y - transform.position.y)/other.gameObject.GetComponent<player>().hit_by_pulse_times);
			print((transform.position.x - other.transform.position.x)*20/other.gameObject.GetComponent<player>().hit_by_pulse_times);
			print(20*(transform.position.y - other.transform.position.y)/other.gameObject.GetComponent<player>().hit_by_pulse_times);
			other.gameObject.GetComponent<player>().HitByPulse(time_of_pulse);
		}
	}





}
