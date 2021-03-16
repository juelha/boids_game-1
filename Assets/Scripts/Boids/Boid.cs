using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // to use .Where



public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float maxVelocity;

    // Start is called before the first frame update
    void Start()
    {
        
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
