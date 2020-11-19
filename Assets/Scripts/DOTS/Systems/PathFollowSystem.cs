using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[DisableAutoCreation]
public class PathFollowSystem : ComponentSystem
{
    float moveSpeed = 10f;


    protected override void OnUpdate()
    {

        Debug.Log("pathFollow system OnUpdate:");

        Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, ref PathFollow pathFollow) =>
        {
            float moveAmount = moveSpeed * Time.DeltaTime;

            while (moveAmount > 0 && pathFollow.pathIndex >= 0 )
            {

                int2 pathPosition = pathPositionBuffer[pathFollow.pathIndex].position;

                float3 targetPosition = new float3(pathPosition.x, pathPosition.y, 0);

                if (pathFollow.pathIndex == pathPositionBuffer.Length - 1 && pathPositionBuffer.Length >= 2)
                {
                    int2 nextPathPosition = pathPositionBuffer[pathFollow.pathIndex - 1].position;

                    float3 nextPosition = new float3(nextPathPosition.x, nextPathPosition.y, 0);
                    //This is the first position to go to, and there is a chance the next index is closer, if it is, then the agent will look werid walking backwards, so lets prevent that
                    if (math.distance(translation.Value, targetPosition) > math.distance(translation.Value, nextPosition)) {
                        pathFollow.pathIndex--;
                        continue;
                    }
                }

               
                float3 moveDir = math.normalizesafe(targetPosition - translation.Value);


               

                
                float dist = math.distance(translation.Value, targetPosition);
                if(moveAmount < dist)
                {
                    translation.Value += moveDir * moveAmount;
                    break;
                } else
                {
                    translation.Value += dist * moveDir;
                    moveAmount -= dist;
                }
                


                //if the entity is close enough to target position, then change the path index to get the new position, if there is one
                if(math.distance(translation.Value , targetPosition) <= 0.1f)
                {
                    pathFollow.pathIndex--;
                }

            }

            
            
            if(pathFollow.pathIndex < 0)
            {
                EntityManager.AddComponentData(entity, new PathfindingParams
                {
                    startPosition = new int2((int)translation.Value.x, (int)translation.Value.y),

                    endPosition = new int2(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100))
                });
            }
        });
    }
}
