using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour {
    
    // everything we want to change in editor:
    [SerializeField] public float separationRadius;
    [SerializeField] public float radius;

    //  jobs doesnt support strongly typed classes ->  encapsulate the code we want to jobify into a struct
    public struct Data {

        public Vector3 velocity;
        public Vector3 pos;
        public Quaternion quat;
        public float maxVelocity;

      //  private Boid boid;

        // init forces for rules
       // public float separationForce;
        //public float alignmentForce;
        // public float cohesionForce;


        public void Update() {  // this is where we actually do stuff!!!
            
            // limit velocity 
          //  if (velocity.magnitude > maxVelocity) {
            //    velocity = velocity.normalized * maxVelocity;
         //   }
          //  pos += velocity;// * Time.deltaTime; // move 10 units every sec
           // quat = Quaternion.LookRotation(velocity); // look in direction its going 
           
        }

        //public Data(Boid boid) {

            // init pos
           // pos = Vector3.zero;
           // quat = Quaternion.identity; 

            // init velocity
            // the reason why one cube is going 0/0/2
          //  maxVelocity = 2;  // hardcoded for now 
          //  velocity = pos * maxVelocity; // start at max speed 
            

            // init
           // var average_alignment = Vector3.zero;
           // var average_cohesion = Vector3.zero;
           // var average_separation = Vector3.zero;
         //  var found = 0;
            // Create a list  
          //  List<Boid> BoidsList = new List<Boid>();
          //  BoidsList.Add(FindObjectOfType<Boid>());
        //}
    }


}
