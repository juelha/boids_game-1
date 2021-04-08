using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;


public class BoidManager : MonoBehaviour {


    public List<GameObject> goList;


    public GameObject prefab;

    public float radius;
    public float neighborRadius;
    public int number;
    public float maxVelocity;

    //
    //NativeArray<Transform> Velocities;
    TransformAccessArray TransformAccessArray;
    [ReadOnly] public NativeArray<Vector3> BoidsPositionArray;

    //public List<Collider> contextColliders;
    List<Transform> nearbyTransforms;
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

            goList.Add(Instantiate(prefab, Random.insideUnitSphere * radius, Random.rotation));
         
            // ref to current gameobject 
            var obj = goList[i];

            // for TransformAccessArray
            TransformTemp[i] = obj.transform;

            BoidsPositionArray[i] = obj.transform.position;
            VelocitiesArray[i] = obj.transform.forward * maxVelocity; // change start velocity HERE
            List<Transform> nearbyTransforms = GetNearbyObjects(obj);

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


    public List<Transform> GetNearbyObjects(GameObject obj) {
        List<Transform> context = new List<Transform>();
        //instancing a array of colliders ´which entails every collider in a given radius (-> neighborRadius)
        Collider[] contextColliders = Physics.OverlapSphere(obj.transform.position, neighborRadius);
        // sorting out own collider and putting rest in list context
        foreach (Collider c in contextColliders) {
            if (c != obj.GetComponent<Collider>()) {
                context.Add(c.transform);
            }

        }
        return context;
    }












}