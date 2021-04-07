using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {
    // Start is called before the first frame update
    public CharacterController controller;

    [Range(1f, 20f)]
    public float speed = 5f;

    public float turnSmoothTime = 0.01f;
    float turnSmoothVelocity;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;

        //if player is moved
        if (direction.magnitude >= 0.1f) {
            //adjust rotation of player so he look in direction of heading
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);

        }
    }
}
