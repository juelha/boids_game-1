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


        if (isHitObstacles[i]) {  // if the ray cast from a boid hits sth -> avoid
            avoidanceVector = Vector3.Reflect(velocity[i], hitNormals[i]);  // just making shit up at this point
            found++;
        }


        if (found > 0) {
            avoidanceVector /= found;  // normalizing
            avoidanceVector *= weight; // change for getting desired effect in flock behavior
            velocity[i] += avoidanceVector;
        }

    }
}
