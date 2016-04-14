using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

	void Update(){
    gameObject.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z - 2);
	}
}
