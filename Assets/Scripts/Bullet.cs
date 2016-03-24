using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float reflectMultiplier = 1.25f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        print(col.name);
        if (col.tag == "slash")
            GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity * reflectMultiplier;

        else if (col.tag == "ground" || col.tag == "bullet")
            Destroy(this.gameObject);
        else if (col.tag == "shield")
            GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity * reflectMultiplier;
        else if (col.tag == "Player")
        {
            GameObject g = GameObject.Find(col.name);
            player other = g.GetComponent<player>();
            other.FindKiller(this.gameObject, true);
            other.KillPlayer();
            Destroy(this.gameObject);
        }
    }
}
