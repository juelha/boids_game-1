using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidStayinRadiusJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    public Vector3 center;
    public int radius;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        var center = Vector3.zero;
        var centerOffset = Vector3.zero;

        // create vector for center
        for (int j = 0; j < BoidsPositionArray.Length; j++) {  // loops over all transforms 
            var curPosition = BoidsPositionArray[j];
            center += curPosition;
        }
        radius = 500; 

        centerOffset = center - transform.position;
        float t = centerOffset.magnitude / radius;

        if (t > 0.9f) {
           // og: centerOffset = * t * t;
            velocity[i] -= centerOffset;
        }
    }


}