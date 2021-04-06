using UnityEngine;

public class Boid : MonoBehaviour {
    
    // everything we want to change in editor:
    [SerializeField] public float separationRadius;
    [SerializeField] public float radius;

    


    //  jobs doesnt support strongly typed classes ->  encapsulate the code we want to jobify into a struct
    public struct encapsulatedData {

        public Vector3 _velocity;
        public Vector3 pos;
        public Quaternion quat;
        public float maxVelocity;

        // needed to change velocity in JOBS
        /* public Vector3 velocity {
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
