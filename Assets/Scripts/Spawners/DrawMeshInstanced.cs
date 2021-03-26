using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class ObjData {
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;  // basically rotations within unity



    public Matrix4x4 matrix {
        get {  // so other classes cant fuck with it
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }

    public ObjData(Vector3 pos, Vector3 scale, Quaternion rot) {  // parameterized constructor
        this.pos = pos;
        this.scale = scale;
        this.rot = rot; 
    }

    // TODO deconstructor 
}

public class DrawMeshInstanced : MonoBehaviour
{
    public int instances;
    
    public Vector3 maxPos;
    public float radius;

    public GameObject prefab;
    public Mesh objMesh;
    public Material objMat;

    private List<List<GameObject>> batches = new List<List<GameObject>>();  // why nested list -> limitation to Graph.DrawMeshInstanced: u cant draw over 1024 obj -> if we want to draw more we have to batch up all the objs we create -> to sections of 1000 and store them in lists of lists == batches

    // buffer stuff
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };



    // Start is called before the first frame update
    void Start()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
        //instances = 8;
        int batchIndexNum = 0;
        List<GameObject> currBatch = new List<GameObject>();  // help track in for loop
        for(int i = 0; i < instances; i++) {
            AddObj(currBatch, i);
            batchIndexNum++;
            if(batchIndexNum >= 1000) {
                batches.Add(currBatch);
                currBatch = BuildNewBatch();
                batchIndexNum = 0;  // reset
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        RenderBatches();
    }

    //                                           useful for when you want to modify pos based on loop number (eg straight lines)
    private void AddObj(List<GameObject> currBatch, int i) {
        Instantiate(prefab, this.transform.position + Random.insideUnitSphere * radius, Random.rotation, this.transform);

    }



    private List<GameObject> BuildNewBatch() {
        return new List<GameObject>();
    }

    private void RenderBatches() {
        foreach (var batch in batches) {
            //                                             stores stuff like in Transform
           //raphics.DrawMeshInstanced(objMesh, 0, objMat, batch.Select((a) => a.matrix).ToList());
            // Render
            Graphics.DrawMeshInstancedIndirect(objMesh, 0, objMat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
        }
    }
    

}
