using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidPlayerAvoidJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    public Vector3 playerPosition;
   // public GameObject playerObj;
    public float radius;
    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        var avoidanceVector = Vector3.zero;
        var found = 0;
        radius = 5; // change here

        // TODO: REDO THIS BIT
        var diff = playerPosition - transform.position;

        // if player is near -> avoid
        if (diff.magnitude < radius) {
            found++;
            // avoidanceVector is opposite direction
            avoidanceVector += (Vector3)(-diff);
            /*
            if (Vector3.SqrMagnitude(diff) < 0.25f) {
                Debug.Log("Boid gefressen");
            }
            */
        }

        if (found > 0) {
            avoidanceVector /= found;
            velocity[i] += avoidanceVector; // bug here?

        }
    }


}