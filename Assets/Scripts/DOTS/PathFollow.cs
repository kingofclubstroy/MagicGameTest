using Unity.Entities;

[GenerateAuthoringComponent]
public struct PathFollow : IComponentData
{

    public int pathIndex;
    public bool NewPath;
}
