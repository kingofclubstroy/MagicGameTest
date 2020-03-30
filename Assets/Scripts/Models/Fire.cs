using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire
{

    Vector2 position;
    int elementAmount = 0;
    float startTime;
    int lifeTime = 100;

    int timesSpread = 0;

    FireControllerScript controller;

    bool destoryed = false;
    
    public Fire(Vector2 position)
    {
        this.position = position;
        startTime = Time.time;
        
    }

    public float getStartTime()
    {
        return startTime;
    }

    public Vector2 GetPosition()
    {
        return position;
    }

}
