using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Boid))]

public class BoidAlignment : MonoBehaviour {

    private Boid boid;

    public float radius;

    // Start is called before the first frame update
    void Start() {
        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void Update() {


        // init
        var average = Vector3.zero;
        var found = 0;
        // Create a list  
        List<Boid> BoidsList = new List<Boid>();


        BoidsList.Add(FindObjectOfType<Boid>());


        foreach (var boid in BoidsList) {
            var diff = boid.transform.position - this.transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average += boid.velocity;
                found += 1;


            }
        }

        if (found > 0) {
            average = average / found;
            boid.velocity += Vector3.Lerp(boid.velocity, average, Time.deltaTime); // gets closer to velocity we want to achieve
        }


    }
}
