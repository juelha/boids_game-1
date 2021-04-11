using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct BoidAvoidPlayerJob : IJobParallelForTransform {

    public float radius;
    public NativeArray<Vector3> velocity;
    [ReadOnly] public NativeArray<bool> isHitObstacles;
    [ReadOnly] public NativeArray<Vector3> hitNormals;

    public void Execute(int i, TransformAccess transform) {

        // in case of disaster change these:
        int weight = 1;

        var avoidanceVector = Vector3.zero;
        var found = 0;
        var rotationSpeed = 2;

      //  Debug.DrawRay(transform.position, velocity[i] * 2, Color.yellow);  // surprisingly this works but the other stuff doesnt

        // at this point we know if boid is about to hit sth 
        if (isHitObstacles[i]) {  // if the ray cast from a boid hits sth -> avoid
                                  // avoidanceVector = Vector3.Reflect(velocity[i], hitNormals[i]);  // just making shit up at this point
                                  //Debug.DrawRay(transform.position, velocity[i] * 2, Color.yellow);
                                  // avoidanceVector = (-velocity[i]);
                                  // transform.rotation = Quaternion.LookRotation(transform.position + Vector3.up,- velocity[i]);
                                  //  Debug.Log(isHitObstacles[i]);
                                  // velocity[i] = Vector3.Reflect(velocity[i], Vector3.right);
            found++;

            //   Debug.Log("velocity[i]");
            //   Debug.Log(velocity[i]);

            // calc new vector: 
            avoidanceVector = Vector3.Reflect(velocity[i], hitNormals[i]); // doesnt do what i want it to
                                                                           // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(avoidanceVector), rotationSpeed * deltaTime);

            avoidanceVector *= weight;
            velocity[i] += avoidanceVector;

        }

    }
}