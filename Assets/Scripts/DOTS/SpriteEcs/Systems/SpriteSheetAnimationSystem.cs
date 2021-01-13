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
        public float3 viewPortPosition;
        public Matrix4x4 MVP;
        public float3 scaling;
        
        public void Execute(ref SpriteSheetComponentData spriteSheetComponentData, ref Translation translation)
        {
            //spriteSheetComponentData.frameTimer += deltaTime;
            //while (spriteSheetComponentData.frameTimer >= spriteSheetComponentData.frameTimerMax)
            //{
            //    spriteSheetComponentData.frameTimer -= spriteSheetComponentData.frameTimerMax;
            //    spriteSheetComponentData.currentFrame = (spriteSheetComponentData.currentFrame + 1) % spriteSheetComponentData.frameCount;
            //}

            float uvWidth = 1f / spriteSheetComponentData.frameCount;
            float uvHeight = 1f;
            float uvOffsetX = uvWidth * spriteSheetComponentData.currentFrame;
            float uvOffsetY = 0f;

            spriteSheetComponentData.uv = new Vector4(uvWidth, uvHeight, uvOffsetX, uvOffsetY);

            //TODO: may want to only update matrix if the camera sees the object
            float3 position = translation.Value;
            //this to set soting layers properly, ie sprites with lower y will be in front of sprites with higher y

           

            //position.z = ((position.y * Mathf.Cos(zRotation)) + (position.x * Mathf.Sin(zRotation))) * 0.00001f;
            

           // viewPortPosition = camera.WorldToViewportPoint(position);

            Vector3 screenPos = MVP.MultiplyPoint(position);
            viewPortPosition = new Vector3(screenPos.x + 1f, screenPos.y + 1f, screenPos.z + 1f) * 0.5f;

            position.z = viewPortPosition.y * 0.01f;

            spriteSheetComponentData.viewportPosition = viewPortPosition;



            spriteSheetComponentData.matrix = Matrix4x4.TRS(position, cameraRotation, scaling);
            
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Camera camera = Camera.main;

        Quaternion cameraRotation = camera.transform.rotation;

        // Same as above
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        Matrix4x4 P = Camera.main.projectionMatrix;
        Matrix4x4 MVP = P * V; // Skipping M, point in world coordinates

        float3 scaling = new float3(28, 28, 1);
       

        Job job = new Job
        {
            deltaTime = Time.DeltaTime,
            cameraRotation = cameraRotation,
            MVP = MVP,
            scaling = scaling
            //zRotation = Mathf.Deg2Rad * cameraRotation.eulerAngles.z
    };

        return job.Schedule( this, inputDeps);
    }

}
