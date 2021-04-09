using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayTracing : MonoBehaviour
{
    public float range = 2f;
    public Camera mainCamera;

    void Update()
    {
        Vector3 camPos = this.mainCamera.transform.position;
        Vector3 camDirectionVector = this.mainCamera.transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(camPos, camDirectionVector, this.range);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.name != "Player")
            {
                Debug.Log(hit.transform.name + ": " + i);
            }
        }
    }
}

/*
// doesn't work because the player is overlaying the middle of the screen
// and therefore hit all the time
Vector3 camPos = this.mainCamera.transform.position;
Vector3 camDirectionVector = this.mainCamera.transform.forward;
RaycastHit hit;
bool isHit = Physics.Raycast(camPos, camDirectionVector, out hit, this.range);
if (isHit == true)
{
    Debug.Log(hit.transform.name);
}
*/
