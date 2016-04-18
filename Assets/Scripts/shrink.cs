using UnityEngine;
using System.Collections;

public class shrink : MonoBehaviour{

  float x, y, z;
  float speed = 0.1f;

  // bool done = false;
	
	void Update(){

    x = transform.localScale.x - speed;
    y = transform.localScale.y - speed;
    z = transform.localScale.z - speed;

    if(x <= 0){
      Destroy(this.gameObject);
      return;
    }

    transform.localScale = new Vector3(x, y, z);
	}

}
