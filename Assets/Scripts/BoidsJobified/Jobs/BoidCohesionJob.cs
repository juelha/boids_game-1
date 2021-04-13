using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidCohesionJob : IJobParallelForTransform {

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int radius = 5;
        float weight = 0.2f;

        var average_cohesion = Vector3.zero;
        var found = 0;

        // loops over all transforms 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {
            var curPosition = BoidsPositionArray[j];
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_cohesion += diff;
                found++;
            }
        }
        if (found > 0) {
            average_cohesion /= found;  // normalizing
            average_cohesion *= weight;  // change for getting desired effect in flock behavior
            velocity[i] += average_cohesion;
        }


        var test = Vector3.zero;
        // velocity[i] = test;

    }


}