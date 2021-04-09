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
    public NativeArray<RaycastHit> raycastHitsArray;

    // for managing raycast results -> TODO: 
    //  public NativeArray<Vector3> hitNormals;
    //  public NativeArray<bool> isHitObstacles; 

    // JOBS 
  //  public RaycastCommandJobs _RaycastCommandJobs;
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

        // raycast data containers
        raycastCommandsArray = new NativeArray<RaycastCommand>(number, Allocator.Persistent); // Allocator.TempJob);
        raycastHitsArray = new NativeArray<RaycastHit>(number, Allocator.Persistent); // Allocator.TempJob);

        for (int i = 0; i < number; ++i) {

            goList.Add(Instantiate(prefab, Random.insideUnitSphere * StartRadius, Random.rotation));
         
            // ref to current gameobject 
            var obj = goList[i];

            // for TransformAccessArray
            TransformTemp[i] = obj.transform;

            BoidsPositionArray[i] = obj.transform.position;
          //  VelocitiesArray[i] = boid.velocity; 
            VelocitiesArray[i] = obj.transform.forward * maxVelocity; // change start velocity HERE

            // raycast commands array init -> per boid one command 
            raycastCommandsArray[i] = new RaycastCommand(BoidsPositionArray[i], VelocitiesArray[i], raycastDistance);
        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
    }

   
    private void Update() {

        // RAYCAST START--------------------------------------------------------------------------------------------
        NativeArray<bool> isHitObstacles = new NativeArray<bool>(number, Allocator.TempJob);
        NativeArray<Vector3> hitNormals = new NativeArray<Vector3>(number, Allocator.TempJob);


        // alt:
        // var raycastHitsArray = new NativeArray<RaycastHit>(number, Allocator.Temp);
        // var raycastCommandsArray = new NativeArray<RaycastCommand>(number, Allocator.Temp);

        // make job
         RaycastCommandJobs _RaycastCommandJobs;


        _RaycastCommandJobs = new RaycastCommandJobs() {
            raycastDistance = raycastDistance,
            velocities = VelocitiesArray,
           // positions = BoidsPositionArray,
            Raycasts = raycastCommandsArray,
            //layerMask = avoidanceLayer
        }; // 104

        // Schedule the batch of raycasts
        // var setupDependency = raycastJobs.Schedule(number, 32);
        JobHandle raycastHandle;// = RaycastCommand.ScheduleBatch(raycastCommandsArray, raycastHitsArray, 32);//, setupDependency);
                                                                                                           // Wait for the batch processing job to complete

        raycastHandle = _RaycastCommandJobs.Schedule(TransformAccessArray);

        raycastHandle.Complete();  // "Since the results are written asynchronously the results buffer cannot be accessed until the job has been completed."

        // CHECKPOINT: WE HAVE raycastCommandsArray ----------------------------------------------------------------

        // Copy the result (= RaycastHit Array). If batchedHit.collider is null there was no hit
        // RaycastHit batchedHit = raycastHitsArray[0];

        // Manage results from raycasting

        //withinBounds = new NativeArray<bool>(number, Allocator.TempJob);
        // isHitObstacles = new NativeArray<bool>(number, Allocator.Persistent); // Allocator.TempJob);
        //  hitNormals = new NativeArray<Vector3>(number, Allocator.Persistent); // Allocator.TempJob);


        //  using collider to figure out if sth is hit


        if (raycastHitsArray != null) {
           // Debug.Log(raycastHitsArray);
           // Debug.DrawRay(BoidsPositionArray[i], VelocitiesArray[i] * raycastDistance, Color.yellow);
        }

        for (int i = 0; i < number; i++) {
            // the collider that was hit 
            var hit = raycastHitsArray[i];
           // Debug.DrawRay(BoidsPositionArray[i], VelocitiesArray[i] * raycastDistance, Color.yellow);
            if (hit.collider) {  
                isHitObstacles[i] = true;
                
            } else {
                isHitObstacles[i] = false;
            }

            // the if else thing works, isHitObstacles throws bugs

           // isHitObstacles[i] = hit.collider ? true : false;
           // hitNormals[i] = raycastHitsArray[i].normal;
            // turnings[i] = (withinBounds[i] == false || isHitObstacles[i]) ? true : false;

            
            /*
            if (debug) {
                if (isHitObstacles[i]) {
                    Debug.DrawRay(BoidsPositionArray[i], forwards[i] * raycastDistance, Color.yellow);
                }*/

        }
         

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
            isHitObstacles = isHitObstacles,
            hitNormals = hitNormals,
            velocity = VelocitiesArray,
            radius = PlayerAvoidRadius,
        };


        UpdateJob = new BoidUpdateJob() {  // LAST
            deltaTime = Time.deltaTime,
            velocity = VelocitiesArray,
        };

        // Schedule--------------------------------------------------------
        AlignmentJobHandle = AlignmentJob.Schedule(TransformAccessArray, raycastHandle);
        CohesionJobHandle = CohesionJob.Schedule(TransformAccessArray, AlignmentJobHandle);     // ????
        StayinRadiusJobHandle = CohesionJob.Schedule(TransformAccessArray, CohesionJobHandle);
        PlayerAvoidJobHandle = PlayerAvoidJob.Schedule(TransformAccessArray, StayinRadiusJobHandle);
        AvoidObjJobHandle = AvoidObjJob.Schedule(TransformAccessArray, PlayerAvoidJobHandle);


        // update gets called in the end and uses the changed velocities to move obj with transform
        // combine all dependencies: 
        //  NativeArray<JobHandle> handles = new NativeArray<JobHandle>(1, Allocator.TempJob);

        // Populate `handles` with `JobHandles` from multiple scheduled jobs...

        //   JobHandle jh = JobHandle.CombineDependencies(handles);

        UpdateJobHandle = UpdateJob.Schedule(TransformAccessArray, AvoidObjJobHandle);

        // Complete--------------------------------------------------------
        AlignmentJobHandle.Complete();
        CohesionJobHandle.Complete();
        StayinRadiusJobHandle.Complete();
        PlayerAvoidJobHandle.Complete();
        AvoidObjJobHandle.Complete();

        UpdateJobHandle.Complete();


        //dipose raycast bs
        hitNormals.Dispose();
        isHitObstacles.Dispose();
    }

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


  









}