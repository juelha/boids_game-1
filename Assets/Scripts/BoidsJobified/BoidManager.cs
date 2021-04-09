using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;


public class BoidManager : MonoBehaviour {


    public List<GameObject> goList;


    public GameObject prefab;
    Player player;
    Vector3 playerPosition;

    public float StartRadius;

    public float AlignmentRadius;
    public float CohesionRadius;
    public float StayinRadius;
    public float PlayerAvoidRadius;

    public int number;
    public float maxVelocity;

    TransformAccessArray TransformAccessArray;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    NativeArray<Vector3> VelocitiesArray; // need to be able to write to it in the jobs

    // JOBS 
    public BoidAlignmentJob AlignmentJob;
    public BoidCohesionJob CohesionJob;
    public BoidStayinRadiusJob StayinRadiusJob;
    public BoidPlayerAvoidJob PlayerAvoidJob;

    public BoidUpdateJob UpdateJob;

    // Handles 
    JobHandle AlignmentJobHandle;
    JobHandle CohesionJobHandle;
    JobHandle StayinRadiusJobHandle;
    JobHandle PlayerAvoidJobHandle;

    JobHandle UpdateJobHandle;


    void Start() {

        // init radius 
        StartRadius = 10;
        AlignmentRadius = 50;
        CohesionRadius = 100;
        StayinRadius = 100;
        PlayerAvoidRadius = 7;

        // init player
        player = GetComponent<Player>();


        // init arrays
        Transform[] TransformTemp = new Transform[number];
        BoidsPositionArray = new NativeArray<Vector3>(number, Allocator.Persistent);
        VelocitiesArray = new NativeArray<Vector3>(number, Allocator.Persistent);

        for (int i = 0; i < number; ++i) {

            goList.Add(Instantiate(prefab, Random.insideUnitSphere * StartRadius, Random.rotation));
         
            // ref to current gameobject 
            var obj = goList[i];

            // for TransformAccessArray
            TransformTemp[i] = obj.transform;

            BoidsPositionArray[i] = obj.transform.position;
            VelocitiesArray[i] = obj.transform.forward * maxVelocity; // change start velocity HERE

        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
    }

   
    private void Update() {
        
        playerPosition = player.transform.position;

        // create list of jobhandles and use complete all (see vid code monkey 9min) 

        // new job-----------------------------------------------------------
        AlignmentJob = new BoidAlignmentJob() {
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
            radius = AlignmentRadius,
        };

        CohesionJob = new BoidCohesionJob() {
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
            radius = CohesionRadius,
        };

        // /*
        StayinRadiusJob = new BoidStayinRadiusJob() {
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
            radius = StayinRadius,
        };
        //  */

        PlayerAvoidJob = new BoidPlayerAvoidJob() {
            playerPosition = playerPosition,
            velocity = VelocitiesArray,
            radius = PlayerAvoidRadius,
        };

        UpdateJob = new BoidUpdateJob() {  // LAST
            deltaTime = Time.deltaTime,
            velocity = VelocitiesArray,
        };

        // Schedule--------------------------------------------------------
        AlignmentJobHandle = AlignmentJob.Schedule(TransformAccessArray);
        CohesionJobHandle = CohesionJob.Schedule(TransformAccessArray, AlignmentJobHandle);     // ????
        StayinRadiusJobHandle = CohesionJob.Schedule(TransformAccessArray, CohesionJobHandle);
        PlayerAvoidJobHandle = PlayerAvoidJob.Schedule(TransformAccessArray, StayinRadiusJobHandle);

        // update gets called in the end and uses the changed velocities to move obj with transform
        // combine all dependencies: 
        //  NativeArray<JobHandle> handles = new NativeArray<JobHandle>(1, Allocator.TempJob);

        // Populate `handles` with `JobHandles` from multiple scheduled jobs...

        //   JobHandle jh = JobHandle.CombineDependencies(handles);

        UpdateJobHandle = UpdateJob.Schedule(TransformAccessArray, PlayerAvoidJobHandle);

        // Complete--------------------------------------------------------
        AlignmentJobHandle.Complete();
        CohesionJobHandle.Complete();
        StayinRadiusJobHandle.Complete();
        PlayerAvoidJobHandle.Complete();

        UpdateJobHandle.Complete();
    }

     private void OnDestroy() {
    //private void OnDisable() {
        
        BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
        TransformAccessArray.Dispose();
     }


   











}