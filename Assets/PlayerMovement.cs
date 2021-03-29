using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 3f;
    public float slowDownFor = 3f;
    // Start is called before the first frame update
    //public float sensibility;

    void Start()
    {
        
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
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        controller.Move(move * speed * Time.deltaTime);
        
        // check for upper boundary (water surface, currently at y = 8.5)
        if (transform.position.y > 7.9f)
        {
            // keep player position at 8.4
            transform.position = new Vector3(transform.position.x, 7.9f, transform.position.z);
        }
    }

    //Slow down function
    IEnumerator ExecuteAfterTime(float time)
    {
        speed /= 2;
        yield return new WaitForSeconds(time);
        speed *= 2;
        //Debug.Log("Here 74
    }

    public void slowDown()
    {
        StartCoroutine(ExecuteAfterTime(slowDownFor));

    }


}
