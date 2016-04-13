using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour{
  
  public float reflectMultiplier = 1.25f;
  public int borderSwaps = 0;
  
    void Update()
    {
        if(borderSwaps > 10)
            Destroy(this.gameObject);
    }

  void OnTriggerEnter2D(Collider2D col){
    
    if(col.tag == "slash" || col.tag == "shield"){
      GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity * reflectMultiplier;
    }
    else if(col.tag == "ground" || col.tag == "bullet" || col.tag == "select_border"){
      Destroy(this.gameObject);
    }
    else if(col.tag == "Player"){
      GameObject g = GameObject.Find(col.name);
      player other = g.GetComponent<player>();
      if(!other.dead && !other.respawn && !other.invincible){
        other.FindKiller(this.gameObject, true);
        Destroy(this.gameObject);
      }
    }
  }
}
