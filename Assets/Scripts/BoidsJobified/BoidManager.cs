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

    //
    //NativeArray<Transform> Velocities;
    TransformAccessArray TransformAccessArray;
    TransformAccessArray BoidsTransformArray;

    // DO NOT TOUCH
    // private Transform[] TransformTemp;// = new Transform[100];

    public BoidUpdateJob UpdateJob;
    public BoidRulesJob RulesJob;
    JobHandle UpdateJobHandle;
    JobHandle RulesJobHandle;


    void Start() {
        Transform[] TransformTemp = new Transform[number];

        for (int i = 0; i < number; ++i) {

            boids.Add(Instantiate(prefab, Random.insideUnitSphere * radius, Random.rotation));
         
            var obj = boids[i];
            TransformTemp[i] = obj.transform;
        }

        TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
        BoidsTransformArray = new TransformAccessArray(TransformTemp); // same array twice? 
    }

   
    private void Update() {

        // create list of jobhandles and use complete all (see vid code monkey 9min) 

        // new job-----------------------------------------------------------
        UpdateJob = new BoidUpdateJob() {
            deltaTime = Time.deltaTime
        };

        RulesJob = new BoidRulesJob() {
            deltaTime = Time.deltaTime,
            BoidsTransformArray = BoidsTransformArray,
        };

        // Schedule--------------------------------------------------------
        UpdateJobHandle = UpdateJob.Schedule(TransformAccessArray);
        RulesJobHandle = RulesJob.Schedule(TransformAccessArray);

        // Complete--------------------------------------------------------
        UpdateJobHandle.Complete();
        RulesJobHandle.Complete();
    }


    private void OnDisable() {
            TransformAccessArray.Dispose();
        BoidsTransformArray.Dispose(); 
    }
}