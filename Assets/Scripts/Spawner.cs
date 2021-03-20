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

public class Spawner : MonoBehaviour
{
    public int instances;
    public Vector3 maxPos;
    public Mesh objMesh;
    public Material objMat;

    private List<List<ObjData>> batches = new List<List<ObjData>>();  // why nested list -> limitation to Graph.DrawMeshInstanced: u cant draw over 1024 obj -> if we want to draw more we have to batch up all the objs we create -> to sections of 1000 and store them in lists of lists == batches


    // Start is called before the first frame update
    void Start()
    {
        int batchIndexNum = 0;
        List<ObjData> currBatch = new List<ObjData>();  // help track in for loop
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
    private void AddObj(List<ObjData> currBatch, int i) {
        Vector3 position = new Vector3(Random.Range(-maxPos.x, maxPos.x), Random.Range(-maxPos.y, maxPos.y), Random.Range(-maxPos.z, maxPos.z));
        currBatch.Add(new ObjData(position, new Vector3(2, 2, 2), Quaternion.identity));
            

    }

    private List<ObjData> BuildNewBatch() {
        return new List<ObjData>();
    }

    private void RenderBatches() {
        foreach (var batch in batches) {
            //                                             stores stuff like in Transform
            Graphics.DrawMeshInstanced(objMesh, 0, objMat, batch.Select((a) => a.matrix).ToList()); 
        }
    }


}
