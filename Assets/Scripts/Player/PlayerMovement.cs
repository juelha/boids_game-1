using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 3f;
    public float slowDownFor = 3f;
    public float speedUpFor = 3f;
    public float waitForNextSpeed = 5f;
    private bool isFast = false;
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //float midPoint = (transform.position - Camera.main.transform.position).magnitude * 0.5f;
        //transform.LookAt(mouseRay.origin * sensibility  + mouseRay.direction * midPoint);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // if Player would move backwards (z > 0), stop movement -> shark can't swim backwards
        if (z < 0) z = 0;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // check for upper boundary (water surface, currently at y = 8.5)
        if (transform.position.y > 7.5f)
        {
            // keep player position and outer camera under the water surface 
            transform.position = new Vector3(transform.position.x, 7.5f, transform.position.z);
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
