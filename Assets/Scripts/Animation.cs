using UnityEngine;
using System.Collections;

public class Animation : MonoBehaviour {

  public float delay = 0.3f;
  public float start;

  void Start(){
    start = Time.time;
  }

  void Update(){
    if(Time.time - start > delay){
      Destroy(this.gameObject);
    }
  }
  
}
