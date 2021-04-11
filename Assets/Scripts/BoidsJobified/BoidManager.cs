using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;


public class BoidManager : MonoBehaviour {



    // GAMEOBJECT STUFF HERE
    public List<GameObject> goList;
    public GameObject prefab;

    // STUFF TO CHANGE IN EDITOR  
    public float StartRadius;

    public float avoidPlayerRadius;

    public int number;
    public float maxVelocity;
    public float AgentDensity;

    // DATA CONTAINERS
    TransformAccessArray TransformAccessArray;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    NativeArray<Vector3> VelocitiesArray; // we need to be able to write to it in the jobs

    // RAYCAST STUFF
    public ushort raycastDistance = 2;


    // JOBS 
    public BoidAlignmentJob AlignmentJob;
    public BoidCohesionJob CohesionJob;
    public BoidSeparateJob SeparateJob;

    public BoidAvoidPlayerJob AvoidPlayerJob;
    public BoidAvoidObjJob AvoidObjJob;


    // JOBHANDLES
    JobHandle AlignmentJobHandle;  
    JobHandle CohesionJobHandle;
    JobHandle SeparateJobHandle; 

    JobHandle AvoidPlayerJobHandle;
    JobHandle AvoidObjJobHandle;



    void Start() {

        // INIT 
        for (int i = 0; i < number; ++i) {

            goList.Add(Instantiate(prefab, Random.insideUnitSphere * StartRadius * AgentDensity, Random.rotation));

        }
    }

   
    private void Update() {  

        // DATA CONTAINERS

        // init arrays
        Transform[] TransformTemp = new Transform[number];
        BoidsPositionArray = new NativeArray<Vector3>(number, Allocator.TempJob);
        VelocitiesArray = new NativeArray<Vector3>(number, Allocator.TempJob);

        // raycast data containers
      //  NativeArray < SpherecastCommand > raycastCommandsArray = new NativeArray<SpherecastCommand>(number, Allocator.TempJob);
        NativeArray<RaycastCommand> raycastCommandsArray = new NativeArray<RaycastCommand>(number, Allocator.TempJob);
        NativeArray<RaycastHit> raycastHitsArray = new NativeArray<RaycastHit>(number, Allocator.TempJob);


        // INIT ---------------------------------------------------------------------------------------------------------------------------------------------------
        for (int i = 0; i < number; ++i) {
           
            var obj = goList[i];  // ref to current gameobject 
            
            TransformTemp[i] = obj.transform;  // for TransformAccessArray

            BoidsPositionArray[i] = obj.transform.position;

            VelocitiesArray[i] = obj.transform.forward * maxVelocity; // change start velocity HERE

            transform.up = VelocitiesArray[i];  // upward part of capsule points in direction of movement

            // raycast commands array init -> per boid one command 
           // raycastCommandsArray[i] = new SpherecastCommand(BoidsPositionArray[i], raycastDistance, VelocitiesArray[i]);
            raycastCommandsArray[i] = new RaycastCommand(BoidsPositionArray[i], VelocitiesArray[i], raycastDistance);
        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good



        // create list of jobhandles and use complete all (see vid code monkey 9min) 

        // START JOBS-------------------------------------------------------------------------------------------------------------------------------------------
        AlignmentJob = new BoidAlignmentJob() {
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
        };

        CohesionJob = new BoidCohesionJob() {
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
        };

        SeparateJob = new BoidSeparateJob() {
            BoidsPositionArray = BoidsPositionArray,
            velocity = VelocitiesArray,
        };

        


        // Schedule--------------------------------------------------------
        AlignmentJobHandle = AlignmentJob.Schedule(TransformAccessArray);
        CohesionJobHandle = CohesionJob.Schedule(TransformAccessArray, AlignmentJobHandle);     
        SeparateJobHandle = CohesionJob.Schedule(TransformAccessArray, CohesionJobHandle);




        // update gets called in the end and uses the changed velocities to move obj with transform
        // combine all dependencies: 
        //  NativeArray<JobHandle> handles = new NativeArray<JobHandle>(1, Allocator.TempJob);

        // Populate `handles` with `JobHandles` from multiple scheduled jobs...


        // Complete--------------------------------------------------------
        AlignmentJobHandle.Complete();
        CohesionJobHandle.Complete();
        SeparateJobHandle.Complete();



        // END JOBS---------------------------------------------------------------------------------------------------------------------------------------------------------------


        // RAYCAST START---------------------------------------------------------------------------------------------------------------------------------------
        NativeArray<bool> isHitObj = new NativeArray<bool>(number, Allocator.TempJob);
        NativeArray<bool> isHitPlayer = new NativeArray<bool>(number, Allocator.TempJob);
        NativeArray<Vector3> hitNormals = new NativeArray<Vector3>(number, Allocator.TempJob);
        NativeArray<Vector3> hitNormalsPlayer = new NativeArray<Vector3>(number, Allocator.TempJob);

        // make job
        BoidRaycastCommandJobs RaycastCommandJobs;

        RaycastCommandJobs = new BoidRaycastCommandJobs() {
            raycastDistance = raycastDistance,
            positions = BoidsPositionArray, // cant use IJobParallelForTransform so we have to pass pos manually
            velocities = VelocitiesArray,
            Raycasts = raycastCommandsArray,
        }; 

        // Schedule the batch of raycasts
        JobHandle handle;
        var setupDependency = RaycastCommandJobs.Schedule(number, 32);
        handle = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependency);

        handle.Complete();  // "Since the results are written asynchronously the results buffer cannot be accessed until the job has been completed."

        // IS BOID ABOUT TO HIT OBJ?--------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            RaycastHit hit;
            hit = raycastHitsArray[i];

            // about to hit Obj?
            if (hit.collider) {
                isHitObj[i] = true; // setting up isHitObj Array
                hitNormals[i] = hit.normal; 
            }
            else {
                isHitObj[i] = false;
            }

        }


        //----------------------------------------------
        BoidRaycastCommandJobs RaycastCommandJobsPlayer;
        NativeArray<Vector3> newVelocitiesArray = new NativeArray<Vector3> (number, Allocator.TempJob);
        for (int i = 0; i < number; i++) {
            newVelocitiesArray[i]= - VelocitiesArray[i] ;

        }

        RaycastCommandJobsPlayer = new BoidRaycastCommandJobs() {
            raycastDistance = raycastDistance,
            positions = BoidsPositionArray, // cant use IJobParallelForTransform so we have to pass pos manually
            velocities = newVelocitiesArray,
            Raycasts = raycastCommandsArray,
        };

        // Schedule the batch of raycasts
        JobHandle handlePlayer;
        var setupDependencyPlayer = RaycastCommandJobsPlayer.Schedule(number, 32, handle);
        handlePlayer = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependencyPlayer);

        handlePlayer.Complete();  // "Since the results are written asynchronously the results buffer cannot be accessed until the job has been completed."
       


        // IS BOID ABOUT TO HIT OBJ?--------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            RaycastHit hit;
            hit = raycastHitsArray[i];

            // about to hit Player?
            if (hit.collider.gameObject.tag == "Shark") {
                isHitPlayer[i] = true;
                hitNormalsPlayer[i] = hit.normal;
            }
            else {
                isHitPlayer[i]  = false;
            }

        }

        // trash can
        raycastCommandsArray.Dispose();
        raycastHitsArray.Dispose();
        newVelocitiesArray.Dispose();

        // RAYCAST END--------------------------------------------------------------------------------------------------------------------------------------------------------
        AvoidPlayerJob = new BoidAvoidPlayerJob() {

            radius = avoidPlayerRadius,
            isHitObstacles = isHitPlayer,
            hitNormals = hitNormalsPlayer,
            velocity = VelocitiesArray,

        };
        AvoidPlayerJobHandle = AvoidPlayerJob.Schedule(TransformAccessArray,handle);
        AvoidPlayerJobHandle.Complete();


        AvoidObjJob = new BoidAvoidObjJob() {
            isHitObstacles = isHitObj,
            hitNormals = hitNormals,
            velocity = VelocitiesArray,
        };
        AvoidObjJobHandle = AvoidObjJob.Schedule(TransformAccessArray, handle);
        AvoidObjJobHandle.Complete();


        //dispose raycast dc
        hitNormals.Dispose();
        isHitObj.Dispose();
        isHitPlayer.Dispose();


        // UPDATE BOID-------------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            if (VelocitiesArray[i].magnitude > maxVelocity) {
                VelocitiesArray[i] = VelocitiesArray[i].normalized * maxVelocity;
            }
            TransformAccessArray[i].up = VelocitiesArray[i]; // cannot turn this into a job bc transform.up
            TransformAccessArray[i].position += VelocitiesArray[i] * Time.deltaTime;
        }

        // trash can
        BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
        TransformAccessArray.Dispose();

    }




}