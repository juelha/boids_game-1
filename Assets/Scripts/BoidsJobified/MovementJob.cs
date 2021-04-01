
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace OOPJobSystem {
    [BurstCompileAttribute]
    public struct MovementJob : IJobParallelForTransform {
        public NativeArray<Vector3> positions;
        public float startTime;
        public float time;
        public float frequency;

        public void Execute(int index, TransformAccess transform) {
            Vector3 displacement = positions[index];
            displacement.y = Mathf.Sin(frequency * time - startTime);
            transform.position = displacement;
        }
    }

    public class MovementJobManager : MonoBehaviour {  
        public TransformAccessArray transforms;
        public NativeArray<Vector3> displacements;
        MovementJob movementJob;
        JobHandle jobHandle;
        float m_startTime;
        public float frequency = 2;

        void Start() {
            m_startTime = Time.time;
            transforms = new TransformAccessArray(transform.childCount);
            displacements = new NativeArray<Vector3>(transform.childCount, Allocator.Persistent);
            for (int i = 0; i < transform.childCount; i++) {
                Transform t = transform.GetChild(i);
                transforms.Add(t);
                displacements[i] = t.position;
            }
        }


        void Update() {
            jobHandle.Complete();

            movementJob = new MovementJob() {
                positions = displacements,
                startTime = m_startTime,
                time = Time.time,
                frequency = frequency
            };

            jobHandle = movementJob.Schedule(transforms);
        }

        private void OnDisable() {
            jobHandle.Complete();
            transforms.Dispose();
            displacements.Dispose();
        }
    }
}