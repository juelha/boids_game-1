using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public CharacterController controller;
    public float speed = 3f;
    public float slowDownFor = 3f;
    public float speedUpFor = 3f;
    public float waitForNextSpeed = 5f;
    private bool isFast = false;


    private Rigidbody rb;
    // Start is called before the first frame update
    //public float sensibility;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameState.GetState() != GameState.State.Playing)
        {
            return;
        }
        //Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //float midPoint = (transform.position - Camera.main.transform.position).magnitude * 0.5f;
        //transform.LookAt(mouseRay.origin * sensibility  + mouseRay.direction * midPoint);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // if movement would be directed backwards (negative z), player can't move in that direction
        if (z < 0) z = 0;

        //slower left and right
        Vector3 move = rb.transform.right * x / 3 + rb.transform.forward * z;

        //controller.Move(move * speed * Time.deltaTime);
        rb.AddForce(move * speed * Time.deltaTime, ForceMode.VelocityChange);

        // check for upper boundary (water surface, currently at y = 8.5)
        // because the distance btw surface and shark is measured from the midpoint of the shark model
        // the shark has to be stopped before the y surface level
        // in addition we want the third person camera to always be underwater, so the shark has to stay below
        // the point where the third person player camera (main camera) would breach the surface
        // test showed: if the camera is directly above the shark through rotation
        // and the shark is at y = 6.9 or below the main camera is still underwater
        if (transform.position.y > 6.9f)
        {
            // keep player below threshold 
            transform.position = new Vector3(transform.position.x, 6.9f, transform.position.z);
        }

        //If player presses "space", shark get's a speed up (calls f speedUp() )
        if (Input.GetButton("Jump") && !isFast)
        {
            speedUp();
        }
    }

    //Speed up
    public void speedUp()
    {
        StartCoroutine(speedUpTime(speedUpFor, waitForNextSpeed));
    }

    IEnumerator speedUpTime(float time, float wait)
    {
        speed *= 2;
        isFast = true;
        yield return new WaitForSeconds(time);
        speed /= 2;
        yield return new WaitForSeconds(wait);
        isFast = false;
    }


    //Slow down function
    IEnumerator slowDownTime(float time)
    {
        speed /= 2;
        yield return new WaitForSeconds(time);
        speed *= 2;
        //Debug.Log("Here 74
    }

    public void slowDown()
    {
        StartCoroutine(slowDownTime(slowDownFor));
    }

}
