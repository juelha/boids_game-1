using Unity.Jobs;
using Unity.Collections;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;


[BurstCompile]

// create a job
public struct BoidUpdateJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    // add array of dataobjs for job to iterate over
    // public NativeArray<Boid.Data> BoidDataArray;  // why NativeArray? <-  special type of array that was designed to work optimally with jobs

   // public NativeArray<Vector3> velocity;  // the velocities from AccelerationJob

    public float deltaTime;
    public Vector3 vel;
    

    public void Execute(int i, TransformAccess transform) {


        vel = new Vector3(1, 1, 0);
        // move the given transforms forward in their local coordinate system
        //  transform.localPosition += transform.localRotation * math.float3(0, 0, 1);

        transform.position = vel ;// * deltaTime;
       // transform.localPosition = vel;// * deltaTime;   
    }

   // public void Execute(int index) {
        // how it works:
        // 1. pull dataObj out of array
        // 2. do our work
        // 3. put it back in

     //   var curDataObj = BoidDataArray[index];  // 1. ref to current data obj 
    //    curDataObj.Update();                    // 2. do stuff



       // BoidDataArray[index] = curDataObj;      // 3. pass the data back in the array using index
   // }

}

