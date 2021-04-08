using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Object Avoidance")]
public class ObjectAvoidanceBehavior : FilteredFlockBehavior
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
            Collider obstacleCollider = item.GetComponent<Collider>();
            //get nearest point of the collider of the obstacle
            Vector3 closestPointofObstacle = obstacleCollider.ClosestPoint(agent.transform.position);

            // if obstacle is near -> avoid
            if (Vector3.SqrMagnitude(closestPointofObstacle - agent.transform.position) < flock.SquareObstacleAvoidanceRadius)
            {
                nAvoid++;
                //setting vector pointing away from neighbor
                avoidanceMove += (Vector3)(agent.transform.position - closestPointofObstacle);
            }
        }

        
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
}
