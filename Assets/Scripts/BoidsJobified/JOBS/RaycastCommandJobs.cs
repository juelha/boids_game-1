using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;

/*
struct RaycastCommandJobs : IJobParallelFor {
    [ReadOnly] public ushort raycastDistance;
    [ReadOnly] public NativeArray<Vector3> velocities;
    [ReadOnly] public NativeArray<Vector3> positions;
    //[ReadOnly] public LayerMask layerMask;
    public NativeArray<RaycastCommand> Raycasts;

    public void Execute(int i) {
        Raycasts[i] = new RaycastCommand(positions[i], velocities[i], raycastDistance);//, layerMask);
        Debug.DrawRay(positions[i], velocities[i] * raycastDistance, Color.yellow);  // works!!!
    }
}*/

struct RaycastCommandJobs : IJobParallelForTransform {
    [ReadOnly] public ushort raycastDistance;
    [ReadOnly] public NativeArray<Vector3> velocities;
   // [ReadOnly] public NativeArray<Vector3> positions;
    //[ReadOnly] public LayerMask layerMask;
    public NativeArray<RaycastCommand> Raycasts;

    public void Execute(int i, TransformAccess transform) {
        Raycasts[i] = new RaycastCommand(transform.position, velocities[i], raycastDistance);//, layerMask);
        Debug.DrawRay(transform.position, velocities[i] * raycastDistance, Color.yellow);  // works!!!
    }
}


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