using System.Collections;
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

    List<Vector2> consumeOrder;


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

        List<Vector2> areas = new List<Vector2>();
        areas.Add(position);

        for (int i = 0; i < amount; i++)
        {

            while (areas.Count > 0)
            {

                int ran = Random.Range(0, areas.Count - 1);
                Vector2 p = areas[ran];

                areas.RemoveAt(ran);

                if (waterPositions.ContainsKey(p))
                {

                    //waterPositions[position].addAmount(amount);

                }
                else
                {

                    Water water = new Water(amount);
                    waterPositions.Add(p, water);

                    texture.SetPixel((int)p.x, (int)p.y, Color.blue);

                    WaterCreatedEvent e = new WaterCreatedEvent();
                    e.waterPosition = p;
                    e.FireEvent();

                    areas.Add(new Vector2(p.x + 1, p.y));
                    areas.Add(new Vector2(p.x - 1, p.y));
                    areas.Add(new Vector2(p.x, p.y + 1));
                    areas.Add(new Vector2(p.x, p.y - 1));

                    break;
                }

                

            }

        }

        texture.Apply();

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

    public void ConsumeWater(Vector2 position, int amountConsumed, int pixelsPerFrame)
    {
        Queue<Vector2> consumeQueue = new Queue<Vector2>();
        

        int completed = 0;

        for(int i = 0; i < consumeOrder.Count; i++)
        {
            if(completed == amountConsumed)
            {
                break;
            }

            if(waterHere(consumeOrder[i]))
            {
                completed++;
                consumeQueue.Enqueue(consumeOrder[i]);
            }

        }

        StartCoroutine(consumeRoutine(consumeQueue, pixelsPerFrame));

    }

    IEnumerator consumeRoutine(Queue<Vector2> consumeQueue, int pixelsPerFrame)
    {
        Debug.Log("Consume routine");
        Debug.Log(consumeQueue.Count);
        int count = 0;
        int consumeSize = consumeQueue.Count;
        for (int i = 0; i < consumeSize; i++)
        {

            //TODO: want to remove the crawl, not set it on fire
            Vector2 p = consumeQueue.Dequeue();

            subtractWater(p, 100);
            
            //SetOnFire((int)p.x, (int)p.y);
            count++;

            if (count == pixelsPerFrame)
            {
                count = 0;
                texture.Apply();
                yield return new WaitForSeconds(0.01f);
            }


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

            //TODO: commented this part out so it would compile
            //(HashSet<Vector2>, int, List<Vector2>) values = HelperFunctions.MakeCircleHashSet(origin, width, height, r, texture, Color.blue);

            //totalWater = values.Item2;
            //consumeOrder = values.Item3;
            


        }

        return totalWater;
    }

}
