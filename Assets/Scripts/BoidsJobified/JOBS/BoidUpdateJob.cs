using Unity.Jobs;
using Unity.Collections;

using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;



[BurstCompile]

// create a job
public struct BoidUpdateJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    
    public NativeArray<Vector3> velocity;  // the velocities from AccelerationJob

    public float deltaTime;
    public float maxVelocity;
    // public TransformAccessArray BoidsTransformArray; 
    public Vector3 vel;  // = move

    
    public void Execute(int i, TransformAccess transform) {

        maxVelocity = 5; 

        //  transform.localPosition += transform.localRotation * math.float3(0, 0, 1);

        // vel = new Vector3(1, 1, 0);
        // transform.position += vel  * deltaTime;
        // limit velocity 
        if (velocity[i].magnitude > maxVelocity) {
            velocity[i] = velocity[i].normalized * maxVelocity;
        }
        
        velocity[i] = transform.rotation * Vector3.up;  //upward part points in direction of movement
                                                        //  transform.position += (Vector3)velocity * Time.deltaTime;
        transform.position += velocity[i] * deltaTime;
        //transform.localPosition = vel;// * deltaTime;   
    }


}

