using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Agent Avoidance")]
public class AgentAvoidanceBehavior : FilteredFlockBehavior
{

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //if no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector3.zero;

        //add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            Collider agentCollider = item.GetComponent<Collider>();
            //get nearest point of the collider of the agent
            Vector3 closestPointofAgent = agentCollider.ClosestPoint(agent.transform.position);

            // if agent is near -> avoid
            if (Vector3.SqrMagnitude(closestPointofAgent - agent.transform.position) < flock.SquareAgentAvoidanceRadius)
            {
                nAvoid++;
                //setting vector pointing away from neighbor
                avoidanceMove += (Vector3)(agent.transform.position - closestPointofAgent);
                if(Vector3.SqrMagnitude(closestPointofAgent - agent.transform.position) < 0.25f)
                {
                    Debug.Log("Boid gefressen");
                }
            }
        }

        
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
}
