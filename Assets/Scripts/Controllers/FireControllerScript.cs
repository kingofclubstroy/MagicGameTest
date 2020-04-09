using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * This handles all the crawl in a given room, any interaction with crawl will be asked here and handled
 * */

public class FireControllerScript : MonoBehaviour
{

    [SerializeField]
    int height, width, totalGrowth;

    Texture2D texture;
    SpriteRenderer spriteRenderer;

    Dictionary<Vector2, Fire> firePositions;

    Queue<Fire> fireQueue;

    bool initialized = false;

    public static FireControllerScript instance;

    [SerializeField]
    float fireLifeTime = 0.5f;

    [SerializeField]
    float queueChance = 0.5f;

    [SerializeField]
    int waterSubAmount = 20;

    [SerializeField]
    int maxFire = 1000;

    int fireGrown = 0;

    HashSet<Vector2> castingList;


    // Start is called before the first frame update
    void Start()
    {

        firePositions = new Dictionary<Vector2, Fire>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        texture = TextureHelper.MakeTexture(1000, 1000, Color.clear);


        InvokeRepeating("updateFireObjects", 0f, 0.1f);

        fireQueue = new Queue<Fire>();

        instance = this;

        totalGrowth = 0;

    }

    void updateFireObjects()
    {

        if (!initialized)
        {

            TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
            initialized = true;

        }
        else
        {

            float time = Time.time;
            while (fireQueue.Count > 0)
            {

                if (Random.Range(0f, 1f) > queueChance)
                {


                    if (fireQueue.Peek().getStartTime() + fireLifeTime < time)
                    {
                        Fire fire = fireQueue.Dequeue();

                        if (fireGrown < maxFire || Random.Range(0f, 1f) <= 0.30f)
                        {

                            SpreadToNeighbours(fire.GetPosition());

                        }

                        //spreadToNeighboursTest(fire.GetPosition(), numberDirections, diagonals);
                        firePositions.Remove(fire.GetPosition());

                        DestroyFire(fire);


                    }

                    else
                    {
                        break;
                    }
                }
                else
                {
                    fireQueue.Enqueue(fireQueue.Dequeue());
                }



            }

            if (fireQueue.Count == 0)
            {
                fireGrown = 0;
            }
        }

        texture.Apply();
        WaterControllerScript.instance.applyTexture();
    }

    private void Update()
    {
        //updateCrawlObjects();
    }

    void SpreadToNeighbours(Vector2 position)
    {

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (!(i == 0 && j == 0) && !(Mathf.Abs(j) == 1 && Mathf.Abs(i) == 1))
                {
                    AddFire(new Vector2(position.x + i, position.y + j));
                }
            }
        }

    }

    void spreadToNeighboursTest(Vector2 position, int numberOfDirections, bool diagonals)
    {

        for (int i = 0; i < numberOfDirections; i++)
        {
            AddFire(chooseRandomDirection(position, diagonals));
        }

    }


    public void AddFire(Vector2 position)
    {

        if (CrawlController.instance.GetPixel((int)position.x, (int)position.y) == Color.green && !firePositions.ContainsKey(position))
        {
            if (WaterControllerScript.instance.waterHere(position))
            {

                Debug.Log("water here");

                WaterControllerScript.instance.subtractWater(position, waterSubAmount);

            }
            else
            {

                Fire newFire = new Fire(position);
                firePositions[position] = newFire;
                fireQueue.Enqueue(newFire);
                CrawlController.instance.SetPixel((int)position.x, (int)position.y, Color.black);
                texture.SetPixel((int)position.x, (int)position.y, Color.red);
                CrawlController.instance.BurntSpaces.Add(position);
                fireGrown++;
                if(castingList.Contains(position))
                {
                    totalGrowth += 1;
                }
            }



        }
    }

    public void DestroyFire(Fire fire)
    {

        texture.SetPixel((int)fire.GetPosition().x, (int)fire.GetPosition().y, Color.clear);
        if (castingList.Contains(fire.GetPosition()))
        {
            Debug.Log("destroying fire in casting list");
            totalGrowth -= 1;
        }

    }

    Vector2 chooseRandomDirection(Vector2 position, bool diagonals)
    {

        int x = Random.Range(-1, 2);

        int y = Random.Range(-1, 2);

        if (x == 0 && y == 0 || (diagonals == false && ((Mathf.Abs(x) + Mathf.Abs(y)) == 2)))
        {
            return chooseRandomDirection(position, diagonals);
        }

        return new Vector2(position.x + x, position.y + y);

    }

    public int GetNumberPixelsInCircle(Vector2 origin, int r, bool reset)
    {
        if (reset)
        {
            (HashSet<Vector2>, int) values = HelperFunctions.MakeCircleHashSet(origin, width, height, r, texture, Color.red);

            totalGrowth = values.Item2;
            castingList = values.Item1;


        }

        return totalGrowth;

    }

    public void clearCasting()
    {
        castingList = null;
    }

    

}
