using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;



[BurstCompile]
public struct BoidAlignmentJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    public float deltaTime;
    public Vector3 vel;
    //public sth  BoidsList;
    public float radius;
   // public TransformAccessArray BoidsTransformArray;

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    
    public NativeArray<Vector3> velocity;

    [NativeSetThreadIndex]
    private int m_ThreadIndex;  // need another index inside job for nested for loop

    public void Execute(int i, TransformAccess transform) {

        //  transform.localPosition += transform.localRotation * math.float3(0, 0, 1);


        vel = new Vector3(1, 1, 0);
       // transform.position += vel * deltaTime;
        //transform.localPosition = vel;// * deltaTime;   


        var average_alignment = Vector3.zero;
        var found = 0;
        radius = 5; // change here


        // loops over all transforms 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {     //S HARCODED HERE 

     //   for (m_ThreadIndex = 0; m_ThreadIndex < BoidsPositionArray.Length; m_ThreadIndex++) {
            var curTransform = BoidsPositionArray[j]; 
            var diff = curTransform - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself

                // Boid curBoid = curTransform.GetComponent<Boid>();  // gets boid from transform
                // var curBoidData = curBoid.Update(); // get BoidData

                //  average_alignment += curBoid.GetVelocity();
                // prob bug here -> get boid from transfrom, boid than has Vel 
              //  var curVelocity = velocity[j];
                var curVelocity = BoidsPositionArray[j];

                average_alignment += curVelocity;
                found += 1;


            }

        }


        if (found > 0) {


            average_alignment = average_alignment / found;
            velocity[i] += average_alignment;

           // transform.position += vel * deltaTime;

            // set vel here

            //boid.vel += Vector3.Lerp(boid.vel, average_alignment, Time.deltaTime); // gets closer to vel we want to achieve
        }
        

        // reset
        found = 0;

    }


}