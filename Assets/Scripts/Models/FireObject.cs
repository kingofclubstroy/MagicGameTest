using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This object handles everything fire. It is created once a tile is on fire
/// </summary>
public class FireObject
{

    private List<FireSprite> sprites = new List<FireSprite>();

    private float[] fireSpawnPlan;

    private float spriteGrowthFactor;

    private int CurrentIndex = 0;

    private Vector2 tilePosition;

    private GameObject firePrefab;

    //TODO: i don't like that this knows about the world controller... fix this somehow
    float size = WorldController.size;


    public FireObject(Vector2 transform, GameObject prefab)
    {

        this.tilePosition = transform;
        firePrefab = prefab;
        makeFirePlan();

        spawnFire(0);

    }

    /// <summary>
    /// Sets up the plan of how much fire the tile will contain and the speed at which the fire visually grows
    /// </summary>
    void makeFirePlan()
    {

        int numFires = Random.Range(3, 4);

        fireSpawnPlan = new float[numFires - 1];

        //set the speed the sprite visually grows to be maximum at 3 fires, and half the speed at 6 fires
        spriteGrowthFactor = 3f / numFires;
        
        // This is the maximum a fire must grow from the last spawn to spawn a new fire
        float maxStep = (100 / (numFires - 1));

        float lastStep = 0f;

        for(int i = 0; i < (numFires - 1); i++ )
        {

            lastStep = lastStep + Random.Range(maxStep/2, maxStep);
            fireSpawnPlan[i] = lastStep;

        }

    }

    void spawnFire(float fire)
    {

        makeFireAtPoint(RandomFirePosition(), firePrefab, fire);

       
    }

    private void makeFireAtPoint(Vector3 pos, GameObject firePrefab, float fire)
    {


        GameObject fireGo = GameObject.Instantiate(firePrefab, pos, Quaternion.identity);

        FireSprite fireSprite = fireGo.GetComponent<FireSprite>();
        fireSprite.SetupFire(fire, spriteGrowthFactor, this);
        fireSprite.position = pos;
        sprites.Add(fireSprite);

    }

    /// <summary>
    /// Finds a available position for a fire
    /// TODO: maybe add poisson disk sampling: Add 
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomFirePosition()
    { 

        float minDistance = 2f;

        //We are going to keep trying to find a spot for a fire randomly untill we find one that doesnt clash with the others
        while (true)
        {


            //TODO: reduce the constraints on position as there are more fires (because we expect them to be smaller and take up less space)
            float yPos = tilePosition.y + Random.Range(0, (size - size / 16));
            Vector3 testPosition = new Vector3(tilePosition.x + Random.Range(size / 16, size - size / 16), yPos, 0);

            bool tooClose = false;

            

            foreach (FireSprite fire in sprites)
            {
                //TODO: set minimum distance between fire on a tile, for now im chosing an abrituary value

                float distance = Mathf.Abs(Vector2.Distance(testPosition, fire.position));
                
                if (distance <= minDistance)
                {
                    //too close so i continue the while loop to try another position
                    //TODO: look into potential infinate loop if there are a lot of fires and no available spot to put another fire
                    tooClose = true;
                    break;
                }
            }

            if (tooClose == false)
            {
                //We have found a position for the new fire that isnt too close to the existing fires on the same tile

                return testPosition;
            }
            else
            {
                // Chosen position was too close to the others so lets retry and lower the minimum distance
                minDistance -= 0.01f;
            }

        }


    }


    public void FireChanged(float fire)
    {

        updateSprites(fire);
        checkToSpawn(fire);


    }


    void checkToSpawn(float fire)
    {

        if (sprites.Count <= fireSpawnPlan.Length)
        {

            if (fire >= fireSpawnPlan[sprites.Count - 1])
            {

                //Fire is more than the required to spawn a new fire, so lets do that

                spawnFire(fire);

            }

        }

    }

    void updateSprites(float fire)
    {

        foreach(FireSprite sprite in sprites)
        {

            sprite.fireChanged(fire);

        }

    }

    public void FireIsOut(FireSprite fireSprite)
    {

        sprites.Remove(fireSprite);

    }
   



}
