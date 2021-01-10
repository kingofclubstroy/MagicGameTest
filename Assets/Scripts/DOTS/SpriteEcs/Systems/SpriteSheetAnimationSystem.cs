using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class SpriteSheetAnimationSystem : JobComponentSystem
{

    [BurstCompile]
    public struct Job : IJobForEach<SpriteSheetComponentData, Translation>
    {
        public float deltaTime;
        public Quaternion cameraRotation;
        public void Execute(ref SpriteSheetComponentData spriteSheetComponentData, ref Translation translation)
        {
            spriteSheetComponentData.frameTimer += deltaTime;
            while (spriteSheetComponentData.frameTimer >= spriteSheetComponentData.frameTimerMax)
            {
                spriteSheetComponentData.frameTimer -= spriteSheetComponentData.frameTimerMax;
                spriteSheetComponentData.currentFrame = (spriteSheetComponentData.currentFrame + 1) % spriteSheetComponentData.frameCount;
            }

            float uvWidth = 1f / spriteSheetComponentData.frameCount;
            float uvHeight = 1f;
            float uvOffsetX = uvWidth * spriteSheetComponentData.currentFrame;
            float uvOffsetY = 0f;

            spriteSheetComponentData.uv = new Vector4(uvWidth, uvHeight, uvOffsetX, uvOffsetY);

            float3 position = translation.Value;
            //this to set soting layers properly, ie sprites with lower y will be in front of sprites with higher y
            position.z = position.y * 0.01f;

            spriteSheetComponentData.matrix = Matrix4x4.TRS(position, cameraRotation, Vector3.one);
            
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Camera camera = Camera.main;
        
        Job job = new Job
        {
            deltaTime = Time.DeltaTime,
            cameraRotation = camera.transform.rotation
        };

        return job.Schedule( this, inputDeps);
    }

}
