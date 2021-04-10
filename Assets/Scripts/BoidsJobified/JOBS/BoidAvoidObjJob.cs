using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;


public struct BoidAvoidObjJob : IJobParallelForTransform {

    public float deltaTime;
    public NativeArray<Vector3> velocity;
    [ReadOnly] public NativeArray<bool> isHitObstacles;
    [ReadOnly] public NativeArray<Vector3> hitNormals;


    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int weight = 12;

        var avoidanceVector = Vector3.zero;
        var found = 0;
        var rotationSpeed = 2; 

        Debug.DrawRay(transform.position, velocity[i] * 2, Color.yellow);  // surprisingly this works but the other stuff doesnt

        // at this point we know if boid is about to hit sth 
        if (isHitObstacles[i]) {  // if the ray cast from a boid hits sth -> avoid
                                  // avoidanceVector = Vector3.Reflect(velocity[i], hitNormals[i]);  // just making shit up at this point
            //Debug.DrawRay(transform.position, velocity[i] * 2, Color.yellow);
           // avoidanceVector = (-velocity[i]);
           // transform.rotation = Quaternion.LookRotation(transform.position + Vector3.up,- velocity[i]);
            Debug.Log(isHitObstacles[i]);
           // velocity[i] = Vector3.Reflect(velocity[i], Vector3.right);
            found++;

            // calc new vector: 
            avoidanceVector = Vector3.Reflect(velocity[i], hitNormals[i]);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(avoidanceVector), rotationSpeed * deltaTime);

            velocity[i] += avoidanceVector; 
            //  var test = Vector3.zero;
            //velocity[i] = test;
        }



        /*
        if (found > 0) {
            avoidanceVector /= found;  // normalizing
            avoidanceVector *= weight; // change for getting desired effect in flock behavior
            velocity[i] *= -1 ; // opposite direction
        }*/

    }

    /*
    // takes first ray that doesnt hit anything as direction
    // prob: physics baby 
    // sol: end my fucking life
    Vector3 ObstacleRays() {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(Transform.position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        return forward;
    }


    */










}


