﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Stay In Radius")]
public class StayInRadiusBehavior : FlockBehavior
{
    public Vector3 center;
    public float radius = 15f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 centerOffset = center - (Vector3)agent.transform.position;
        float t = centerOffset.magnitude / radius;
        if (t > 0.9f)
        {
           return centerOffset * t * t;
        }

        if ( agent.transform.position.y < -25f)
        {
            return centerOffset * t;
        }

        if (agent.transform.position.y > -3f)
        {
            return centerOffset * t;
        }

        return Vector3.zero;
    } 
}
