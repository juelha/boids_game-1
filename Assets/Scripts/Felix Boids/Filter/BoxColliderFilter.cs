using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Box Collider")]
public class BoxColliderFilter : ContextFilter
{
    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            Collider itemcollider = item.GetComponent<Collider>();
            if (itemcollider != null && itemcollider.GetType() == typeof(BoxCollider))
            {
                filtered.Add(item);
            }
        }
        return filtered;
    }
}
