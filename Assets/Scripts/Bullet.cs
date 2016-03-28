﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour{
  
  public float reflectMultiplier = 1.25f;
  void OnTriggerEnter2D(Collider2D col){
        
        if (col.tag == "slash")
        {
            GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity * reflectMultiplier;
        }
        else if (col.tag == "ground" || col.tag == "bullet")
        {
            Destroy(this.gameObject);
        }
        else if (col.tag == "shield")
        {
            print("shield");
           GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity * reflectMultiplier;
        }
        else if (col.tag == "Player")
        {
            GameObject g = GameObject.Find(col.name);
            player other = g.GetComponent<player>();
            if (!other.dead && !other.respawn)
            {
                other.FindKiller(this.gameObject, true);
                other.KillPlayer();
                Destroy(this.gameObject);
            }
        }
  }
}