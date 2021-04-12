using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;

struct BoidRaycastCommandJobs : IJobParallelFor {
    [ReadOnly] public float raycastDistance;
    [ReadOnly] public NativeArray<Vector3> velocities;
    [ReadOnly] public NativeArray<Vector3> positions;
    //[ReadOnly] public LayerMask layerMask;
    // public NativeArray<SpherecastCommand> Raycasts;
    public NativeArray<RaycastCommand> Raycasts;


    public void Execute(int i) {
        //  Raycasts[i] = new SpherecastCommand(positions[i], raycastDistance, velocities[i]);
        Raycasts[i] = new RaycastCommand(positions[i], velocities[i], raycastDistance);//, layerMask);
       Debug.DrawRay(positions[i], velocities[i] * raycastDistance, Color.yellow);  // works!!!

    }
}


/*
struct BoidRaycastCommandJobs : IJobParallelFor {
    [ReadOnly] public ushort raycastDistance;
    [ReadOnly] public NativeArray<Vector3> velocities;
    [ReadOnly] public NativeArray<Vector3> positions;
    // [ReadOnly] public NativeArray<Vector3> positions;
    //[ReadOnly] public LayerMask layerMask;
    public NativeArray<bool> isHitObstacles;
  //  public NativeArray<RaycastCommand> Raycasts;
   // public NativeArray<RaycastHit> Hits;
    public void Execute(int i) {
        var results = new NativeArray<RaycastHit>(1, Allocator.Temp);
        var commands = new NativeArray<SpherecastCommand>(1, Allocator.Temp);
        commands[0] = new SpherecastCommand(positions[i], raycastDistance, velocities[i]);
        // Schedule the batch of sphere casts
        var handle = SpherecastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));
        // Wait for the batch processing job to complete
        handle.Complete();
        // Copy the result. If batchedHit.collider is null, there was no hit
        RaycastHit batchedHit = results[0];
        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
        // 
        //
        //  Raycasts[i] = new RaycastCommand(positions[i], velocities[i], raycastDistance);//, layerMask);
        Debug.DrawRay(positions[i], velocities[i] * raycastDistance, Color.yellow);  // works!!!
        /*
        RaycastHit hit;
        if (Physics.SphereCast(positions[i],2, velocities[i], out hit, raycastDistance)) {
            isHitObstacles[i] = true;
           // return true;
        }
        else { }
            isHitObstacles[i] = false;
        */


// Schedule the batch of raycasts
// var handle = RaycastCommand.ScheduleBatch(Raycasts, results, 1);

// Wait for the batch processing job to complete
//  handle.Complete();

// Copy the result. If batchedHit.collider is null there was no hit
//  RaycastHit batchedHit = results[0];
//  Hits[i] = results[0]; */


/*
Vector3 ObstacleRays() {
    Vector3[] rayDirections = BoidHelper.directions;
    for (int i = 0; i < rayDirections.Length; i++) {
        Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
        Ray ray = new Ray(position, dir);
        if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
            return dir;
        }
    }
    return forward;
}
*/