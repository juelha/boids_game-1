using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidSeparateJob : IJobParallelForTransform {

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int radius = 5;
        float weight = 1.3f;

        var average_sep = Vector3.zero;
        var found = 0;

        // loops over all transforms 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {
            var curPosition = BoidsPositionArray[j];
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_sep -= diff;
                found++;
            }
        }
        if (found > 0) {
            average_sep /= found;  // normalizing
            average_sep *= weight;  // change for getting desired effect in flock behavior
            velocity[i] += average_sep;
        }


    }


}