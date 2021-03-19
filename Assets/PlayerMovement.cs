using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 10;
    // Start is called before the first frame update
    public float sensibility;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //float midPoint = (transform.position - Camera.main.transform.position).magnitude * 0.5f;
        //
        //transform.LookAt(mouseRay.origin * sensibility  + mouseRay.direction * midPoint);


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        controller.Move(move * speed * Time.deltaTime);
    }
}
