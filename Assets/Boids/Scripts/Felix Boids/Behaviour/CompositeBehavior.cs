using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Composite3")]
public class CompositeBehavior : FlockBehavior
{
    public FlockBehavior[] behaviors;


    //array of wheight for combination with behaviors
    public float[] weights;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //handle data mismatch in case of not matching between behaviors and wheights
        if (weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        //set up move-vector
        Vector3 move = Vector3.zero;
        //iterate through behaviors
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flock) * weights[i];
            //if some movement is returned
            if (partialMove != Vector3.zero)
            {
                // if movement exceeds wheigth normalize and give it lenght stored in weight
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;

            }
        }
        return move;
    }

    
}
