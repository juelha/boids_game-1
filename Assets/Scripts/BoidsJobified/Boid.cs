using UnityEngine;

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


        public void Update() {  // this is where we actually do stuff!!!
            
           
        }

    }


}
