using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Boid))]


public class BoidContainerBehavior : MonoBehaviour
{
    private Boid boid;

    public float radius;
    public float boundaryForce;

    // Start is called before the first frame update
    void Start() {
        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void Update()
    {
        // move toward center if > radius
        if (boid.transform.position.magnitude > radius) {

            //               direction from where we are           increase the further u get outside                             smoothing out 
            boid.velocity += this.transform.position.normalized * (radius - boid.transform.position.magnitude) * boundaryForce * Time.deltaTime; 
        }
    }
}
