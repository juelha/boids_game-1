using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidStayJob : IJobParallelForTransform {

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        float radius = 15f;
        float weight = 0.7f;

        var center = Vector3.zero;
        var stayVec = Vector3.zero;
        var found = 0;

        // loops over all transforms 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {
            center += BoidsPositionArray[j];
            center /= BoidsPositionArray.Length;
        }
        Vector3 centerOffset = center - (Vector3)transform.position;
        float t = centerOffset.magnitude / radius;
        if (t > 0.9f) {
            stayVec = centerOffset * t * t;
        }

        if (transform.position.y < -25f) {
            stayVec = centerOffset * t;
        }

        if (transform.position.y > 3f) {
            stayVec = centerOffset * t;
        }

        stayVec = Vector3.zero;

        velocity[i] += stayVec;

    }


}