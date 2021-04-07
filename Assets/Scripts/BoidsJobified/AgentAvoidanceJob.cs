using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

//[CreateAssetMenu(menuName = "Flock/Behavior/Agent Avoidance")]
public struct AgentAvoidanceJob : IJobParallelForTransform {

    public float deltaTime;
    public float radius;

    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;

    public NativeArray<Vector3> velocity;


    public void Execute(int i, TransformAccess transform) {

        //add all points together and average
        var avoidanceVector = Vector3.zero;
        int found = 0;
        radius = 5; // change here

        /*

        foreach (Transform item in filteredContext) {
            Collider agentCollider = item.GetComponent<Collider>();
            //get nearest point of the collider of the agent
            Vector3 closestPointofAgent = agentCollider.ClosestPoint(agent.transform.position);

            // if agent is near -> avoid
            if (Vector3.SqrMagnitude(closestPointofAgent - agent.transform.position) < flock.SquareAgentAvoidanceRadius) {
                found++;
                //setting vector pointing away from neighbor
                avoidanceVector += (Vector3)(agent.transform.position - closestPointofAgent);
                if (Vector3.SqrMagnitude(closestPointofAgent - agent.transform.position) < 0.25f) {
                    Debug.Log("Boid gefressen");
                }
            }
        }
        */


        if (found > 0)
            avoidanceVector /= found;

        avoidanceVector = new Vector3(1, 1, 0);

    }
}
