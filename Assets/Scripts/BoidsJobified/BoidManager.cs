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
   // public NativeArray<RaycastCommand> raycastCommandsArray;
   // public NativeArray<RaycastHit> raycastHitsArray;

    //  public NativeArray<bool> isHitObstacles; 


    // JOBS 
  //  public RaycastCommandJobs _RaycastCommandJobs;
   // public BoidRaycastCommandJobs RaycastCommandJobs;
    public BoidAlignmentJob AlignmentJob;
    public BoidCohesionJob CohesionJob;
    public BoidSeparateJob SeparateJob;
  //  public BoidStayinRadiusJob StayinRadiusJob;
    public BoidAvoidPlayerJob AvoidPlayerJob;
    public BoidAvoidObjJob AvoidObjJob;

   //public BoidUpdateJob UpdateJob;

    // JOBHANDLES
    JobHandle AlignmentJobHandle;  
    JobHandle CohesionJobHandle;
    JobHandle SeparateJobHandle; 

    //JobHandle StayinRadiusJobHandle;
    JobHandle AvoidPlayerJobHandle;
    JobHandle AvoidObjJobHandle;

   // JobHandle UpdateJobHandle;


    void Start() {

        // INIT 
        for (int i = 0; i < number; ++i) {

            goList.Add(Instantiate(prefab, Random.insideUnitSphere * StartRadius * AgentDensity, Random.rotation));

        }
    }

   
    private void Update() {  // TODO FixedUpdate()


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

        // new job---------------------------------------------------------------------------------------------------------------------
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



        /*
        UpdateJob = new BoidUpdateJob() {  // LAST
            deltaTime = Time.deltaTime,
            velocity = VelocitiesArray,
        };
        */

        // Schedule--------------------------------------------------------
        AlignmentJobHandle = AlignmentJob.Schedule(TransformAccessArray);//, handle);
        CohesionJobHandle = CohesionJob.Schedule(TransformAccessArray, AlignmentJobHandle);     // ????
        SeparateJobHandle = CohesionJob.Schedule(TransformAccessArray, CohesionJobHandle);
        //StayinRadiusJobHandle = CohesionJob.Schedule(TransformAccessArray, CohesionJobHandle);
        AvoidPlayerJobHandle = AvoidPlayerJob.Schedule(TransformAccessArray, SeparateJobHandle);



        // update gets called in the end and uses the changed velocities to move obj with transform
        // combine all dependencies: 
        //  NativeArray<JobHandle> handles = new NativeArray<JobHandle>(1, Allocator.TempJob);

        // Populate `handles` with `JobHandles` from multiple scheduled jobs...

        //   JobHandle jh = JobHandle.CombineDependencies(handles);

        // UpdateJobHandle = UpdateJob.Schedule(TransformAccessArray, AvoidObjJobHandle);

        // Complete--------------------------------------------------------
        AlignmentJobHandle.Complete();
        CohesionJobHandle.Complete();
        SeparateJobHandle.Complete();
        // StayinRadiusJobHandle.Complete();
        AvoidPlayerJobHandle.Complete();

        //  UpdateJobHandle.Complete();

        // END JOBS---------------------------------------------------------------------------------------------------------------------------------------------------------------


        // RAYCAST START--------------------------------------------------------------------------------------------
        NativeArray<bool> isHitObstacles = new NativeArray<bool>(number, Allocator.TempJob);
        NativeArray<Vector3> hitNormals = new NativeArray<Vector3>(number, Allocator.TempJob);


        // alt:
        // var raycastHitsArray = new NativeArray<RaycastHit>(number, Allocator.Temp);
        // var raycastCommandsArray = new NativeArray<RaycastCommand>(number, Allocator.Temp);

        // make job
        BoidRaycastCommandJobs RaycastCommandJobs;


        RaycastCommandJobs = new BoidRaycastCommandJobs() {
            raycastDistance = raycastDistance,
            positions = BoidsPositionArray, // cant use IJobParallelForTransform so we have to pass pos manually
            velocities = VelocitiesArray,
            // positions = BoidsPositionArray,
            Raycasts = raycastCommandsArray,
            //   isHitObstacles = isHitObstacles,
            //  Hits = raycastHitsArray,
            //layerMask = avoidanceLayer
        }; // 104
     

        // Schedule the batch of raycasts
        // Schedule the batch of raycasts
 JobHandle handle;
        //   handle = RaycastCommandJobs.Schedule();// raycastCommandsArray, raycastHitsArray, 32);
        var setupDependency = RaycastCommandJobs.Schedule(number, 32);

        //  handle = SpherecastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependency);
        handle = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependency);

        //  handle = RaycastCommandJobs.ScheduleBatch(number, 32);
        // handle = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, number, default(JobHandle));
        // var setupDependency = raycastJobs.Schedule(number, 32);
        //  JobHandle raycastHandle;// = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32);//, setupDependency);
        // Wait for the batch processing job to complete


        // JobHandle handle = _RaycastCommandJobs.Schedule(TransformAccessArray); 
        //  raycastHandle = _RaycastCommandJobs.Schedule(TransformAccessArray);
        //   raycastHandle = // need raycastHitsArray

        handle.Complete();  // "Since the results are written asynchronously the results buffer cannot be accessed until the job has been completed."




















        // IS BOID ABOUT TO HIT OBJ?--------------------------------------------------------------------------------------------------------------------------
        

        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            RaycastHit hit;
            hit = raycastHitsArray[i];
            // True when the sphere sweep intersects any collider, otherwise false. 
            if (hit.collider) {
                hitNormals[i] = hit.normal; 
                isHitObstacles[i] = true; // setting up isHitObstacles Array
                Debug.Log("hitNormals[i]");
                Debug.Log(hitNormals[i]);
            }
            else {
                isHitObstacles[i] = false;
                //   Debug.Log(isHitObstacles[i]);
            }
        }

        

         raycastCommandsArray.Dispose();
         raycastHitsArray.Dispose();


        // RAYCAST END-----------------------------------------------------------------------------------------------------------------------

        AvoidObjJob = new BoidAvoidObjJob() {
            deltaTime = Time.deltaTime,
            isHitObstacles = isHitObstacles,
            hitNormals = hitNormals,
            velocity = VelocitiesArray,
        };
        AvoidObjJobHandle = AvoidObjJob.Schedule(TransformAccessArray);
        AvoidObjJobHandle.Complete();


        //dispose raycast bs
        hitNormals.Dispose();
        isHitObstacles.Dispose();


        // UPDATE BOID-------------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            if (VelocitiesArray[i].magnitude > maxVelocity) {
                VelocitiesArray[i] = VelocitiesArray[i].normalized * maxVelocity;
            }

            // Debug.Log("VelocitiesArray[i]");
            // Debug.Log(VelocitiesArray[i]);
            TransformAccessArray[i].up = VelocitiesArray[i]; // cannot turn this into a job bc transform.up
            TransformAccessArray[i].position += VelocitiesArray[i] * Time.deltaTime;
        }

        
        BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
        TransformAccessArray.Dispose();


      



    }

    /*
    private void OnDestroy() {
   // private void OnDisable() {
        
        BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
        TransformAccessArray.Dispose();


        raycastCommandsArray.Dispose();
        raycastHitsArray.Dispose();

      //  isHitObstacles.Dispose();
      //  hitNormals.Dispose();
     }


  
    */








}