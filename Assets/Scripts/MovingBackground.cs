using UnityEngine;
using System.Collections;

public class MovingBackground : MonoBehaviour {

  void Start(){
    if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer){
      Debug.Log("Windows detected");
    }
    else{
      Debug.Log("Mac detected");
    }
  }
  
  void Update(){
    if(transform.localScale.y >= 1){
      transform.localScale = transform.localScale / 1.0025f;
    }
    
  }
}
