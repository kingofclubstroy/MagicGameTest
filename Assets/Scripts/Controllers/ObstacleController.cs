﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;


//TODO: need to go through this after im done with pathfinding and look into how much of this is needed and whether or not we even need this and instead go a full ecs route having the obstacle map
//as a component that the other systems have read access to, while updating it is another component that gets run first

    //TODO: make sure to change up the functions so other controllers are updateing the native map, not the regular dictionary
public class ObstacleController : MonoBehaviour
{
    public static ObstacleController instance;

    //TODO: need this to react to room size
    static Dictionary<Vector2, int> obstacleMap = new Dictionary<Vector2, int>();

    public NativeArray<PathNode> pathNodeArray;

    public NativeArray<int2> neighbourOffsetArray;

    public static NativeArray<int> nativeObstacleMap;
    static int2 gridParams;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

       
       
        

    }

    private void OnDestroy()
    {
        //pathNodeArray.Dispose();
        //neighbourOffsetArray.Dispose();
        //nativeObstacleMap.Dispose();
    }

    public int GetObstacleResistance(Vector2 pos)
    {
        if (obstacleMap.ContainsKey(pos))
        {
            return obstacleMap[pos];
        } else
        {
            return 0;
        }
    }

    public void SetObstacleMap(Vector2 pos, int value)
    {
        
        obstacleMap[pos] = value;
    }


    int H(Vector2 current, Vector2 goal)
    {
        return (int)Vector2.Distance(current, goal);
    }

    int d(Node current, Node neighbour)
    {
        return GetObstacleResistance(neighbour.position) + 1;
    }

    

    // A* finds a path from start to goal.
    // h is the heuristic function. h(n) estimates the cost to reach goal from node n.
    public List<Node> A_Star(Vector2 startPosition, Vector2 goalPosition, int h)
    {

        //need to round the start and target position to whole integers
        startPosition.x = Mathf.Round(startPosition.x);
        startPosition.y = Mathf.Round(startPosition.y);
        goalPosition.x = Mathf.Round(goalPosition.x);
        goalPosition.y = Mathf.Round(goalPosition.y);

        HashSet<Vector2> checkedSet = new HashSet<Vector2>();


        Node start = new Node(startPosition, null);

        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        //PriorityQueue<Node> openSet = new PriorityQueue<Node>();
        //openSet.Enqueue(start);

        List<Node> openSet = new List<Node> { start };

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
        // to n currently known.
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        Dictionary<Node, int> gScore = new Dictionary<Node, int>();
        gScore[start] = 0;

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how short a path from start to finish can be if it goes through n.
        Dictionary<Node, int> fScore = new Dictionary<Node, int>();
        fScore[start] = H(startPosition, goalPosition);

        while (openSet.Count > 0)
        {
            // This operation can occur in O(1) time if openSet is a min-heap or a priority queue
            Node current = GetLowestPriority(openSet);
            //openSet.dequeue;

        //Debug.Log("currentPosition  = " + current.position);
            if (current.position == goalPosition) {
                return reconstructPath(cameFrom, current);
            }


            List<Node> neighbours = current.GetNeighbours();

        //Debug.Log("neighbours = " + neighbours);

            foreach (Node neighbor in neighbours) {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // tentative_gScore is the distance from start to the neighbor through current
                int tentative_gScore = gScore[current] + d(current, neighbor);
                if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor]) {
                // This path to neighbor is better than any previous one. Record it!
                    //Debug.Log("gscore better ! tenitive gscore = " + tentative_gScore);
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    //fScore[neighbor] = gScore[neighbor] + H(neighbor.position, goalPosition);
                    neighbor.priority = gScore[neighbor] + H(neighbor.position, goalPosition);
                    //if (!openSet.IsInQueue(neighbor)) {

                    if(!checkedSet.Contains(neighbor.position))
                    {
                        //openSet.Enqueue(neighbor);
                        openSet.Add(neighbor);
                        checkedSet.Add(neighbor.position);

                    }
    
                }
            }

        }

        // Open set is empty but goal was never reached
        return null;


    }

    List<Node> reconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node>();

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;

    }

    Node GetLowestPriority(List<Node> openset)
    {
        int lowest = -1;
        Node best = null;

        foreach(Node node in openset)
        {
            if(lowest == -1 || node.priority < lowest)
            {
                lowest = node.priority;
                best = node;
            }
        }

        openset.Remove(best);
        return best;
    }

    //TODO: currently making this public to initialize via the pathfinding system, since we dont have a proper understanding of the size of the rooms yet
    public void initializePathNodeArray(int width, int height)
    {
            pathNodeArray = new NativeArray<PathNode>(width * height, Allocator.Persistent);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, width);

                    pathNode.gCost = int.MaxValue;

                    //TODO: this is where we set some stuff with obstacles
                    pathNode.isWalkable = true;
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            
    }

    public static NativeArray<int> getNativeMap(int2 gridParams)
    {
        if(!nativeObstacleMap.IsCreated)
        {
            return initializeNativeObstacleMap(gridParams.x, gridParams.y);
        }
        return nativeObstacleMap;
    }
    
    public static NativeArray<int> initializeNativeObstacleMap(int gridWidth, int gridHeight)
    {
        nativeObstacleMap = new NativeArray<int>(gridWidth * gridHeight, Allocator.Persistent);
        gridParams = new int2(gridWidth, gridHeight);

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                int index = x + (gridWidth * y);
                nativeObstacleMap[index] = 0;
            }
        }

        return nativeObstacleMap;
    }



    public NativeArray<PathNode> GetPathNodeArray()
    {
        return pathNodeArray;
    }

    private int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

}