using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;


public class BoidManager : MonoBehaviour {


    public List<GameObject> boids;
    public GameObject boid; 
    //private NativeArray<Boid.Data> _boidsDataArray;
    //private BoidUpdateJob _UpdateJob;
    //private BoidMoveJob move_job;

    public GameObject prefab;
    public float radius;
    public int number;

    //
    NativeArray<Vector3> Velocities;
    TransformAccessArray TransformAccessArray;

    // what type???
    GameObject[] GOArray;
    List<Transform> TransformTempList;

    // DO NOT TOUCH
    private Transform[] TransformTemp;// = new Transform[100];

    public BoidUpdateJob _UpdateJob;
    JobHandle UpdateJobHandle;



    // Spawner 
    // Start is called before the first frame update
    void Start() {
        Transform[] TransformTemp = new Transform[number];

        for (int i = 0; i < number; ++i) {

            // anything inside of sphere is going to be a valid spawn location 
            //                                           sphere at 0/0/0 with r=1           
            // boid = new GameObject();
            //  var bla = boid.transform; 
            boids.Add(Instantiate(prefab, Random.insideUnitSphere * radius, Random.rotation));
           // boids.Add(Instantiate(prefab, Vector3.zero, Random.rotation));  // works!!!

            //   boid = Instantiate(prefab, Random.insideUnitSphere * radius, Random.rotation);
            // boids.Add(boid);

            // Transform[] TransformTemp = new Transform[100];
            TransformTemp[i] = boid.transform;
           // TransformAccessArray[i] = boid.transform;

        }

         TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
        //TransformAccessArray.SetTransforms(TransformTemp);
    }

    // private void Awake() {

    // MAKE API HERE
    // include a boid data array based on its list of buildings from the scene
    //                                           length = size of list, Allocator (let jobs know how long we need this list to live in memory) 
    // _boidsDataArray = new NativeArray<Boid.Data>(boids.Count, Allocator.Persistent);  // .TempJob <- only using this list for 1 frame within a single _UpdateJob

    // populate the array
    // for (var i = 0; i < boids.Count; i++) {

    // _boidsDataArray[i] = boids[i];
    // }

    // pass it into BoidUpdateJob
    //  _UpdateJob = new BoidUpdateJob {

    //    BoidDataArray = _boidsDataArray // PASS API HERE
    //   };
    //  }

    
 

    private void Update() {
        // length (number of items the _UpdateJob will iterate over)
        // batchcount (size of each threaded batch)
        //        length = size of list, batchcount = 1    
        //  var jobHandle = _UpdateJob.Schedule(transformsAccess);
        // this will get the _UpdateJob running

        //  var jobHandle = _UpdateJob.Schedule(transformsAccess);

        //  jobHandle.Complete();  // ensure that _UpdateJob finished its work before next frame 

        _UpdateJob = new BoidUpdateJob() {
            deltaTime = Time.deltaTime//,
          //  velocity = Velocities,
        };


        UpdateJobHandle = _UpdateJob.Schedule(TransformAccessArray);
       //++
       //UpdateJobHandle = _UpdateJob.Run(); // for debug
        //Velocities.Dispose();
        UpdateJobHandle.Complete();
     //   TransformAccessArray.Dispose();
    }



   // public void LateUpdate() {
     //   UpdateJobHandle.Complete();
   // }

   // private void OnDestroy() {
    private void OnDisable() {
            //       Velocities.Dispose();
            TransformAccessArray.Dispose();
    }
}