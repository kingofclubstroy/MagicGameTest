using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;

[DisableAutoCreation]
public class ElementFindTest : JobComponentSystem
{

    //NativeQuadTree.NativeQuadTree<int> QuadTree;

    protected override void OnCreate()
    {
        base.OnCreate();

        NativeQuadTree.AABB2D aabb = new NativeQuadTree.AABB2D(new float2(500, 500), new float2(1000, 1000));
        //initializeQuadTree(aabb);

        //entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //disposeQuadTree();
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        //EntityCommandBuffer concurrentCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<ElementFindParams>());

        NativeArray<ElementFindParams> pathFindingArray = entityQuery.ToComponentDataArray<ElementFindParams>(Allocator.TempJob);
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.TempJob);

        if(entityArray.Length < 0)
        {
            return inputDeps;

        }

        //We have some values to process, so lets create the quad tree
        //AddBulkJob<int> addBulkJob = new AddBulkJob<int>
        //{
        //    QuadTree = QuadTree,
        //    Elements = 
        //}



        return inputDeps;
       

        
    }

//    [BurstCompile]
//    public struct AddBulkJob<T> : IJob where T : unmanaged
//    {
//        [ReadOnly]
//        public NativeArray<NativeQuadTree.QuadElement<T>> Elements;

//        //public NativeQuadTree.NativeQuadTree<T> QuadTree;

//        public void Execute()
//        {
//            QuadTree.ClearAndBulkInsert(Elements);
//        }
//    }

//    private void initializeQuadTree(NativeQuadTree.AABB2D aabb)
//    {
//         QuadTree = new NativeQuadTree.NativeQuadTree<int>(aabb, Allocator.Persistent);
//}

//    private void disposeQuadTree()
//    {
//        QuadTree.Dispose();
//    }

    
}
