using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;


public class BoidManager : MonoBehaviour {


    public List<GameObject> boids;


    public GameObject prefab;

    public float radius;
    public int number;
    public float maxVelocity;

    //
    //NativeArray<Transform> Velocities;
    TransformAccessArray TransformAccessArray;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;

    public List<Collider> contextColliders;
   // [ReadOnly] public NativeArray<Collision> BoidsCollisionArray;

    NativeArray<Vector3> VelocitiesArray; // saving vel here not in Boid.cs

    // DO NOT TOUCH
    // private Transform[] TransformTemp;// = new Transform[100];

    public BoidUpdateJob UpdateJob;
    public BoidAlignmentJob AlignmentJob;
    public BoidCohesionJob CohesionJob;

    JobHandle UpdateJobHandle;
    JobHandle AlignmentJobHandle;
    JobHandle CohesionJobHandle;


    void Start() {
        Transform[] TransformTemp = new Transform[number];
       // NativeArray<Vector3> BoidsPositionArray = new NativeArray<Vector3>(number, Allocator.Persistent);
       // NativeArray<Vector3> VelocitiesArray = new NativeArray<Vector3>(number, Allocator.Persistent);
        BoidsPositionArray = new NativeArray<Vector3>(number, Allocator.Persistent);
        VelocitiesArray = new NativeArray<Vector3>(number, Allocator.Persistent);

        for (int i = 0; i < number; ++i) {

            boids.Add(Instantiate(prefab, Random.insideUnitSphere * radius, Random.rotation));
         
            var obj = boids[i];
            TransformTemp[i] = obj.transform;
            BoidsPositionArray[i] = obj.transform.position;
            VelocitiesArray[i] = obj.transform.forward * maxVelocity; // change start velocity HERE
        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
        //BoidsTransformArray = new TransformAccessArray(TransformTemp); // same array twice? 
    }

   
    private void Update() {

        // create list of jobhandles and use complete all (see vid code monkey 9min) 

        // new job-----------------------------------------------------------
        AlignmentJob = new BoidAlignmentJob() {
            deltaTime = Time.deltaTime,
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
        };

        CohesionJob = new BoidCohesionJob() {
            deltaTime = Time.deltaTime,
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
        };

        UpdateJob = new BoidUpdateJob() {  // LAST
            deltaTime = Time.deltaTime,
            velocity = VelocitiesArray,
        };

        // Schedule--------------------------------------------------------
        AlignmentJobHandle = AlignmentJob.Schedule(TransformAccessArray);
        CohesionJobHandle = CohesionJob.Schedule(TransformAccessArray, AlignmentJobHandle);     // ????



        // update gets called in the end and uses the changed velocities to move obj with transform
        // combine all dependencies: 
        //  NativeArray<JobHandle> handles = new NativeArray<JobHandle>(1, Allocator.TempJob);

        // Populate `handles` with `JobHandles` from multiple scheduled jobs...

        //   JobHandle jh = JobHandle.CombineDependencies(handles);

        UpdateJobHandle = UpdateJob.Schedule(TransformAccessArray, CohesionJobHandle);

        // Complete--------------------------------------------------------
        AlignmentJobHandle.Complete();
        CohesionJobHandle.Complete();

        UpdateJobHandle.Complete();
    }

     private void OnDestroy() {
    //private void OnDisable() {
        
        BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
        TransformAccessArray.Dispose();
    }
}