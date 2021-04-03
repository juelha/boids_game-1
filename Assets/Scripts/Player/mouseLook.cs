using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    public Camera cam1;
    public Camera cam2;

    public float mouseSensitivity = 200f;
    public Transform playerBody;
    float xRotation = 0f;
    public float maxDegreeX = 45f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam1.enabled = true;
        cam2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Change
        if (Input.GetKeyDown(KeyCode.F))
        {
            cam1.enabled = !cam1.enabled;
            cam2.enabled = !cam2.enabled;
        }


        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation = -mouseY;
        playerBody.Rotate(Vector3.right * xRotation);
        playerBody.Rotate(Vector3.up * mouseX);
        //playerBody.Rotate(Vector3.up * mouseX + Vector3.left * mouseY );

        //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Debug.Log(xRotation);
        //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);



        //set z rotation to 0, for better gameplay
        var rotationVector = playerBody.transform.rotation.eulerAngles;
        rotationVector.z = 0f;

        //set x rotation for natural movement
        if(rotationVector.x < 180 && rotationVector.x > maxDegreeX)
        {
            rotationVector.x = 45f;
        }
        if (rotationVector.x < 360 - maxDegreeX && rotationVector.x > 180)
        {
            rotationVector.x = 315f;
        }
        //Debug.Log(rotationVector.x);

        playerBody.transform.rotation = Quaternion.Euler(rotationVector);


        //playerBody.transform.localRotation.eulerAngles = 0f;
        //
        //
        //playerBody.Rotate(Vector3.* mouseY);

        //playerBody.LookAt(new Vector3(mouseX,))
        //Debug.Log(Vector3.up * mouseX + Vector3.left * mouseY);

    }
}
