using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;

public struct SpriteSheetComponentData : IComponentData
{

    public int currentFrame;
    public int frameCount;
    public float frameTimer;
    public float frameTimerMax;
    public Vector4 uv;
    public Matrix4x4 matrix;

}
