using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    private Transform _goal;
    public Transform Route;
    public float collisionRadius;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        _goal = Route.GetChild(0);
        transform.position = _goal.position;

        nextGoal();
    }

    // Update is called once per frame
    void Update()
    {
        move();

        if (Vector3.Distance(transform.position, _goal.position) < collisionRadius)
        {
            //Debug.Log("reached" + _goal.name);
            nextGoal();
        }

    }

    private void nextGoal()
    {
        //get next Node
        int index = _goal.GetSiblingIndex();
        int nextIndex = index + 1;

        //If last Node
        if (Route.childCount <= nextIndex)
        {
            nextIndex = 0;
        }

        _goal = Route.GetChild(nextIndex);

    }

    private void move()
    {

        // Determine which direction to rotate towards
        Vector3 targetDirection = _goal.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        //move
        transform.position += targetDirection.normalized * singleStep;


        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
