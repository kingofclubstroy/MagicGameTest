/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;


public class Pathfinding : JobComponentSystem
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int itterationLimit = 1000;

    //This is our hardcoded grid parameters, need to make this flexible if we segment into various rooms
    private int2 gridParams { get {
            return ObstacleController.instance.gridParams;
        }

    }

    //This is the number of threads we want working on pathfinding, may need to adjust as for each we will need to allocate memory proportional to the gridsize ^ 2
    private const int numberWorkerThreads = 4;

    EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    //NativeQuadTree.NativeQuadTree QuadTree;

    private NativeArray<int> Grid;

    NativeArray<PathNode> pathNodeArray;

    NativeArray<int2> neighbourOffsetArray;

    List<jobCollection> jobCollections;

    NativeArray<ObstacleStruct> obstacleArray;

    bool initialized = false;


    protected override void OnDestroy()
    {
        base.OnDestroy();
        disposeArrays();
       
    }

    private struct jobCollection
    {
        public NativeArray<float> CostSoFar;
        public NativeArray<int2> CameFrom;
        public NativeMinHeap OpenSet;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        //initializing the quad tree with hardcoded boundries for testing
        //TODO: make the boundries and tree reactive to the current room/environment
        //QuadTree = new NativeQuadTree.NativeQuadTree(new NativeQuadTree.AABB2D(new float2(500, 500), new float2(1000, 1000)));


    }

    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {

        if(!initialized)
        {
            initializePersistentArrays(numberWorkerThreads, gridParams);
            initialized = true;
        }

        //QuadTree.ClearAndBulkInsert(obstacleArray);

        //NativeList<ObstacleStruct> queryResult = new NativeList<ObstacleStruct>(Allocator.Temp);

        //QuadTree.RangeQuery(new NativeQuadTree.AABB2D(new float2(500, 500), new float2(1000, 1000)), queryResult);

        //Debug.Log(queryResult);

       


        EntityCommandBuffer concurrentCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<PathfindingParams>());

        NativeArray<PathfindingParams> pathFindingArray = entityQuery.ToComponentDataArray<PathfindingParams>(Allocator.TempJob);
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.TempJob);

       

        NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(jobCollections.Count, Allocator.TempJob);
        for (int index = 0; index < jobCollections.Count; index++)
        {
            jobHandles[index] = jobHandle;
        }

        for (int r = 0; r < entityArray.Length; r++)
        {
            int index = r % jobCollections.Count;
            jobCollection data = jobCollections[index];
            PathfindingParams pathfindingParams = pathFindingArray[r];

            FindPathJob findPathJob = new FindPathJob
            {
                DimX = gridParams.x,
                DimY = gridParams.y,
                Neighbours = neighbourOffsetArray,
                startPosition = pathfindingParams.startPosition,
                endPosition = pathfindingParams.endPosition,
                itterationLimit = itterationLimit,
                Grid = ObstacleController.getNativeMap(),
                CostSoFar = data.CostSoFar,
                CameFrom = data.CameFrom,
                entity = entityArray[r],
                OpenSet = data.OpenSet

            };

            JobHandle findPathHandle = findPathJob.Schedule(jobHandles[index]);



            ResetJob resetJob = new ResetJob
            {
                CostSoFar = data.CostSoFar,
                OpenSet = data.OpenSet,
                CameFrom = data.CameFrom,
                DimX = gridParams.x,
                entity = entityArray[r],
                pathfindingParamsComponentDataFromEntity = GetComponentDataFromEntity<PathfindingParams>(),
                pathFollowComponentDataFromEntity = GetComponentDataFromEntity<PathFollow>(),
                pathPositionBufferFromEntity = GetBufferFromEntity<PathPosition>(),
                startPosition = pathfindingParams.startPosition,
                endPosition = pathfindingParams.endPosition,


            };

            jobHandles[index] = resetJob.Schedule(findPathHandle);
            //PostUpdateCommands.RemoveComponent<PathfindingParams>(entity);
            concurrentCommandBuffer.RemoveComponent<PathfindingParams>(entityArray[r]);


        }

        jobHandle = JobHandle.CombineDependencies(jobHandles);
        entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        entityArray.Dispose();
        pathFindingArray.Dispose();

        jobHandles.Dispose();


        return jobHandle;
    }


    [BurstCompile]
    private struct FindPathJob : IJob
    {
        [ReadOnly] public int DimX;
        [ReadOnly] public int DimY;
        public int itterationLimit;
        [ReadOnly] public int2 startPosition;
        public int2 endPosition;
        [ReadOnly] public NativeArray<int2> Neighbours;

        //TODO: need to decouple the pathnode struct into multiple arrays to allow for easier parrelization
        [ReadOnly] public NativeArray<int> Grid;

        //Waypoints resembles the path buffer, may need to do research to use something else

        public NativeArray<float> CostSoFar;
        public NativeArray<int2> CameFrom;

        public Entity entity;

        public NativeMinHeap OpenSet;

        public void Execute()
        {

            FindPath(startPosition, endPosition);

        }

        private void FindPath(int2 startPosition, int2 endPosition)
        {
            if(startPosition.Equals(endPosition))
            {
               
                return;
            }

            MinHeapNode head = new MinHeapNode(startPosition, CalculateDistanceCost(startPosition, endPosition));
            OpenSet.Push(head);

           

            while (itterationLimit > 0 && OpenSet.HasNext())
            {
                int currentIndex = OpenSet.Pop();
                MinHeapNode current = OpenSet[currentIndex];

                

                if(current.Position.Equals(endPosition))
                {

                    
                    //Found our destination, we will let the cleanup job handle the path reconstruction for now
                    //ReconstructPath(startPosition, endPosition);
                    return;
                }

                float initialCost = CostSoFar[GetIndex(current.Position)];

                for(int i = 0; i < Neighbours.Length; i++)
                {
                    int2 neighbour = Neighbours[i];
                    int2 position = current.Position + neighbour;

                    if (position.x < 0 || position.x >= DimX || position.y < 0 || position.y >= DimY)
                        continue;

                    int index = GetIndex(position);

                    float cellCost = GetCellCost(currentIndex, index, true);

                    if (float.IsInfinity(cellCost))
                        continue;

                    float neighbourCost = 1;
                    if((math.abs(neighbour.x) + math.abs(neighbour.y)) == 2)
                    {
                        neighbourCost = 1.4f;
                    }

                    float newCost = initialCost + neighbourCost + cellCost;
                    float oldCost = CostSoFar[index];

                    if(!(oldCost <= 0) && !(newCost < oldCost))
                    {
                        continue;
                    }

                    CostSoFar[index] = newCost;
                    CameFrom[index] = current.Position;

                    float expectedCost = newCost + CalculateDistanceCost(position, endPosition);
                    OpenSet.Push(new MinHeapNode(position, expectedCost));

                    
                }

                itterationLimit--;
            }

            if(OpenSet.HasNext())
            {
                //We ran out of itterations
               
                //We will just give out where we stapped at for now
                //TODO: fix this
                var currentIndex = OpenSet.Pop();
                endPosition = OpenSet[currentIndex].Position;
                
            }

        }

        private float GetCellCost(int fromIndex, int toIndex, bool areNeighbours)
        {
            int cell = Grid[toIndex];
            if (cell == -1)
                return float.PositiveInfinity;

            // TODO HEIGHT ADJUSTMENTS ETC

            return cell;
        }

        private int GetIndex(int2 position)
        {
            return position.x + (position.y * DimX);
        }

       


    }

    /// <summary>
    ///     Job that resets our CostSoFar array and Heap, the CameFrom array does not need resetting
    /// </summary>
    [BurstCompile]
    private unsafe struct ResetJob : IJob
    {
        [WriteOnly] public NativeArray<float> CostSoFar;
        [WriteOnly] public NativeMinHeap OpenSet;
        [ReadOnly] public NativeArray<int2> CameFrom;
        [ReadOnly] public int DimX;
        public Entity entity;
        public int index;

        public ComponentDataFromEntity<PathfindingParams> pathfindingParamsComponentDataFromEntity;
        public ComponentDataFromEntity<PathFollow> pathFollowComponentDataFromEntity;
        public BufferFromEntity<PathPosition> pathPositionBufferFromEntity;

        [ReadOnly] public int2 startPosition;
        [ReadOnly] public int2 endPosition;


        public void Execute()
        {
            DynamicBuffer<PathPosition> pathPositionBuffer = pathPositionBufferFromEntity[entity];
            pathPositionBuffer.Clear();

            PathfindingParams pathfindingParams = pathfindingParamsComponentDataFromEntity[entity];

            

            CalculatePath(startPosition, endPosition, pathPositionBuffer);

            pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1, NewPath = true };


            var buffer = CostSoFar.GetUnsafePtr();
            UnsafeUtility.MemClear(buffer, (long)CostSoFar.Length * UnsafeUtility.SizeOf<float>());

            OpenSet.Clear();

        }
       

        private void CalculatePath(int2 startPosition, int2 endPosition, DynamicBuffer<PathPosition> pathPositionBuffer)
        {
                  
            pathPositionBuffer.Add(new PathPosition { position = new int2(endPosition.x, endPosition.y) });

            var current = endPosition;
            do
            {
                var previous = CameFrom[GetIndex(current)];
                current = previous;
                pathPositionBuffer.Add(new PathPosition { position = current });
               
            } while (!current.Equals(startPosition));

            
        }

        private int GetIndex(int2 position)
        {
            return position.x + (position.y * DimX);
        }

    }


   

    private static void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode, DynamicBuffer<PathPosition> pathPositionBuffer)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            // Couldn't find a path!
        }
        else
        {
            // Found a path
            pathPositionBuffer.Add(new PathPosition { position = new int2(endNode.x, endNode.y) });

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                pathPositionBuffer.Add(new PathPosition { position = new int2(cameFromNode.x, cameFromNode.y) });
                currentNode = cameFromNode;
            }
        }
    }

    private static NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
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

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1)
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
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


    private static int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }


    private void initializePersistentArrays(int numberThreads, int2 gridParams)
    {
        neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Persistent);
        neighbourOffsetArray[0] = new int2(-1, 0); // Left
        neighbourOffsetArray[1] = new int2(+1, 0); // Right
        neighbourOffsetArray[2] = new int2(0, +1); // Up
        neighbourOffsetArray[3] = new int2(0, -1); // Down
        neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
        neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
        neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
        neighbourOffsetArray[7] = new int2(+1, +1); // Right Up


        //Initialize the arrays for the worker threads to use in parrallel
        jobCollections = new List<jobCollection>(numberThreads);

        for(int i = 0; i < numberThreads; i++)
        {
            jobCollection collection = new jobCollection
            {
                CostSoFar = new NativeArray<float>(gridParams.x * gridParams.y, Allocator.Persistent),
                CameFrom = new NativeArray<int2>(gridParams.x * gridParams.y, Allocator.Persistent),
                OpenSet = new NativeMinHeap(gridParams.x * gridParams.y, Allocator.Persistent)
            };

            jobCollections.Add(collection);
        }


    }

    private void disposeArrays()
    {
        neighbourOffsetArray.Dispose();

        for (int i = 0; i < jobCollections.Count; i++)
        {
            jobCollection collection = jobCollections[i];
            collection.CameFrom.Dispose();
            collection.CostSoFar.Dispose();
            collection.OpenSet.Dispose();
        }

        obstacleArray.Dispose();
    }


    ///TO Delete
    ///
    //[BurstCompile]
    //private struct SetBufferPathJob : IJob
    //{

    //    public int2 gridSize;

    //    [DeallocateOnJobCompletion]
    //    public NativeArray<PathNode> pathNodeArray;

    //    public Entity entity;

    //    public ComponentDataFromEntity<PathfindingParams> pathfindingParamsComponentDataFromEntity;
    //    public ComponentDataFromEntity<PathFollow> pathFollowComponentDataFromEntity;
    //    public BufferFromEntity<PathPosition> pathPositionBufferFromEntity;

    //    public void Execute()
    //    {
    //        DynamicBuffer<PathPosition> pathPositionBuffer = pathPositionBufferFromEntity[entity];
    //        pathPositionBuffer.Clear();

    //        PathfindingParams pathfindingParams = pathfindingParamsComponentDataFromEntity[entity];
    //        int endNodeIndex = CalculateIndex(pathfindingParams.endPosition.x, pathfindingParams.endPosition.y, gridSize.x);
    //        PathNode endNode = pathNodeArray[endNodeIndex];
    //        if (endNode.cameFromNodeIndex == -1)
    //        {
    //            // Didn't find a path!
    //            //Debug.Log("Didn't find a path!");
    //            pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = -1 };
    //        }
    //        else
    //        {
    //            // Found a path
    //            CalculatePath(pathNodeArray, endNode, pathPositionBuffer);

    //            pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1 };
    //        }

    //    }
    //}


    //protected override void OnUpdate()
    //{

    //    if (pathNodeArray.IsCreated == false)
    //    {
    //        ObstacleController.instance.initializePathNodeArray(gridWidth, gridHeight);

    //        //TODO: I don't know if in need to fetch this from the obstacle controller each update because it is a struct and not a reference
    //        pathNodeArray = ObstacleController.instance.GetPathNodeArray();

    //    }

    //    if (nativeInt == null)
    //    {
    //        nativeInt = new NativeArray<int>();
    //    }



    //    List<FindPathJob> findPathJobList = new List<FindPathJob>();
    //    NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);



    //    Entities.ForEach((Entity entity, ref PathfindingParams pathfindingParams) =>
    //    {

    //        // NativeArray<PathNode> tmpPathNodeArray = new NativeArray<PathNode>(pathNodeArray, Allocator.TempJob);

    //        FindPathJob findPathJob = new FindPathJob
    //        {
    //            gridSize = gridSize,
    //            pathNodeArray = new NativeArray<PathNode>(pathNodeArray, Allocator.TempJob),
    //            startPosition = pathfindingParams.startPosition,
    //            endPosition = pathfindingParams.endPosition,
    //            entity = entity,
    //            neighbourOffsetArray = neighbourOffsetArray
    //        };
    //        findPathJobList.Add(findPathJob);
    //        jobHandleList.Add(findPathJob.Schedule());

    //        //tmpPathNodeArray.Dispose();

    //        PostUpdateCommands.RemoveComponent<PathfindingParams>(entity);
    //    });

    //    JobHandle.CompleteAll(jobHandleList);

    //    foreach (FindPathJob findPathJob in findPathJobList)
    //    {
    //        new SetBufferPathJob
    //        {
    //            entity = findPathJob.entity,
    //            gridSize = findPathJobOne.gridSize,
    //            pathNodeArray = findPathJob.pathNodeArray,
    //            pathfindingParamsComponentDataFromEntity = GetComponentDataFromEntity<PathfindingParams>(),
    //            pathFollowComponentDataFromEntity = GetComponentDataFromEntity<PathFollow>(),
    //            pathPositionBufferFromEntity = GetBufferFromEntity<PathPosition>(),
    //        }.Run();
    //    }

    //    //pathNodeArray.Dispose();
    //}



    //[BurstCompile]
    //private struct FindPathJobOne : IJob
    //{

    //    public int2 gridSize;
    //    public NativeArray<PathNode> pathNodeArray;

    //    public int2 startPosition;
    //    public int2 endPosition;

    //    public Entity entity;

    //    [ReadOnly] public NativeArray<int2> neighbourOffsetArray;

    //    //public BufferFromEntity<PathPosition> pathPositionBuffer;

    //    public void Execute()
    //    {
    //        for (int i = 0; i < pathNodeArray.Length; i++)
    //        {
    //            PathNode pathNode = pathNodeArray[i];
    //            pathNode.hCost = CalculateDistanceCost(new int2(pathNode.x, pathNode.y), endPosition);
    //            pathNode.cameFromNodeIndex = -1;

    //            pathNodeArray[i] = pathNode;
    //        }



    //        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

    //        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
    //        startNode.gCost = 0;
    //        startNode.CalculateFCost();
    //        pathNodeArray[startNode.index] = startNode;

    //        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
    //        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

    //        openList.Add(startNode.index);

    //        while (openList.Length > 0)
    //        {
    //            int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
    //            PathNode currentNode = pathNodeArray[currentNodeIndex];

    //            if (currentNodeIndex == endNodeIndex)
    //            {
    //                // Reached our destination!
    //                break;
    //            }

    //            // Remove current node from Open List
    //            for (int i = 0; i < openList.Length; i++)
    //            {
    //                if (openList[i] == currentNodeIndex)
    //                {
    //                    openList.RemoveAtSwapBack(i);
    //                    break;
    //                }
    //            }

    //            closedList.Add(currentNodeIndex);

    //            for (int i = 0; i < neighbourOffsetArray.Length; i++)
    //            {
    //                int2 neighbourOffset = neighbourOffsetArray[i];
    //                int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

    //                if (!IsPositionInsideGrid(neighbourPosition, gridSize))
    //                {
    //                    // Neighbour not valid position
    //                    continue;
    //                }

    //                int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

    //                if (closedList.Contains(neighbourNodeIndex))
    //                {
    //                    // Already searched this node
    //                    continue;
    //                }

    //                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
    //                if (!neighbourNode.isWalkable)
    //                {
    //                    // Not walkable
    //                    continue;
    //                }

    //                int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

    //                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
    //                if (tentativeGCost < neighbourNode.gCost)
    //                {
    //                    neighbourNode.cameFromNodeIndex = currentNodeIndex;
    //                    neighbourNode.gCost = tentativeGCost;
    //                    neighbourNode.CalculateFCost();
    //                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

    //                    if (!openList.Contains(neighbourNode.index))
    //                    {
    //                        openList.Add(neighbourNode.index);
    //                    }
    //                }

    //            }
    //        }

    //        //pathPositionBuffer.Clear();

    //        /*
    //        PathNode endNode = pathNodeArray[endNodeIndex];
    //        if (endNode.cameFromNodeIndex == -1) {
    //            // Didn't find a path!
    //            //Debug.Log("Didn't find a path!");
    //            pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = -1 };
    //        } else {
    //            // Found a path
    //            //CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
    //            //pathFollowComponentDataFromEntity[entity] = new PathFollow { pathIndex = pathPositionBuffer.Length - 1 };
    //        }
    //        */

    //        //neighbourOffsetArray.Dispose();
    //        openList.Dispose();
    //        closedList.Dispose();
    //    }


    //}


}
