using Unity.Jobs;
using Unity.Collections;




// create a job
public struct BoidUpdateJob : IJobParallelFor {  // IJobParallelFor can run same logic over a list of items

    // add array of dataobjs for job to iterate over
    public NativeArray<Boid.Data> BoidDataArray;  // why NativeArray? <-  special type of array that was designed to work optimally with jobs

    public void Execute(int index) {
        // how it works:
        // 1. pull dataObj out of array
        // 2. do our work
        // 3. put it back in

        var curDataObj = BoidDataArray[index];  // 1. ref to current data obj 
        curDataObj.Update();                    // 2.
        BoidDataArray[index] = curDataObj;      // 3. pass the data back in the array using index
    }
}

