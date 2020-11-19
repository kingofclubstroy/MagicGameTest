﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControllerScript : MonoBehaviour
{
    [SerializeField]
    int height, width;

    Texture2D texture;
    SpriteRenderer spriteRenderer;

    Dictionary<Vector2, Water> waterPositions;

    Queue<Fire> fireQueue;

    bool initialized = false;

    public static WaterControllerScript instance;

    int totalWater = 0;


    // Start is called before the first frame update
    void Start()
    {

        waterPositions = new Dictionary<Vector2, Water>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        texture = TextureHelper.MakeTexture(1000, 1000, Color.clear);

        fireQueue = new Queue<Fire>();

        instance = this;

    }


    public void AddWater(Vector2 pos, int amount)
    {

        if(!initialized)
        {
            Debug.Log("initializing");
            TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
            initialized = true;
        }

        Vector2 position = new Vector2(width / 2, height / 2);
        pos.x = (int)pos.x;
        pos.y = (int)pos.y;

        position = position + pos;

        if (waterPositions.ContainsKey(position))
        {

            waterPositions[position].addAmount(amount);

        } else
        {

            Water water = new Water(amount);
            waterPositions.Add(position, water);

            texture.SetPixel((int)position.x, (int)position.y, Color.blue);

            WaterCreatedEvent e = new WaterCreatedEvent();
            e.waterPosition = position;
            e.FireEvent();
        }

        //texture.Apply();

    }

    public bool waterHere(Vector2 position)
    {
        return (waterPositions.ContainsKey(position));
    }

    public void subtractWater(Vector2 position, int subtractAmount)
    {
        if (!waterPositions[position].subtractWater(subtractAmount))
        {
            waterPositions.Remove(position);
            texture.SetPixel((int)position.x, (int)position.y, Color.clear);
            WaterRemovedEvent e = new WaterRemovedEvent();
            e.waterPosition = position;
            e.FireEvent();
        }

    }

    public void applyTexture()
    {
        texture.Apply();
    }

    public int GetNumberPixelsInCircle(Vector2 origin, int r, bool reset)
    {
        if (reset)
        {
            origin.x = (int)origin.x;
            origin.y = (int)origin.y;
            (HashSet<Vector2>, int) values = HelperFunctions.MakeCircleHashSet(origin, width, height, r, texture, Color.blue);

            totalWater = values.Item2;
            


        }

        return totalWater;
    }

}
