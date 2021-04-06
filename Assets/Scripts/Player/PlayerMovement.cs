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
        //Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //float midPoint = (transform.position - Camera.main.transform.position).magnitude * 0.5f;
        //
        //transform.LookAt(mouseRay.origin * sensibility  + mouseRay.direction * midPoint);


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //verhindern, dass er rückwärts kann
        if (z < 0) z = 0;

        //slower left and right
        Vector3 move = rb.transform.right * x / 3 + rb.transform.forward * z;

        //controller.Move(move * speed * Time.deltaTime);
        rb.AddForce(move * speed * Time.deltaTime, ForceMode.VelocityChange);

        // check for upper boundary (water surface, currently at y = 8.5)
        if (transform.position.y > 7.9f)
        {
            // keep player position at 8.4
            transform.position = new Vector3(transform.position.x, 7.9f, transform.position.z);
        }

        //If u press space, speed up
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
