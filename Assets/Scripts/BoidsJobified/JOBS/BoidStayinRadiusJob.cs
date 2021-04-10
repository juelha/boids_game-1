using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidStayinRadiusJob : IJobParallelForTransform {  

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int radius = 500;
        int weight = 0;

        var center = Vector3.zero;
        var centerOffset = Vector3.zero;

        // create vector for center
        for (int j = 0; j < BoidsPositionArray.Length; j++) {  // loops over all transforms 
            var curPosition = BoidsPositionArray[j];
            center += curPosition;
        }
       

        centerOffset = center - transform.position;
        float t = centerOffset.magnitude / radius;

        if (t > 0.9f) {
            centerOffset *= weight;  // change for getting desired effect in flock behavior
            velocity[i] -= centerOffset;
        }
    }


}