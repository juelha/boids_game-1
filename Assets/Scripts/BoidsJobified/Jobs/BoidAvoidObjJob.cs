using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;


public struct BoidAvoidObjJob : IJobParallelForTransform {

    public NativeArray<Vector3> velocity;
    [ReadOnly] public NativeArray<bool> isHitObstacles;
    [ReadOnly] public NativeArray<Vector3> hitNormals;


    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int weight = 12;

        var avoidanceVector = Vector3.zero;
        var found = 0;

        Debug.DrawRay(transform.position, velocity[i] * 2, Color.yellow);  // surprisingly this works but the other stuff doesnt

        // at this point we know if boid is about to hit sth 
        if (isHitObstacles[i]) {
            // calc new vector: 

         //   Debug.Log("HIT");

            avoidanceVector = Vector3.Reflect(velocity[i], hitNormals[i]);
            avoidanceVector *= weight;
            velocity[i] += avoidanceVector;
          //  velocity[i] = avoidanceVector;


        }


    }


}


