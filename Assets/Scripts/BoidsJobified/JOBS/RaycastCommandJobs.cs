using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;
struct RaycastCommandJobs : IJobParallelFor {
    [ReadOnly] public ushort raycastDistance;
    [ReadOnly] public NativeArray<Vector3> velocities;
    [ReadOnly] public NativeArray<Vector3> positions;
    //[ReadOnly] public LayerMask layerMask;
    public NativeArray<RaycastCommand> Raycasts;

    public void Execute(int i) {
        Raycasts[i] = new RaycastCommand(positions[i], velocities[i], raycastDistance);//, layerMask);
    }
}