using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fressen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "boid");// || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )

        {
            Destroy(col.gameObject);

        }
    }
}
