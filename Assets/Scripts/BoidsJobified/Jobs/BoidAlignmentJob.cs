using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public struct BoidAlignmentJob : IJobParallelForTransform {

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int radius = 10;
        float weight = 0.8f;

        var average_alignment = Vector3.zero;
        var found = 0;

        // loops over all positions 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {
            var curPosition = BoidsPositionArray[j];
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                var curVelocity = BoidsPositionArray[j];
                average_alignment += curVelocity;
                found++;
            }
        }
        if (found > 0) {
            average_alignment /= found;   // normalizing
            average_alignment *= weight;  // change for getting desired effect in flock behavior
            velocity[i] += average_alignment;
        }
    }
}