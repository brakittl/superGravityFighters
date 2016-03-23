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
        if (col.tag == "swipe")
        {
            GetComponent<Rigidbody>().velocity = -GetComponent<Rigidbody>().velocity * reflectMultiplier;
        }
        else if (col.tag == "ground" || col.tag == "bullet")
        {
            Destroy(this.gameObject);
        }
    }
}
