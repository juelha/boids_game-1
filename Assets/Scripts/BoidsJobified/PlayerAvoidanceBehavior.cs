using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Flock/Behavior/Agent Avoidance")]
[RequireComponent(typeof(Boid))]
[RequireComponent(typeof(Player))] // rename scripts
public class PlayerAvoidanceBehavior : MonoBehaviour {


    Boid agent;
    List<Transform> context;
    Player player;
    Boid boid;
    int radiusAvoidPlayer;


    void Start() {
        boid = GetComponent<Boid>();
        player = GetComponent<Player>();
        radiusAvoidPlayer = 2; 
    }

    // Update is called once per frame
    void Update() {

        //init
        Vector3 avoidanceVector = Vector3.zero;
        int nAvoid = 0;

        Collider playerCollider = player.GetComponent<Collider>();
       // Collider playerPosition = player.transform.position;

      //  Vector3 closestPointofAgent = playerCollider.ClosestPoint(this.transform.position);
        // .ClosestPoint calculates closest point between playerCollider and position of boid
        Vector3 closestPointofAgent = playerCollider.ClosestPoint(this.transform.position);

        var diff = closestPointofAgent - this.transform.position;

        // if player is near -> avoid
        if (diff.magnitude < radiusAvoidPlayer) {
            nAvoid++;
            // avoidanceVector is opposite direction
            avoidanceVector += (Vector3)(-diff);
            /*
            if (Vector3.SqrMagnitude(diff) < 0.25f) {
                Debug.Log("Boid gefressen");
            }
            */
        }

        if (nAvoid > 0) {
            avoidanceVector /= nAvoid;
            boid.velocity += avoidanceVector; // bug here?

        }

    }

}
