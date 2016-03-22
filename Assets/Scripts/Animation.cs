using UnityEngine;
using System.Collections;

public class Animation : MonoBehaviour {

    public float delay = 0.3f;
    public float start;

    // Use this for initialization
    void Start()
    {
        start = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - start > delay)
            Destroy(this.gameObject);
    }
}
