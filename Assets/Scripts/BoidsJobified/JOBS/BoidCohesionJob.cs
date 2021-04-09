using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidCohesionJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    public float radius;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        var average_cohesion = Vector3.zero;
        var found = 0;
        radius = 5; // change here

        // loops over all transforms 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {     
            var curPosition = BoidsPositionArray[j];
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_cohesion += diff;
                found += 1;


            }
        }
        if (found > 0) {
            average_cohesion = average_cohesion / found;
            velocity[i] += average_cohesion;
        } 
    }


}