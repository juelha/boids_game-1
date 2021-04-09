using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;


public class BoidManager : MonoBehaviour {



    // GAMEOBJECT STUFF HERE
    public List<GameObject> goList;
    public GameObject prefab;
    GameObject playerObj;
   // Boid boid;
    Vector3 playerPosition;

    // RADIUS TO CHANGE IN EDITOR -> can be taken out here and assigned manually in each script 
    public float StartRadius;

    public float AlignmentRadius;
    public float CohesionRadius;
    public float StayinRadius;
    public float PlayerAvoidRadius;

    public int number;
    public float maxVelocity;

    // DATA CONTAINERS
    TransformAccessArray TransformAccessArray;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    NativeArray<Vector3> VelocitiesArray; // need to be able to write to it in the jobs

    // RAYCAST STUFF
    public ushort raycastDistance = 2;
    public NativeArray<RaycastCommand> raycastCommandsArray;

    // JOBS 
    public BoidAlignmentJob AlignmentJob;
    public BoidCohesionJob CohesionJob;
    public BoidStayinRadiusJob StayinRadiusJob;
    public BoidPlayerAvoidJob PlayerAvoidJob;
    public BoidAvoidObjJob AvoidObjJob;

    public BoidUpdateJob UpdateJob;

    // JOBHANDLES 
    JobHandle AlignmentJobHandle;
    JobHandle CohesionJobHandle;
    JobHandle StayinRadiusJobHandle;
    JobHandle PlayerAvoidJobHandle;
    JobHandle AvoidObjJobHandle;

    JobHandle UpdateJobHandle;


    void Start() {

        // init radius 
        StartRadius = 10;
        AlignmentRadius = 50;
        CohesionRadius = 100;
        StayinRadius = 100;
        PlayerAvoidRadius = 7;

        // init player
        if (gameObject.tag == "Player") { 
            playerObj = gameObject;
        }
        //player = GetComponent<Player>();  // use sth like if (col.gameObject.tag == "Boid") 
        playerPosition =Vector3.zero; // OnTriggerEnter();

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
          //  VelocitiesArray[i] = boid.velocity; 
            VelocitiesArray[i] = obj.transform.forward * maxVelocity; // change start velocity HERE

        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
    }

   
    private void Update() {

        // RAYCAST START--------------------------------------------------------------------------------------------
            
        // raycast data containers
        raycastCommandsArray = new NativeArray<RaycastCommand>(number, Allocator.TempJob);
        NativeArray<RaycastHit> raycastHits = new NativeArray<RaycastHit>(number, Allocator.TempJob);

        // raycast dc init
        for (int i = 0; i < number; ++i) {
            raycastCommandsArray[i] = new RaycastCommand(BoidsPositionArray[i], VelocitiesArray[i], raycastDistance);
        }


        //Schedule the batch of raycasts
        RaycastCommandJobs raycastJobs = new RaycastCommandJobs() {
            raycastDistance = raycastDistance,
            velocities = VelocitiesArray,
            positions = BoidsPositionArray,
            Raycasts = raycastCommandsArray,
            //layerMask = avoidanceLayer
        }; // 104
        var setupDependency = raycastJobs.Schedule(number, 32);
        JobHandle raycastHandle = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHits, 32, setupDependency);
        raycastHandle.Complete();

        // CHECKPOINT: WE HAVE raycastCommandsArray ----------------------------------------------------------------


        RaycastHit batchedHit = raycastCommandsArray[0];



        // RAYCAST END-----------------------------------------------------------------------------------------------------------------------



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

        AvoidObjJob = new BoidAvoidObjJob() {
            NearbyObjsPositionArray = NearbyObjsPositionArray,
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
        raycastCommandsArray.Dispose();
     }


    private Vector3 OnTriggerEnter(Collider other) {
        //Check to see if the tag on the collider is equal to Enemy
        if (other.tag == "Player") {
            playerPosition = other.transform.position;
        }
        return playerPosition;
    }











}