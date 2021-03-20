using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// test
[RequireComponent(typeof(Boid))]

public class BoidRules : MonoBehaviour {

    private Boid boid;

    public float separationRadius;
    public float radius;

    // init forces for rules
    public float separationForce;
    public float alignmentForce;
    public float cohesionForce;



    // Start is called before the first frame update
    void Start() {
        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void Update() {


        // init
        var average_alignment = Vector3.zero;
        var average_cohesion = Vector3.zero;
        var average_separation = Vector3.zero;
        var found = 0;
        // Create a list  
        List<Boid> BoidsList = new List<Boid>();
        BoidsList.Add(FindObjectOfType<Boid>());


        // align
        foreach (var boid in BoidsList) {
            var diff = boid.transform.position - this.transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_alignment += boid.velocity;
                found += 1;


            }
        }

        if (found > 0) {
            average_alignment = average_alignment / found;
            boid.velocity += Vector3.Lerp(boid.velocity, average_alignment, Time.deltaTime); // gets closer to velocity we want to achieve
        }

        // reset
        found = 0; 

        // cohesion
        foreach (var boid in BoidsList) {
            var diff = boid.transform.position - this.transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_cohesion += diff;
                found += 1;


            }
        }

        if (found > 0) {
            average_cohesion = average_cohesion / found;
            boid.velocity += Vector3.Lerp(Vector3.zero, average_cohesion, average_cohesion.magnitude / radius); //the closer the less impact
        }


        BoidsList.Add(FindObjectOfType<Boid>());

        // reset
        found = 0;

        // separation
        foreach (var boid in BoidsList) {
            var diff = boid.transform.position - this.transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average_separation += diff; // opposite of cohesion
                found += 1;


            }
        }

        if (found > 0) {
            average_separation = average_separation / found;
            boid.velocity -= Vector3.Lerp(Vector3.zero, average_separation, average_separation.magnitude / radius) * separationForce;

        }


    }
}
