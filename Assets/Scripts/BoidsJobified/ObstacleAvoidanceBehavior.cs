using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleAvoidanceBehavior : MonoBehaviour {

    Boid boid;
    int radiusAvoidObstacle;
    List<Transform> nearbyTransforms;

    // Update is called once per frame
    void Update() {

        //init
        Vector3 avoidanceVector = Vector3.zero;
        int nAvoid = 0;
       

        foreach (Transform obstacleTransform in nearbyTransforms) {

            Collider obstacleCollider = obstacleTransform.GetComponent<Collider>();
            //get nearest point of the collider of the obstacle
            Vector3 closestPointofObstacle = obstacleCollider.ClosestPoint(this.transform.position);

            var diff = closestPointofObstacle - this.transform.position;

            // if obstacle is near -> avoid
            if (diff.magnitude < radiusAvoidObstacle) {
                nAvoid++;
                // avoidanceVector is opposite direction
                avoidanceVector += (Vector3)(-diff);
            }

        }

        if (nAvoid > 0) {
            avoidanceVector /= nAvoid;
            boid.velocity += avoidanceVector; // bug here?

        }

    }

}
