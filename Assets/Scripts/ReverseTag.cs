using UnityEngine;
using System.Collections;

public class ReverseTag : MonoBehaviour {

	//Amount of time that should pass before player gets a point
	public float time_until_point;

	public bool attached_to_player;
	public float time_with_player;

	void Start() {
		time_with_player = 0;
		attached_to_player = false;

	}

	void Update() {
		if (attached_to_player) {
			if (time_with_player > time_until_point) {
				transform.parent.GetComponent<player>().rt_points++;
				time_with_player -= time_until_point;
			}
			time_with_player += Time.deltaTime;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject != transform.parent && (col.gameObject.layer == 8)) {
			attached_to_player = true;
			time_with_player = 0;
			transform.position = col.transform.position;
			this.transform.parent = col.gameObject.transform;
		}

	}



}
