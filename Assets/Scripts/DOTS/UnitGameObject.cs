using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class UnitGameObject : MonoBehaviour
{
    [SerializeField]
    private ConvertedEntityHolder convertedEntityHolder;

    [SerializeField]
    float moveSpeed;


    private void Start()
    {
        Debug.Log(convertedEntityHolder.GetEntity());
        //TODO: create entity here, instead of making one via a gameobject conversion, this will prevent transform and rotation being values in given entity and save space/processing
        //World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity()
    }

    private void Update()
    {

        Entity entity = convertedEntityHolder.GetEntity();
        EntityManager entityManager = convertedEntityHolder.GetEntityManager();

        if (Input.GetMouseButtonDown(0))
        {
           
            //TODO: update this to be the targets position
            //TODO: make sure these coordinates are adjusted to the world, or texture coordinates
            entityManager.AddComponentData(entity, new PathfindingParams
            { startPosition = new int2(0, 0),
                endPosition = new int2(20, 0)
            });
        }

        PathFollow pathFollow = entityManager.GetComponentData<PathFollow>(entity);
        DynamicBuffer<PathPosition> pathPositionBuffer = entityManager.GetBuffer<PathPosition>(entity);

        float moveAmount = moveSpeed * Time.deltaTime;

        while (moveAmount > 0 && pathFollow.pathIndex >= 0)
        {

            

            int2 pathPosition = pathPositionBuffer[pathFollow.pathIndex].position;

            

            float3 targetPosition = new float3(pathPosition.x, pathPosition.y, 0);

            //Check to see if this is the first position, if so it might be closer to start moing to the second position instead
            if (pathFollow.pathIndex == pathPositionBuffer.Length - 1 && pathPositionBuffer.Length >= 2)
            {
               
                int2 nextPathPosition = pathPositionBuffer[pathFollow.pathIndex - 1].position;

                float3 nextPosition = new float3(nextPathPosition.x, nextPathPosition.y, 0);
                //This is the first position to go to, and there is a chance the next index is closer, if it is, then the agent will look werid walking backwards, so lets prevent that
                if (math.distance(transform.position, targetPosition) > math.distance(transform.position, nextPosition))
                {
                    pathFollow.pathIndex--;
                    entityManager.SetComponentData(entity, pathFollow);
                    continue;
                }
            }


            float3 moveDir = math.normalizesafe(targetPosition - (float3) transform.position);



            float dist = math.distance(transform.position, targetPosition);
            if (moveAmount < dist)
            {
                transform.position += (Vector3) (moveDir * moveAmount);
                break;
            }
            else
            {
                transform.position += (Vector3) (dist * moveDir);
                moveAmount -= dist;
            }



            //if the entity is close enough to target position, then change the path index to get the new position, if there is one
            if (math.distance(transform.position, targetPosition) <= 0.1f)
            {
          
                pathFollow.pathIndex = pathFollow.pathIndex - 1;
                
                //Make sure to update the entity here, as the pathfollow is not a reference!!
                entityManager.SetComponentData(entity, pathFollow);
                
            }

        }

    }


}
