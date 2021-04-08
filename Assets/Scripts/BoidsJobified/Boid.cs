using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boid : MonoBehaviour {
    
    // everything we want to change in editor:
    [SerializeField] public float separationRadius;
    [SerializeField] public float radius;
    [SerializeField] public float neighborRadius;

    public Vector3 velocity;
    public Vector3 pos;
    public Quaternion quat;
    public float maxVelocity;

    Collider boidCollider;
    public Collider getboidCollider { get { return boidCollider; } }

    // Start is called before the first frame update
    void Start() {
        boidCollider = GetComponent<Collider>();
    }

    public static Boid boid;

    void Awake() {
        boid = this;
    }

    


    public void Update() {  // this is where we actually do stuff!!!


    }

    //  jobs doesnt support strongly typed classes ->  encapsulate the code we want to jobify into a struct
    public struct encapsulatedData {



        public Vector3 _velocity;
        public Vector3 pos;
        public Quaternion quat;
        public float maxVelocity;

        // needed to change vel in JOBS
        /* public Vector3 vel {
             get { return _velocity; }
             set { _velocity = value; }
         } */
        public Vector3 GetVelocity() {
            return _velocity;
        }

        public void SetVelocity(Vector3 vel) {  // this is where we actually do stuff!!!
            _velocity = vel; 

        }

        public void Update() {  // this is where we actually do stuff!!!


        }



    }


}
