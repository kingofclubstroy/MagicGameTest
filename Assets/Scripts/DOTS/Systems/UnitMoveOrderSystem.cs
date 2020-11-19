using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
public class UnitMoveOrderSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        ////If the left mouse betton is clicked
        //if(Input.GetMouseButtonDown(0))
        //{

        //    Vector3 mousePosition = GetMouseWorldPosition();
        //    Debug.Log("mousePosition = " + mousePosition);

        //    //We loop through all the entities of the PathfindingParams component
        //    Entities.ForEach((Entity entity, ref Translation translation) =>
        //    {
        //        EntityManager.AddComponentData(entity, new PathfindingParams
        //        {
        //            startPosition = new int2((int)translation.Value.x, (int)translation.Value.y),
        //            endPosition = new int2((int)mousePosition.x, (int)mousePosition.y)
        //        });
        //    });
        //}
    }

   Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = GetWorldPositionWithZ(Input.mousePosition, Camera.main);
        pos.z = 0;
        return pos;
    }

    Vector3 GetWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        return worldCamera.ScreenToWorldPoint(screenPosition);
    }
}
