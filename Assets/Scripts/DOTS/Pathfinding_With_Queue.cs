//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Mathematics;
//using Unity.Collections;
//using Unity.Jobs;
//using Unity.Burst;

//public class Pathfinding_With_Queue : MonoBehaviour
//{

//    private const int MOVE_STRAIGHT_COST = 10;
//    private const int MOVE_DIAGONAL_COST = 14;
//    private NativeArray<int2> neighbourOffsetArray;
//    private NativeMinHeap OpenSet;
//    private int2 gridSize = new int2(20, 20);
//    private int itterationLimit = 10000;
//    private NativeArray<PathNode> 

//    private void Start()
//    {
//        neighbourOffsetArray = new NativeArray<int2>(new int2[]
//            {
//            new int2(-1, 0), //Left
//            new int2(+1, 0), //Right
//            new int2(0, +1), //Up
//            new int2(0, -1), //Down
//            new int2(-1, +1), //Left UP
//            new int2(-1, -1), //Left Down
//            new int2(+1, +1), //Right Up
//            new int2(+1, -1), //Right Down


//            }, Allocator.Persistent);

//        OpenSet = new NativeMinHeap();



//        //Initialize an array to hold all nodes, native arry so make sure to dispose with it when done
//        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Persistent);

//        for (int x = 0; x < gridSize.x; x++)
//        {
//            for (int y = 0; y < gridSize.y; y++)
//            {
//                PathNode node = new PathNode();
//                node.x = x;
//                node.y = y;

//                node.index = calculateIndex(x, y, gridSize.x);

//                node.gCost = int.MaxValue;

//                node.hCost = int.MaxValue;

//                node.CalculateFCost();

//                node.cameFromNodeIndex = -1;

//                pathNodeArray[node.index] = node;

//            }
//        }


//    }

//    private void Update()
//    {

//        int findPathJobCount = 5;
//        NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.Temp);


//        for (int i = 0; i < findPathJobCount; i++)
//        {
//            //FindPath(new int2(0, 0), new int2(19, 19));
//            FindPathJob findPathJob = new FindPathJob
//            {
//                startPosition = new int2(0, 0),
//                endPosition = new int2(19, 19)
//            };

//            jobHandleArray[i] = findPathJob.Schedule();
//            //findPathJob.Run();


//        }

//        JobHandle.CompleteAll(jobHandleArray);

//        jobHandleArray.Dispose();
//    }

//    [BurstCompile]
//    private struct FindPathJob : IJob
//    {

//        public int2 startPosition;
//        public int2 endPosition;
//        public int itterationLimit;

//        [ReadOnly] public NativeArray<int2> neighbourOffsetArray;
//        [ReadOnly] public NativeArray<PathNode> pathNodeArray;
//        [ReadOnly] public int2 gridSize;

//        public NativeMinHeap OpenSet;


//        public void Execute()
//        {

//        }

//        private void FindPath(int2 startPosition, int2 endPosition)
//        {

//            if (startPosition.Equals(endPosition))
//            {
//                return;
//            }


//            var head = new MinHeapNode(startPosition, CalculateDistanceCost(startPosition, endPosition));

//            OpenSet.Push(head);

//            //Get end node index so we can easily check to see if we have made it to the endpoint/goal
//            int endNodeIndex = calculateIndex(endPosition.x, endPosition.y, gridSize.x);


//            //Get the first node and calculate costs
//            PathNode startNode = pathNodeArray[calculateIndex(startPosition.x, startPosition.y, gridSize.x)];
//            startNode.gCost = 0;
//            startNode.CalculateFCost();

//            //Note we are using structs and not references to objects, so need to update the actual value
//            pathNodeArray[startNode.index] = startNode;



//            //Setup a list of nodes we have already checked and don't want to check again
//            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

//            //Add the starting node to the open list, as we must start by checking the path from there
//            openList.Add(startNode.index);

//            //Keep checking nodes untill the open list is empty (no path found) or we have made it to the destination
//            while (openList.Length > 0)
//            {
//                //Get the lowest fCost node, which is our best guess as to what the best node to check will be
//                int currentPathNodeIndex = GetLowestFCostIndex(openList, pathNodeArray);
//                PathNode currentNode = pathNodeArray[currentPathNodeIndex];

//                if (currentPathNodeIndex == endNodeIndex)
//                {
//                    //We have made it to the goal, now need to reconstruct the path there
//                    break;
//                }

//                //Havent made it to the destaination, so need to keep checking from cuurrent node
//                //Need to remove current node the from open list
//                //TODO: this function is inefficient, can optimize, but with burst compiller it may not be a big issue
//                for (int i = 0; i < openList.Length; i++)
//                {
//                    if (openList[i] == currentPathNodeIndex)
//                    {
//                        openList.RemoveAtSwapBack(i);
//                        break;
//                    }
//                }

//                //Add current node index to the closed list, as we are evaluating it now, and dont want to again
//                closedList.Add(currentPathNodeIndex);

//                //Loop through all neighbours of current node
//                for (int i = 0; i < neighbourOffsetArray.Length; i++)
//                {
//                    int2 neighbourOffset = neighbourOffsetArray[i];
//                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

//                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
//                    {
//                        //Neighbour is not a valid position inside grid, so lets skip
//                        continue;
//                    }

//                    int neighbourIndex = calculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

//                    if (closedList.Contains(neighbourIndex))
//                    {
//                        //neighbour is in closed list, so we have already examined it, so can skip
//                        continue;
//                    }

//                    PathNode neighbourNode = pathNodeArray[neighbourIndex];
//                    if (!neighbourNode.isWalkable)
//                    {
//                        //Can't walk on this node, so lets skip
//                        continue;
//                    }

//                    //This node passed through all checks, so its valid, now lefts evaluate its costs to get there

//                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
//                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);

//                    //if the estimated gCost is better than any other gCosts calculated for this node (all nodes start with a max gCost) then this is the path path to the current node so far
//                    //so lets evaluate further
//                    if (tentativeGCost < neighbourNode.gCost)
//                    {
//                        //change the nodes cameFromIndex to reflect that we are coming from the current node
//                        neighbourNode.cameFromNodeIndex = currentPathNodeIndex;
//                        neighbourNode.gCost = tentativeGCost;
//                        neighbourNode.CalculateFCost();
//                        pathNodeArray[neighbourIndex] = neighbourNode;

//                        if (!openList.Contains(neighbourIndex))
//                        {
//                            openList.Add(neighbourIndex);
//                        }

//                    }

//                }


//            }

//            PathNode endNode = pathNodeArray[endNodeIndex];

//            NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

//            //TODO: return the path, untill then lets just dispose of the path
//            path.Dispose();


//            //TODO: may not need to dispose of it, may hold onto it from frame to frame, have to look into how native arrays work
//            //Done with array, so dispose of it
//            pathNodeArray.Dispose();
//            neighbourOffsetArray.Dispose();
//            openList.Dispose();
//            closedList.Dispose();


//        }
//    }

    

//    static NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
//    {
//        NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
//        if (endNode.cameFromNodeIndex == -1)
//        {
//            //Couldn't find a path
//            return path;
//        }

//        //Found path! now so need to walk backwords to construct actual path

//        //Start with adding the endNode and initializing current node as endNode
//        path.Add(new int2(endNode.x, endNode.y));
//        PathNode currentNode = endNode;

//        //Lets walk backwards untill we reach a node which didnt come from anywhere ie: was the start node
//        while (currentNode.cameFromNodeIndex != -1)
//        {
//            PathNode tempNode = pathNodeArray[currentNode.cameFromNodeIndex];
//            path.Add(new int2(tempNode.x, tempNode.y));

//            currentNode = tempNode;
//        }

//        return path;

//    }

//    private static bool IsPositionInsideGrid(int2 position, int2 gridSize)
//    {
//        return
//            position.x >= 0 &&
//            position.y >= 0 &&
//            position.x < gridSize.x &&
//            position.y < gridSize.y;
//    }

//    static private int calculateIndex(int x, int y, int gridWidth)
//    {
//        return x + y * gridWidth;
//    }


//    //This calculates the distance from a neighbouring node to another
//    //TODO: need to include added weights that affect walking on it, ie: fire and tough terrain
//    private static int CalculateDistanceCost(int2 aPosition, int2 bPosition)
//    {
//        int xDistance = math.abs(aPosition.x - bPosition.x);
//        int yDistance = math.abs(aPosition.y - bPosition.y);
//        int remaining = math.abs(xDistance - yDistance);

//        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

//    }

//    private static int GetLowestFCostIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
//    {
//        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
//        for (int i = 1; i < openList.Length; i++)
//        {
//            PathNode testPathNode = pathNodeArray[openList[i]];

//            if (testPathNode.fCost < lowestCostPathNode.fCost)
//            {
//                lowestCostPathNode = testPathNode;
//            }
//        }

//        return lowestCostPathNode.index;
//    }


//    private struct PathNode
//    {
//        public int x;
//        public int y;

//        public int index;

//        public int gCost;
//        public int fCost;
//        public int hCost;

//        public bool isWalkable;

//        public int cameFromNodeIndex;

//        public void CalculateFCost()
//        {
//            fCost = gCost + fCost;
//        }
//    }

//}
