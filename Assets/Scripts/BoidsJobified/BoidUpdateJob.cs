using Unity.Jobs;
using Unity.Collections;

using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;


[BurstCompile]

// create a job
public struct BoidUpdateJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    public float deltaTime;
   // public TransformAccessArray BoidsTransformArray; 
    public Vector3 vel;  // = move
    
    public void Execute(int i, TransformAccess transform) {

      //  transform.localPosition += transform.localRotation * math.float3(0, 0, 1);

        vel = new Vector3(1, 1, 0);
        transform.position += vel  * deltaTime;
        //transform.localPosition = vel;// * deltaTime;   
    }


}

