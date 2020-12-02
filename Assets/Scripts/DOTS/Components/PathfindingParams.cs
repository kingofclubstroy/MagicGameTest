using Unity.Entities;
using Unity.Mathematics;

public struct PathfindingParams : IComponentData
{

    public int2 startPosition;
    public int2 endPosition;

    public PathfindingParams(int2 start, int2 end)
    {
        startPosition = start;
        endPosition = end;
    } 

}
