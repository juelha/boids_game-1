using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;


public class BoidManager : MonoBehaviour {


    private List<GameObject> boids;
    private NativeArray<Boid.Data> _boidsDataArray;
    private BoidUpdateJob _job;
    //private BoidMoveJob move_job;

    public GameObject prefab;
    public float radius;
    public int number;

    

    // Spawner 
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < number; ++i) {
            
            // anything inside of sphere is going to be a valid spawn location 
            //                                           sphere at 0/0/0 with r=1           
            boids.Add(Instantiate(prefab, this.transform.position + Random.insideUnitSphere * radius, Random.rotation, this.transform));
       
        }
    }

    private void Awake() {

        // MAKE API HERE
        // include a boid data array based on its list of buildings from the scene
        //                                           length = size of list, Allocator (let jobs know how long we need this list to live in memory) 
        _boidsDataArray = new NativeArray<Boid.Data>(boids.Count, Allocator.Persistent);  // .TempJob <- only using this list for 1 frame within a single _job

        // populate the array
        for (var i = 0; i < boids.Count; i++) {

           // _boidsDataArray[i] = boids[i];
        }

        // pass it into BoidUpdateJob
        _job = new BoidUpdateJob {

            BoidDataArray = _boidsDataArray // PASS API HERE
        };
    }

    private void Update() {
        // length (number of items the _job will iterate over)
        // batchcount (size of each threaded batch)
        //        length = size of list, batchcount = 1    
        var jobHandle = _job.Schedule(boids.Count, 1);
              // this will get the _job running

        jobHandle.Complete();  // ensure that _job finished its work before next frame 
    }

    private void OnDestroy() {
        _boidsDataArray.Dispose(); // need to delete array manually
    }

}
