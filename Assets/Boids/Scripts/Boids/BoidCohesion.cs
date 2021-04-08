using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // to use .Where



[RequireComponent(typeof(Boid))]

public class BoidCohesion : MonoBehaviour 
{

    private Boid boid;

    public float radius;

    // Start is called before the first frame update
    void Start() 
    {
        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void Update() 
    {
        

        // init
        var average = Vector3.zero;
        var found = 0;
        // Create a list  
        List<Boid> BoidsList = new List<Boid>();
        

        BoidsList.Add(FindObjectOfType<Boid>());
        var boids = FindObjectOfType<Boid>(); // returns array of all boids in our scene
                                              // prob: new array each scene, v expensive
                                              // TODO: ds pointer
                                              // TODO: research octree (mentioned in vid)






        foreach (var boid in BoidsList) {
            var diff = boid.transform.position - this.transform.position;
            if ((diff.magnitude < radius) && (diff.magnitude > 0)) { // checks if in radius && not itself
                average += diff;
                found += 1;
                

            }
        }

        if (found > 0) {
            average = average / found;
            boid.velocity += Vector3.Lerp(Vector3.zero, average, average.magnitude/ radius); //the closer the less impact
        }
       

    }
}
