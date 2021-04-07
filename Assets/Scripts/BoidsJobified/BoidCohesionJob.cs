using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;



[BurstCompile]
public struct BoidCohesionJob : IJobParallelForTransform {  // IJobParallelFor can run same logic over a list of items

    public float deltaTime;
    public Vector3 vel;
    //public sth  BoidsList;
    public float radius;
    // public TransformAccessArray BoidsTransformArray;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;

    public NativeArray<Vector3> velocity;

    public void Execute(int i, TransformAccess transform) {

        //  transform.localPosition += transform.localRotation * math.float3(0, 0, 1);


        vel = new Vector3(1, 1, 0);
        // transform.position += vel * deltaTime;
        //transform.localPosition = vel;// * deltaTime;   


        var average_cohesion = Vector3.zero;
        var found = 0;
        radius = 5; // change here

        // loops over all transforms 
        for (int j = 0; j < BoidsPositionArray.Length; j++) {     //S HARCODED HERE 

            var curPosition = BoidsPositionArray[j];
            var diff = curPosition - transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_cohesion += diff;
                found += 1;


            }
            else { // test
                continue;
                //average_cohesion = new Vector3(1, 1, 0);
            }
        }


        if (found > 0) {


            average_cohesion = average_cohesion / found;
            velocity[i] += average_cohesion;
           // velocity[i] = velocity[i] * 5;
            // transform.position += vel * deltaTime;

            // set vel here

            //boid.vel += Vector3.Lerp(boid.vel, average_alignment, Time.deltaTime); // gets closer to vel we want to achieve
        } 
                

        // reset
        found = 0;

    }


}