using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Agent Huntig")]
public class AgentHuntingBehavior : FilteredFlockBehavior
{
    public GameObject player; 

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        
        if(player == null)
        {
            Debug.LogError("No player set to hunt");
        }
        Vector3 huntingMove = Vector3.zero;

        /*
       // Collider playerCollider = player.GetComponent<Collider>();
       // Vector3 closestPointofPlayer = playerCollider.ClosestPoint(agent.transform.position);
        Vector3 playerDistance = player.transform.position- agent.gameObject.transform.position;
        
        Debug.Log("ATP:" + agent.gameObject.transform.position);
        Debug.Log("PTP:" + player.transform.position);
        Debug.Log("PD"+playerDistance);

        if (Vector3.SqrMagnitude(playerDistance) < 30f)
        {
            huntingMove = playerDistance;
            Debug.Log("HUHU");
        }

        huntingMove = huntingMove.normalized;
        Debug.Log(huntingMove);
       
        */
        Vector3 playerPosition = player.gameObject.transform.position;
        Debug.Log("PlayerPos" + playerPosition);
        
        return huntingMove;
    }
}
