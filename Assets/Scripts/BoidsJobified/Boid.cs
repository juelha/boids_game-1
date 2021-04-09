using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    // everything we want to change in editor:
    [SerializeField] public float radius;
  //  [SerializeField] public float separationRadius;
    [SerializeField] public float nearbyObjRadius;
  //  [SerializeField] public float radiusAvoidObstacle;

    public Vector3 velocity;
    public Quaternion quat;
    public float maxVelocity;
    public List<Transform> nearbyTransforms; 
    Collider boidCollider;

    public Collider getboidCollider { get { return boidCollider; } }

    // Start is called before the first frame update
    void Start() {
        boidCollider = GetComponent<Collider>();
       // nearbyObjRadius = 5;
       // boidObj = GetComponent<GameObject>();
        nearbyTransforms = GetNearbyObjTransforms(boidCollider);
    }

    public static Boid boid;

    void Awake() {
        boid = this;
    }
   

    public List<Transform> GetNearbyObjTransforms(Collider boidCollider) {
        List<Transform> context = new List<Transform>();
       // GameObject boidObj = boidCollider.GetComponent<GameObject>();

        //instancing a array of colliders ´which entails every collider in a given radius (-> neighborRadius)
        Collider[] contextColliders = Physics.OverlapSphere(boidCollider.transform.position, nearbyObjRadius);
        // sorting out own collider and putting rest in list context
        foreach (Collider c in contextColliders) {
            if (c != boidCollider) {
                context.Add(c.transform);
            }

        }
        return context;
    }



}
