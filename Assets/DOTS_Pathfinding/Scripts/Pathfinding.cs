﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;


[DisableAutoCreation]
public class PathfindingCodeMonkey : ComponentSystem
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


    private NativeArray<int> nativeInt;

    NativeArray<OldPathNode> pathNodeArray;

    const int gridWidth = 100;
    const int gridHeight = 100;
    int2 gridSize = new int2(gridWidth, gridHeight);
    //ArrayList neighbourOffsetArray = new ArrayList<int2>(8);
    NativeArray<int2> neighbourOffsetArray;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        neighbourOffsetArray.Dispose();
        pathNodeArray.Dispose();
    }


    protected override void OnCreate()
    {
        base.OnCreate();
        neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Persistent);
        neighbourOffsetArray[0] = new int2(-1, 0); // Left
        neighbourOffsetArray[1] = new int2(+1, 0); // Right
        neighbourOffsetArray[2] = new int2(0, +1); // Up
        neighbourOffsetArray[3] = new int2(0, -1); // Down
        neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
        neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
        neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
        neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

    }



    protected override void OnUpdate()
    {

        if(pathNodeArray.IsCreated == false)
        {
            ObstacleController.instance.initializePathNodeArray(gridWidth, gridHeight);

            //TODO: I don't know if in need to fetch this from the obstacle controller each update because it is a struct and not a reference
            pathNodeArray = ObstacleController.instance.GetPathNodeArray();
           
        } 

        if(nativeInt == null)
        {
            nativeInt = new NativeArray<int>();
        }

       

        List<FindPathJob> findPathJobList = new List<FindPathJob>();
        NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);

       

        Entities.ForEach((Entity entity, ref PathfindingParams pathfindingParams) =>
        {

           // NativeArray<PathNode> tmpPathNodeArray = new NativeArray<PathNode>(pathNodeArray, Allocator.TempJob);

            FindPathJob findPathJob = new FindPathJob
            {
                gridSize = gridSize,
                pathNodeArray = new NativeArray<OldPathNode>(pathNodeArray, Allocator.TempJob),
                startPosition = pathfindingParams.startPosition,
                endPosition = pathfindingParams.endPosition,
                entity = entity,
                neighbourOffsetArray = neighbourOffsetArray
            };
            findPathJobList.Add(findPathJob);
            jobHandleList.Add(findPathJob.Schedule());

            //tmpPathNodeArray.Dispose();

            PostUpdateCommands.RemoveComponent<PathfindingParams>(entity);
        });

        JobHandle.CompleteAll(jobHandleList);

        foreach (FindPathJob findPathJob in findPathJobList)
        {
            new SetBufferPathJob
            {
                entity = findPathJob.entity,
                gridSize = findPathJob.gridSize,
                pathNodeArray = findPathJob.pathNodeArray,
                pathfindingParamsComponentDataFromEntity = GetComponentDataFromEntity<PathfindingParams>(),
                pathFollowComponentDataFromEntity = GetComponentDataFromEntity<PathFollow>(),
                pathPositionBufferFromEntity = GetBufferFromEntity<PathPosition>(),
            }.Run();
        }

        //pathNodeArray.Dispose();
    }

    


    [BurstCompile]
    private struct SetBufferPathJob : IJob
    {

        public int2 gridSize;

        [DeallocateOnJobCompletion]
        public NativeArray<OldPathNode> pathNodeArray;

        public Entity entity;

        public ComponentDataFromEntity<PathfindingParams> pathfindingParamsComponentDataFromEntity;
        public ComponentDataFromEntity<PathFollow> pathFollowComponentDataFromEntity;
        public BufferFromEntity<PathPosition> pathPositionBufferFromEntity;

        public void Execute()
        {
            DynamicBuffer<PathPosition> pathPositionBuffer = pathPositionBufferFromEntity[entity];
            pathPositionBuffer.Clear();

            PathfindingParams pathfindingParams = pathfindingParamsComponentDataFromEntity[entity];
            int endNodeIndex = CalculateIndex(pathfindingParams.endPosition.x, pathfindingParams.endPosition.y, gridSize.x);
            OldPathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                // Didn't find a path!
                //Debug.Log("Didn't find a path!");
                pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = -1 };
            }
            else
            {
                // Found a path
                CalculatePath(pathNodeArray, endNode, pathPositionBuffer);

                pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1 };
            }

        }
    }


    [BurstCompile]
    private struct FindPathJob : IJob
    {

        public int2 gridSize;
        public NativeArray<OldPathNode> pathNodeArray;

        public int2 startPosition;
        public int2 endPosition;

        public Entity entity;

        [ReadOnly] public NativeArray<int2> neighbourOffsetArray;

        //public BufferFromEntity<PathPosition> pathPositionBuffer;

        public void Execute()
        {
            for (int i = 0; i < pathNodeArray.Length; i++)
            {
                OldPathNode pathNode = pathNodeArray[i];
                pathNode.hCost = CalculateDistanceCost(new int2(pathNode.x, pathNode.y), endPosition);
                pathNode.cameFromNodeIndex = -1;

                pathNodeArray[i] = pathNode;
            }

            

            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

            OldPathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                OldPathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    // Reached our destination!
                    break;
                }

                // Remove current node from Open List
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        // Neighbour not valid position
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                    if (closedList.Contains(neighbourNodeIndex))
                    {
                        // Already searched this node
                        continue;
                    }

                    OldPathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable)
                    {
                        // Not walkable
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }

                }
            }

            //pathPositionBuffer.Clear();

            /*
            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1) {
                // Didn't find a path!
                //Debug.Log("Didn't find a path!");
                pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = -1 };
            } else {
                // Found a path
                //CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
                //pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1 };
            }
            */

            //neighbourOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }


    }

    private static void CalculatePath(NativeArray<OldPathNode> pathNodeArray, OldPathNode endNode, DynamicBuffer<PathPosition> pathPositionBuffer)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // Couldn't find a path!
        }
        else
        {
            // Found a path
            pathPositionBuffer.Add(new PathPosition { position = new int2(endNode.x, endNode.y) });

            OldPathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)
            {
                OldPathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                pathPositionBuffer.Add(new PathPosition { position = new int2(cameFromNode.x, cameFromNode.y) });
                currentNode = cameFromNode;
            }
        }
    }

    private static NativeList<int2> CalculatePath(NativeArray<OldPathNode> pathNodeArray, OldPathNode endNode)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // Couldn't find a path!
            return new NativeList<int2>(Allocator.Temp);
        }
        else
        {
            // Found a path
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y));

            OldPathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)
            {
                OldPathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                path.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            return path;
        }
    }

    private static bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < gridSize.x &&
            gridPosition.y < gridSize.y;
    }

    private static int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    private static int CalculateDistanceCost(int2 aPosition, int2 bPosition)
    {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(aPosition.y - bPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }


    private static int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<OldPathNode> pathNodeArray)
    {
        OldPathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            OldPathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }

    
}
