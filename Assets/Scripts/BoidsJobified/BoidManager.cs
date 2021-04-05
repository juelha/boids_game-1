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
    NativeArray<Vector3> Velocities;
    TransformAccessArray TransformAccessArray;

    // DO NOT TOUCH
    private Transform[] TransformTemp;// = new Transform[100];

    public BoidUpdateJob _UpdateJob;
    JobHandle UpdateJobHandle;


    void Start() {
        Transform[] TransformTemp = new Transform[number];

        for (int i = 0; i < number; ++i) {

            boids.Add(Instantiate(prefab, Random.insideUnitSphere * radius, Random.rotation));
         
            var obj = boids[i];
            TransformTemp[i] = obj.transform;
        }

         TransformAccessArray = new TransformAccessArray(TransformTemp);  // so far so good
    }

   
    private void Update() {
        _UpdateJob = new BoidUpdateJob() {
            deltaTime = Time.deltaTime
        };

        UpdateJobHandle = _UpdateJob.Schedule(TransformAccessArray);
        UpdateJobHandle.Complete();
    }


    private void OnDisable() {
            TransformAccessArray.Dispose();
    }
}