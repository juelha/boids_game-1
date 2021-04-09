using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Boid))]

public class ObstacleAvoidanceBehavior : MonoBehaviour {

    private Boid boid;
    int radiusAvoidObstacle;
    // List<Transform> nearbyTransforms;

    // Start is called before the first frame update
    void Start() {
        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void Update() {

        //init
        Vector3 avoidanceVector = Vector3.zero;
        int nAvoid = 0;
       

        foreach (Transform obstacleTransform in boid.nearbyTransforms) {

            Collider obstacleCollider = obstacleTransform.GetComponent<Collider>();
            //get nearest point of the collider of the obstacle
            Vector3 closestPointofObstacle = obstacleCollider.ClosestPoint(boid.transform.position);

           // var diff = closestPointofObstacle - this.transform.position;

            // if obstacle is near -> avoid
            if (closestPointofObstacle.magnitude < radiusAvoidObstacle) {
                nAvoid++;
                // avoidanceVector is opposite direction
                avoidanceVector += (Vector3)(-closestPointofObstacle);
            }

        }

        if (nAvoid > 0) {
            avoidanceVector /= nAvoid;
            boid.velocity += avoidanceVector; // bug here?

        }

    }

}
