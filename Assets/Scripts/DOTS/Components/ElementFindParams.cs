using Unity.Entities;
using Unity.Mathematics;

public struct ElementFindParams : IComponentData
{

    public int2 position;
    public int range;
    public Element element;

}
