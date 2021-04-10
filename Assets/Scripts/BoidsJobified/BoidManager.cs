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

    public float PlayerAvoidRadius;

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

        AvoidPlayerJob = new BoidAvoidPlayerJob() {
          //  playerPosition = playerPosition,
            velocity = VelocitiesArray,
            radius = PlayerAvoidRadius,
        };


        // Schedule--------------------------------------------------------
        AlignmentJobHandle = AlignmentJob.Schedule(TransformAccessArray);
        CohesionJobHandle = CohesionJob.Schedule(TransformAccessArray, AlignmentJobHandle);     
        SeparateJobHandle = CohesionJob.Schedule(TransformAccessArray, CohesionJobHandle);

        AvoidPlayerJobHandle = AvoidPlayerJob.Schedule(TransformAccessArray, SeparateJobHandle);



        // update gets called in the end and uses the changed velocities to move obj with transform
        // combine all dependencies: 
        //  NativeArray<JobHandle> handles = new NativeArray<JobHandle>(1, Allocator.TempJob);

        // Populate `handles` with `JobHandles` from multiple scheduled jobs...


        // Complete--------------------------------------------------------
        AlignmentJobHandle.Complete();
        CohesionJobHandle.Complete();
        SeparateJobHandle.Complete();

        AvoidPlayerJobHandle.Complete();


        // END JOBS---------------------------------------------------------------------------------------------------------------------------------------------------------------


        // RAYCAST START---------------------------------------------------------------------------------------------------------------------------------------
        NativeArray<bool> isHitObstacles = new NativeArray<bool>(number, Allocator.TempJob);
        NativeArray<Vector3> hitNormals = new NativeArray<Vector3>(number, Allocator.TempJob);

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
        //   handle = RaycastCommandJobs.Schedule();// raycastCommandsArray, raycastHitsArray, 32);
        var setupDependency = RaycastCommandJobs.Schedule(number, 32);

        //  handle = SpherecastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependency);
        handle = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependency);

        handle.Complete();  // "Since the results are written asynchronously the results buffer cannot be accessed until the job has been completed."

        // IS BOID ABOUT TO HIT OBJ?--------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            RaycastHit hit;
            hit = raycastHitsArray[i];
            // True when the sphere sweep intersects any collider, otherwise false. 
            if (hit.collider) {
                hitNormals[i] = hit.normal; 
                isHitObstacles[i] = true; // setting up isHitObstacles Array
              //  Debug.Log("hitNormals[i]");
               // Debug.Log(hitNormals[i]);
            }
            else {
                isHitObstacles[i] = false;
                //   Debug.Log(isHitObstacles[i]);
            }
        }

         raycastCommandsArray.Dispose();
         raycastHitsArray.Dispose();


        // RAYCAST END--------------------------------------------------------------------------------------------------------------------------------------------------------

        AvoidObjJob = new BoidAvoidObjJob() {
            deltaTime = Time.deltaTime,
            isHitObstacles = isHitObstacles,
            hitNormals = hitNormals,
            velocity = VelocitiesArray,
        };
        AvoidObjJobHandle = AvoidObjJob.Schedule(TransformAccessArray);
        AvoidObjJobHandle.Complete();


        //dispose raycast dc
        hitNormals.Dispose();
        isHitObstacles.Dispose();


        // UPDATE BOID-------------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            if (VelocitiesArray[i].magnitude > maxVelocity) {
                VelocitiesArray[i] = VelocitiesArray[i].normalized * maxVelocity;
            }
            TransformAccessArray[i].up = VelocitiesArray[i]; // cannot turn this into a job bc transform.up
            TransformAccessArray[i].position += VelocitiesArray[i] * Time.deltaTime;
        }

        
        BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
        TransformAccessArray.Dispose();

    }




}