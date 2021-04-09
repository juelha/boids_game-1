using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public struct BoidAlignmentJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

  
    public float radius;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        var average_alignment = Vector3.zero;
        var found = 0;
        radius = 5; // change here

        // loops over all positions 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {     
            var curPosition = BoidsPositionArray[j]; 
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                var curVelocity = BoidsPositionArray[j];
                average_alignment += curVelocity;
                found += 1;
            }
        }
        if (found > 0) {
            average_alignment /= found;
            velocity[i] += average_alignment;
        }
    }
}