using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public struct SpriteSheetComponentData : IComponentData
{

    public int currentFrame;
    public int frameCount;
    public float frameTimer;
    public float frameTimerMax;
    public Vector4 uv;
    public Matrix4x4 matrix;
    public float3 viewportPosition;

}
