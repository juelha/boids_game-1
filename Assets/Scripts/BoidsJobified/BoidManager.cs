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
    public int initialSpawncount = 500;
    public int spawnCount = 0;

    public float maxVelocity;
    public float AgentDensity;

    // DATA CONTAINERS
   // TransformAccessArray TransformAccessArray;
   // [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;
    NativeArray<Vector3> VelocitiesArray; // we need to be able to write to it in the jobs

    // RAYCAST STUFF
    public float raycastDistance = 0.5f;
    NativeArray<RaycastCommand> raycastCommandsArray;
    NativeArray<RaycastCommand> raycastCommandsArrayPlayer;
    NativeArray<RaycastHit> raycastHitsArray;
    NativeArray<RaycastHit> raycastHitsArrayPlayer;

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
        VelocitiesArray = new NativeArray<Vector3>(number, Allocator.TempJob);


        // INIT 
        for (int i = 0; i < number; ++i) {

            goList.Add(Instantiate(prefab, Random.insideUnitSphere * StartRadius * AgentDensity, Random.rotation));


            // init velocity
            var obj = goList[i];  // ref to current gameobject 

            VelocitiesArray[i] = obj.transform.forward * maxVelocity;
            //  obj.transform.up = VelocitiesArray[i];
            Debug.Log("VEL");
            Debug.Log(VelocitiesArray[i]);
        }

       
       

   

        
    }


    private void Update() {

        // DATA CONTAINERS

        // init arrays
        Transform[] TransformTemp = new Transform[number];


        // raycast data containers
        //  NativeArray < SpherecastCommand > raycastCommandsArray = new NativeArray<SpherecastCommand>(number, Allocator.TempJob);
        raycastCommandsArray = new NativeArray<RaycastCommand>(number, Allocator.TempJob);
        raycastCommandsArrayPlayer = new NativeArray<RaycastCommand>(number, Allocator.TempJob);
        raycastHitsArray = new NativeArray<RaycastHit>(number, Allocator.TempJob);

        NativeArray<Vector3> VelocitiesArrayPlayer = new NativeArray<Vector3>(number, Allocator.TempJob);
        raycastHitsArrayPlayer = new NativeArray<RaycastHit>(number, Allocator.TempJob);


        NativeArray<Vector3> BoidsPositionArray;
        TransformAccessArray TransformAccessArray;
        BoidsPositionArray = new NativeArray<Vector3>(number, Allocator.TempJob);


        // UPDATE BOID-------------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            

            // limit velocity
            if (VelocitiesArray[i].magnitude > maxVelocity) {
                VelocitiesArray[i] = VelocitiesArray[i].normalized * maxVelocity;
            }

            // use velocity to changes position
            var obj = goList[i];  // ref to current gameobject 
            obj.transform.up = VelocitiesArray[i];  // upward part of capsule points in direction of movement
            obj.transform.position += VelocitiesArray[i] * Time.deltaTime;

            // save new position
            TransformTemp[i] = obj.transform;  // for TransformAccessArray
            BoidsPositionArray[i] = obj.transform.position;

            // update vel since we changed stuff
           // VelocitiesArray[i] = obj.transform.position;// * maxVelocity;
           // VelocitiesArray[i] = obj.transform.forward;// * maxVelocity;

            // raycast commands array init -> per boid one command 
            raycastCommandsArray[i] = new RaycastCommand(BoidsPositionArray[i], VelocitiesArray[i], raycastDistance);


            // PLAYER STUFF

            VelocitiesArrayPlayer[i] = -VelocitiesArray[i];
            // raycast commands array init -> per boid one command 
          //  Debug.Log(BoidsPositionArray[i]);
          //  Debug.Log(VelocitiesArrayPlayer[i]);
          //  Debug.Log(raycastDistance);
            raycastCommandsArrayPlayer[i] = new RaycastCommand(BoidsPositionArray[i], VelocitiesArray[i], raycastDistance);


        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
        number = TransformAccessArray.length; // update number of flock somewhere

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

        
        // newVelocitiesArray.Dispose();

        // IS BOID ABOUT TO HIT OBJ?--------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            RaycastHit hit;
            hit = raycastHitsArray[i];

            // about to hit Obj?
            if (hit.collider) {
               // Debug.Log("HIT");
                isHitObj[i] = true; // setting up isHitObj Array
                hitNormals[i] = hit.normal;
            }
            else {
                isHitObj[i] = false;
            }

        }

        // trash can raycast job

        raycastCommandsArray.Dispose();
        raycastHitsArray.Dispose();

        //----------------------------------------------
        BoidRaycastCommandJobs RaycastCommandJobsPlayer;
        for (int i = 0; i < number; i++) {
        }
        float raycastDistancePlayer = 0.25f;

        RaycastCommandJobsPlayer = new BoidRaycastCommandJobs() {
            raycastDistance = raycastDistancePlayer,
            positions = BoidsPositionArray, // cant use IJobParallelForTransform so we have to pass pos manually
            velocities = VelocitiesArrayPlayer,
            Raycasts = raycastCommandsArrayPlayer,
        };

        // Schedule the batch of raycasts
        JobHandle handlePlayer;
        var setupDependencyPlayer = RaycastCommandJobsPlayer.Schedule(number, 32, handle);
        handlePlayer = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32, setupDependencyPlayer);

        handlePlayer.Complete();  // "Since the results are written asynchronously the results buffer cannot be accessed until the job has been completed."


        

        // IS BOID ABOUT TO HIT PLAYER?--------------------------------------------------------------------
        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            RaycastHit hit;
            hit = raycastHitsArrayPlayer[i];

            // about to hit Player?
            if (hit.collider) {
       // if (hit.collider.gameObject.tag == "shark") {
                isHitPlayer[i] = true;
                hitNormalsPlayer[i] = hit.normal;
            }
            else {
                isHitPlayer[i] = false;
            }

        }

        // trash can  raycast job
        raycastCommandsArrayPlayer.Dispose();
        VelocitiesArrayPlayer.Dispose();
        raycastHitsArrayPlayer.Dispose();
        // RAYCAST END--------------------------------------------------------------------------------------------------------------------------------------------------------

        /*
        AvoidPlayerJob = new BoidAvoidPlayerJob() {

            radius = avoidPlayerRadius,
            isHitObstacles = isHitPlayer,
            hitNormals = hitNormalsPlayer,
            velocity = VelocitiesArray,

        };
        AvoidPlayerJobHandle = AvoidPlayerJob.Schedule(TransformAccessArray, handle);
        AvoidPlayerJobHandle.Complete();

        */

        AvoidObjJob = new BoidAvoidObjJob() {
            isHitObstacles = isHitObj,
            hitNormals = hitNormals,
            velocity = VelocitiesArray,
        };
        AvoidObjJobHandle = AvoidObjJob.Schedule(TransformAccessArray, handle);//, AvoidPlayerJobHandle);
        AvoidObjJobHandle.Complete();


        //dispose raycast dc
        hitNormals.Dispose();
        hitNormalsPlayer.Dispose();
        isHitObj.Dispose();
        isHitPlayer.Dispose();



        // problem we delete and do not save changed vel anywhere!!!!
        // trash can
        // BoidsPositionArray.Dispose();
        //  VelocitiesArray.Dispose();
        //  TransformAccessArray.Dispose();

        /*
        // UPDATE BOID-------------------------------------------------------------------------
        for (int i = 0; i < number; i++) {

            // limit velocity
            if (VelocitiesArray[i].magnitude > maxVelocity) {
                VelocitiesArray[i] = VelocitiesArray[i].normalized * maxVelocity;
            }

            // use velocity to changes position
            var obj = goList[i];  // ref to current gameobject 
          //  obj.transform.up = VelocitiesArray[i];  // upward part of capsule points in direction of movement
            obj.transform.position += VelocitiesArray[i] * Time.deltaTime;
        }

        */
        for (int i = 0; i < number; i++) {
            Debug.Log("VEL END OF UPDATE");
            Debug.Log(VelocitiesArray[i]);
        }

        // trash can
        TransformAccessArray.Dispose();
        BoidsPositionArray.Dispose();

        
    }


    
    private void OnDestroy() {
  //  private void OnDisable() {

      //  BoidsPositionArray.Dispose();
        VelocitiesArray.Dispose();
      //  raycastCommandsArray.Dispose();
      //  raycastHitsArray.Dispose();
        //  TransformAccessArray.Dispose();
    }

    



    /*
    public void Spanwer(int amt) {
        spawnCount += amt;
        //VelocitiesArray = new NativeArray<float>(spawnCount, Allocator.Persistent);
        for (int i = 0; i < amt; i++) {
            Vector3 pos = this.transform.position;// + new Vector3(UnityEngine.Random.Range(-swimLimits.x, swimLimits.x), UnityEngine.Random.Range(-swimLimits.y, swimLimits.y), UnityEngine.Random.Range(-swimLimits.z, swimLimits.z));

            fishesTrs.Add(Instantiate(fishPrefab, pos, Quaternion.identity, flockParent ? flockParent : null).transform);
            float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            fishSpeeds[i] = speed;
        }
        fishTrsAccessArray = new TransformAccessArray(fishesTrs.ToArray());
    }
    */

}