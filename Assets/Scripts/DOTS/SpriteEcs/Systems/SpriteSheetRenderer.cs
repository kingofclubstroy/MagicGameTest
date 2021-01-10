using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

[UpdateAfter(typeof(SpriteSheetAnimationSystem))]
[DisableAutoCreation]
public class SpriteSheetRenderer : ComponentSystem
{

    private struct RenderData
    {
        public Entity entity;
        public float3 position;
        public Matrix4x4 matrix;
        public Vector4 uv;
    }

    [BurstCompile]
    private struct CullAndSortJob : IJobForEachWithEntity<Translation, SpriteSheetComponentData> {

        public float yTop_1;
        public float yTop_2;
        public float yBottom;


        public NativeQueue<RenderData>.ParallelWriter nativeQueue1;
        public NativeQueue<RenderData>.ParallelWriter nativeQueue2;



        public void Execute(Entity entity, int index, ref Translation translation, ref SpriteSheetComponentData spriteSheetComponentData)
        {

            
            float positionY = translation.Value.y;
            if(positionY > yBottom && positionY < yTop_1)
            {
                //Valid position
                RenderData renderData = new RenderData {

                    entity = entity,
                    position = translation.Value,
                    matrix = spriteSheetComponentData.matrix,
                    uv = spriteSheetComponentData.uv

                };



                if(positionY < yTop_2)
                {
                    nativeQueue2.Enqueue(renderData);
                } else
                {
                    nativeQueue1.Enqueue(renderData);
                }
            }
        }

    }

    [BurstCompile]
    private struct NativeQueueToArrayJob : IJob
    {
        public NativeQueue<RenderData> nativeQueue;
        public NativeArray<RenderData> nativeArray;

        public void Execute()
        {
            int index = 0;
            RenderData renderData;
            while(nativeQueue.TryDequeue(out renderData))
            {
                nativeArray[index] = renderData;
                index++;
            }
        }
    }

    [BurstCompile]
    private struct SortByPositionJob : IJob
    {

        public NativeArray<RenderData> dataArray;

        public void Execute()
        {
            for (int i = 0; i < dataArray.Length; i++)
            {
                for (int j = i + 1; j < dataArray.Length; j++)
                {
                    if (dataArray[i].position.y < dataArray[j].position.y)
                    {
                        //SWAP
                        RenderData temp = dataArray[i];
                        dataArray[i] = dataArray[j];
                        dataArray[j] = temp;

                    }
                }
            }
        }

    }

    [BurstCompile]
    private struct FillArraysParrallelJob : IJobParallelFor
    {

        [ReadOnly] public NativeArray<RenderData> nativeArray;
        [NativeDisableContainerSafetyRestriction] public NativeArray<Matrix4x4> matrixArray;
        [NativeDisableContainerSafetyRestriction] public NativeArray<Vector4> uvArray;
        public int startingIndex;

        public void Execute(int index)
        {
            RenderData renderData = nativeArray[index];
            matrixArray[startingIndex + index] = renderData.matrix;
            uvArray[startingIndex + index] = renderData.uv;
        }


    }


    protected override void OnUpdate()
    {

        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(SpriteSheetComponentData));

        NativeArray<SpriteSheetComponentData> animationDataQuery = entityQuery.ToComponentDataArray<SpriteSheetComponentData>(Allocator.TempJob);
        NativeArray<Translation> translationArray = entityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        NativeQueue<RenderData> nativeQueue_1 = new NativeQueue<RenderData>(Allocator.TempJob);
        NativeQueue<RenderData> nativeQueue_2 = new NativeQueue<RenderData>(Allocator.TempJob);

        Camera cameraMain = Camera.main;
        float3 cameraPosition = cameraMain.transform.position;

        float yBottom = cameraPosition.y - cameraMain.orthographicSize;
        float yTop_1 = cameraPosition.y + cameraMain.orthographicSize;
        float yTop_2 = cameraPosition.y + 0f;

        CullAndSortJob cullAndSortJob = new CullAndSortJob
        {
            yBottom = yBottom,
            yTop_1 = yTop_1,
            yTop_2 = yTop_2,

            nativeQueue1 = nativeQueue_1.AsParallelWriter(),
            nativeQueue2 = nativeQueue_2.AsParallelWriter()
        };

        JobHandle jobHandle = cullAndSortJob.Schedule(this);
        jobHandle.Complete();

        animationDataQuery.Dispose();
        translationArray.Dispose();

        NativeArray<RenderData> nativeArray_1 = nativeQueue_1.ToArray(Allocator.TempJob);
        NativeArray<RenderData> nativeArray_2 = nativeQueue_2.ToArray(Allocator.TempJob);

        nativeQueue_1.Dispose();
        nativeQueue_2.Dispose();

        NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(2, Allocator.TempJob);

        SortByPositionJob sortByPositionJob_1 = new SortByPositionJob
        {
            dataArray = nativeArray_1
        };

        jobHandleArray[0] = sortByPositionJob_1.Schedule();

        SortByPositionJob sortByPositionJob_2 = new SortByPositionJob
        {
            dataArray = nativeArray_2
        };

        jobHandleArray[1] = sortByPositionJob_2.Schedule();

        JobHandle.CompleteAll(jobHandleArray);

        int visibleEntityTotal = nativeArray_1.Length + nativeArray_2.Length;

        NativeArray<Matrix4x4> matrixArray = new NativeArray<Matrix4x4>(visibleEntityTotal, Allocator.TempJob);
        NativeArray<Vector4> uvArray = new NativeArray<Vector4>(visibleEntityTotal, Allocator.TempJob);

        FillArraysParrallelJob fillArraysParrallelJob_1 = new FillArraysParrallelJob
        {
            nativeArray = nativeArray_1,
            matrixArray = matrixArray,
            uvArray = uvArray,
            startingIndex = 0

        };

        jobHandleArray[0] = fillArraysParrallelJob_1.Schedule(nativeArray_1.Length, 10);


        FillArraysParrallelJob fillArraysParrallelJob_2 = new FillArraysParrallelJob
        {
            nativeArray = nativeArray_2,
            matrixArray = matrixArray,
            uvArray = uvArray,
            startingIndex = nativeArray_1.Length
        };

        jobHandleArray[1] = fillArraysParrallelJob_2.Schedule(nativeArray_2.Length, 10);

        JobHandle.CompleteAll(jobHandleArray);

        nativeArray_1.Dispose();
        nativeArray_2.Dispose();

        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        Vector4[] uv = new Vector4[1];

        Mesh quadMesh = GameEntity.GetInstance().quadMesh;
        Material material = GameEntity.GetInstance().walkingSpriteSheetMaterial;
        int shaderPropertyId = Shader.PropertyToID("_MainTex_UV");

        int sliceCount = 1023;

        Matrix4x4[] matrixInstancedArray = new Matrix4x4[sliceCount];
        Vector4[] uvInstancedArray = new Vector4[sliceCount];
        for(int i = 0; i < matrixArray.Length; i+= sliceCount)
        {

            int sliceSize = Mathf.Min(sliceCount, matrixArray.Length - i);

            NativeArray<Matrix4x4>.Copy(matrixArray, i, matrixInstancedArray, 0, sliceSize);
            NativeArray<Vector4>.Copy(uvArray, i, uvInstancedArray, 0, sliceSize);

            materialPropertyBlock.SetVectorArray(shaderPropertyId, uvInstancedArray);

            Graphics.DrawMeshInstanced(
                quadMesh,
                0,
                material,
                matrixInstancedArray,
                sliceSize,
                materialPropertyBlock
            );


        }

        

        matrixArray.Dispose();
        uvArray.Dispose();

        jobHandleArray.Dispose();

       
    }

}
