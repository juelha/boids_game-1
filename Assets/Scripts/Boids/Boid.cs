using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // to use .Where



public class Boid : MonoBehaviour
{


   // public Vector3 pos; // prob dont need
    public Vector3 velocity;
    public float maxVelocity;

    // vectors for rules
   // public Vector3 ali;
   // public Vector3 sep;
   // public Vector3 coh;

    // dont leave
   // public Vector3 border;


    // Start is called before the first frame update
    void Start()
    {
        velocity = this.transform.forward * maxVelocity; // start at max speed 
    }

    // Update is called once per frame
    void Update()
    {
        // limit velocity 
        if(velocity.magnitude > maxVelocity) {
            velocity = velocity.normalized * maxVelocity;
        }
        this.transform.position += velocity * Time.deltaTime; // move 10 units every sec
        this.transform.rotation = Quaternion.LookRotation(velocity); // look in direction its going 
        
    }
}
