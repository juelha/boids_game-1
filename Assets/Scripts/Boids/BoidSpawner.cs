using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{

    public GameObject prefab;
    public float radius;
    public int number;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < number; ++i) {
            // anything inside of sphere is going to be a valid spawn location 
            //                                           sphere at 0/0/0 with r=1           
            Instantiate(prefab, this.transform.position + Random.insideUnitSphere * radius, Random.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
