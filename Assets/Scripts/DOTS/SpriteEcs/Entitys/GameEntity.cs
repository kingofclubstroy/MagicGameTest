﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class GameEntity : MonoBehaviour
{

    private static GameEntity instance;
    public Mesh quadMesh;
    public Material walkingSpriteSheetMaterial;

    public static GameEntity GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            
                typeof(Translation),
                typeof(SpriteSheetComponentData)
            
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(40, Allocator.Temp);

        entityManager.CreateEntity(entityArchetype, entityArray);

        int x = 0;
        int y = 0;

       

        foreach (Entity entity in entityArray)
        {
            entityManager.SetComponentData(entity, new Translation
            {
                //Value = new float3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0)
                //Value = new float3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0)
                //Value = new float3(0, i, 0)

                Value = new float3(x, y, 0)

            });

            //entityManager.SetComponentData(entity, new SpriteSheetComponentData
            //{
            //    currentFrame = UnityEngine.Random.Range(0, 4),
            //    frameCount = 4,
            //    frameTimer = UnityEngine.Random.Range(0f, 1f),
            //    frameTimerMax = 0.1f

            //});

            entityManager.SetComponentData(entity, new SpriteSheetComponentData
            {
                currentFrame = UnityEngine.Random.Range(0, 3),
                frameCount = 3,
                frameTimer = UnityEngine.Random.Range(0f, 4f),
                frameTimerMax = 0.1f

            });

            if (x >= 40)
            {
                x = 0;
                y += 4;
            }
            else
            {
                x += 4;
            }

            //x += 0.14f;
            //y += 0.14f;






        }

        entityArray.Dispose();
    }
}
