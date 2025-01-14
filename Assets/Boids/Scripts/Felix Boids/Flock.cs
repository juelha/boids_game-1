﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;

    public FlockAgent xtrapointsBoid;

    public FlockAgent xtratimeBoid;

    public PlayerMovement player;
    //List of all agents in flock
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;
    private Flock myFlock;

    [Range(1, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;
    [Range(1, 20)]
    public int number_of_xtra_points_boids = 5;
    [Range(1, 20)]
    public int number_of_xtra_time_boids = 5;
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f; //seperation radius = avoidanceRadiusMultiplier * neighborRadius
    [Range(0f, 10f)]
    public float obstacleAvoidanceRadius = 2f;
    [Range(0f, 10f)]
    public float agentAvoidanceRadius = 4f;
    [Range(0f, 100f)]
    public float huntingRadius = 20f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    float squareObstacleAvoidanceRadius;
    float squareAgentAvoidanceRadius;
    float squareHuntingRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    public float SquareObstacleAvoidanceRadius {get { return squareObstacleAvoidanceRadius; } }

    public float SquareAgentAvoidanceRadius { get { return squareAgentAvoidanceRadius; } }

    public float SquareHuntingRadius { get { return squareHuntingRadius; } }

    public PlayerMovement getPlayer{ get { return player; }}
    // Start is called before the first frame update
    void Start()
    {
        //square is used because for comparing the magnitude of vectors squareroots have to be calculated
        //calc sqrt is quite labour intensive for computer -> instead comparing squares
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
        squareObstacleAvoidanceRadius = obstacleAvoidanceRadius * obstacleAvoidanceRadius;
        squareAgentAvoidanceRadius = agentAvoidanceRadius * agentAvoidanceRadius;
        squareHuntingRadius = huntingRadius * huntingRadius;
        //instatiation of the flock, determining the number of boids in the starting count
        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                //insideUnitSphere returns random point within a sphere of radius 1, used for setting starting point
                // AgentDensity determines how far they start away from each other
                Random.insideUnitSphere * startingCount * AgentDensity, 
                //setting a random rotation
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            newAgent.name = "Agent " + i; // not so relevant
            newAgent.Initialize(this); //Assign boid to this flock
            agents.Add(newAgent);
           
        }
        if(xtrapointsBoid != null)
        {
            //instatiation of the flock, determining the number of boids in the starting count
            for (int i = 0; i < number_of_xtra_points_boids; i++)
            {
                FlockAgent newAgent = Instantiate(
                    xtrapointsBoid,
                    //insideUnitSphere returns random point within a sphere of radius 1, used for setting starting point
                    // AgentDensity determines how far they start away from each other
                    Random.insideUnitSphere * startingCount * AgentDensity,
                    //setting a random rotation
                    Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                    transform
                    );
                newAgent.name = "XtraPointsBoid " + i; // not so relevant
                newAgent.Initialize(this); //Assign boid to this flock
                agents.Add(newAgent);

            }
        }
        if (xtratimeBoid != null)
        {
            for (int i = 0; i < number_of_xtra_time_boids; i++)
            {
                FlockAgent newAgent = Instantiate(
                    xtratimeBoid,
                    //insideUnitSphere returns random point within a sphere of radius 1, used for setting starting point
                    // AgentDensity determines how far they start away from each other
                    Random.insideUnitSphere * startingCount * AgentDensity,
                    //setting a random rotation
                    Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                    transform
                    );
                newAgent.name = "XtraTimeBoid " + i; // not so relevant
                newAgent.Initialize(this); //Assign boid to this flock
                agents.Add(newAgent);

            }
        }

    }

    // Update is called once per frame
    void Update()
    {   // agents = list of agents
        foreach (FlockAgent agent in agents)
        {
            if (agent != null)
            {
                List<Transform> context = GetNearbyObjects(agent);

                //call calculateMove funcion from class behavior to get directional vector
                Vector3 move = behavior.CalculateMove(agent, context, this);
                move *= driveFactor;
                //restricting speed to maxSpeed
                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    move = move.normalized * maxSpeed;
                }
                agent.Move(move);
            }
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        if(agent != null)
        {
            //instancing a array of colliders ´which entails every collider in a given radius (-> neighborRadius)
            Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
            // sorting out own collider and putting rest in list context
            foreach (Collider c in contextColliders)
            {
                if (c != agent.AgentCollider)
                {
                    context.Add(c.transform);   
                }
         
            }
           
        }
        return context;
    }

}
