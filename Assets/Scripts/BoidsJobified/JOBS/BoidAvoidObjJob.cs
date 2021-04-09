using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;


public struct BoidAvoidObjJob : IJobParallelForTransform {

    public float radius;
    [ReadOnly] public NativeArray<Vector3> NearbyObjsPositionArray;  // init with ray casting in boid manager
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        var avoidanceVector = Vector3.zero;
        var found = 0;
        radius = 5; // change here

        // loops over all postions 
        for (int j = 0; j < NearbyObjsPositionArray.Length; j++) {
            var curPosition = NearbyObjsPositionArray[j];
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                var curVelocity = NearbyObjsPositionArray[j];
                avoidanceVector += curVelocity;
                found += 1;
            }
        }

        if (found > 0) {
            avoidanceVector /= found;
            velocity[i] += avoidanceVector;
        }

    }
}
