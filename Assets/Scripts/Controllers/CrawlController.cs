using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This handles all the crawl in a given room, any interaction with crawl will be asked here and handled
 * */

public class CrawlController : MonoBehaviour
{

    [SerializeField]
    int height, width, maxCrawl;

    Texture2D texture;
    SpriteRenderer spriteRenderer;

    List<Crawl> crawls;

    public static CrawlController instance;

    HashSet<Vector2> castingList;

    int totalGrowth = 0;

    public HashSet<Vector2> BurntSpaces;

    [SerializeField]
    Color growthColor, burntColor, lastColor;

    #region growth variables

    Dictionary<Vector2, int> CrawlLocations;

    [SerializeField]
    float growthMultiplier = 1.5f;

    [SerializeField]
    float growthDivisor = 100f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {

        crawls = new List<Crawl>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        texture = TextureHelper.MakeTexture(1000, 1000, Color.clear);

        
        InvokeRepeating("updateCrawlObjects", 0f, 0.1f);

        instance = this;

        BurntSpaces = new HashSet<Vector2>();

        initializedCallbacks();

    }

    void updateCrawlObjects()
    {
        for (int i = crawls.Count -1; i >= 0; i--)
        {

            crawls[i].growthUpdateTest();
        }

        texture.Apply();
    }

    private void Update()
    {
        //updateCrawlObjects();
    }




    public Crawl CreateCrawl(Vector3 position)
    {
        if(crawls.Count == 0)
        {
            //texture.Apply();
            TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
        }

        Vector2 adjustedPosition = new Vector2(width / 2, height / 2);

        Crawl crawlScript = new Crawl(this, maxCrawl, adjustedPosition + (Vector2) position, growthMultiplier, growthDivisor);

        crawls.Add(crawlScript);

        return crawlScript;

    }

    public Crawl CreateCrawl(Vector3 position, int maxCrawl, float growthMultiplier)
    {
        if (crawls.Count == 0)
        {
            //texture.Apply();
            TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
        }

        Vector2 adjustedPosition = new Vector2(width / 2, height / 2);

        Crawl crawlScript = new Crawl(this, maxCrawl, adjustedPosition + (Vector2)position, growthMultiplier, growthDivisor);

        crawls.Add(crawlScript);

        return crawlScript;

    }

    public void AddFire(Vector2 position)
    {
        Vector2 adjustedPosition = new Vector2(width / 2, height / 2);
        position.x = (int)position.x;
        position.y = (int)position.y;
        FireControllerScript.instance.AddFire(adjustedPosition + position);
    }

    public Color GetPixel(int x, int y)
    {
        
        return texture.GetPixel(x, y);
    }

    public void SetPixel(int x, int y, bool last)
    {
        
        if (castingList != null && castingList.Contains(new Vector2(x, y)))
        {
            totalGrowth += 1;
        }

        if (last)
        {
            
            texture.SetPixel((int)x, (int)y, lastColor);
        }
        else
        {

            texture.SetPixel((int)x, (int)y, growthColor);
        }
    }

    public void SetOnFire(int x, int y)
    {
        if (castingList != null && castingList.Contains(new Vector2(x, y)))
        {
            totalGrowth -= 1;
            
        } 

        texture.SetPixel((int)x, (int)y, burntColor);
    }

    public bool CrawlHere(int x, int y)
    {
        return texture.GetPixel(x, y) == growthColor || texture.GetPixel(x, y) == lastColor;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }

    public void CrawlComplete(Crawl crawl)
    {
        //Debug.Log("removing crawl");
        //crawls.Remove(crawl);
    }

    public int GetNumberPixelsInCircle(Vector2 origin, int r, bool reset)
    {
        if (reset || castingList == null)
        {
            origin.x = (int)origin.x;
            origin.y = (int)origin.y;
            (HashSet<Vector2>, int, List<Vector2>) values = HelperFunctions.MakeCircleHashSet(origin, width, height, r, texture, growthColor);

            totalGrowth = values.Item2;
            castingList = values.Item1;


        }

        return totalGrowth;
    }

    public void clearCasting()
    {
        castingList = null;
    }

    public void ConsumeCrawl(Vector2 position, int amountConsumed, int pixelsPerFrame)
    {
        Debug.Log("consumeing crawl");
        List<Vector2> SurfaceAreaMap = new List<Vector2>();
        Queue<Vector2> consumeQueue = new Queue<Vector2>();
        HashSet<Vector2> triedPositions = new HashSet<Vector2>();

        //adjust position to map to crawl controller texture
        position = position + new Vector2(width / 2, height / 2);

        Debug.Log("position = " + position);
        Vector2 newpos = new Vector2((int)position.x, (int)position.y);
        Debug.Log("newpos = " + newpos);
        SurfaceAreaMap.Add(newpos);

        int completed = 0;
        
        while(completed < amountConsumed && SurfaceAreaMap.Count > 0)
        {
            
            int index = Random.Range(0, SurfaceAreaMap.Count);
            Vector2 p = SurfaceAreaMap[index];
            SurfaceAreaMap.RemoveAt(index);

            if (triedPositions.Contains(p))
            {
                continue;
            }

            triedPositions.Add(p);
            
            if(CrawlHere((int)p.x, (int)p.y))
            {
                consumeQueue.Enqueue(p);
                completed++;
            } 

            SurfaceAreaMap.Add(new Vector2(p.x + 1, p.y));
            SurfaceAreaMap.Add(new Vector2(p.x - 1, p.y));
            SurfaceAreaMap.Add(new Vector2(p.x, p.y + 1));
            SurfaceAreaMap.Add(new Vector2(p.x, p.y - 1));

        }

        StartCoroutine(consumeRoutine(consumeQueue, pixelsPerFrame));

    }

    IEnumerator consumeRoutine(Queue<Vector2> consumeQueue, int pixelsPerFrame)
    {
        Debug.Log("Consume routine");
        Debug.Log(consumeQueue.Count);
        int count = 0;
        int consumeSize = consumeQueue.Count;
        for(int i = 0; i < consumeSize; i++)
        {

            //TODO: want to remove the crawl, not set it on fire
            Vector2 p = consumeQueue.Dequeue();
            SetOnFire((int)p.x, (int)p.y);
            count++;

            if(count == pixelsPerFrame)
            {
                count = 0;
                texture.Apply();
                yield return new WaitForSeconds(0.01f);
            }
            

        }
        
    }

    #region Callback functions

    void initializedCallbacks()
    {
        WaterCreatedEvent.RegisterListener(WaterAdded);
        WaterRemovedEvent.RegisterListener(WaterRemoved);
    }

    void WaterAdded(WaterCreatedEvent e)
    {
        if(CrawlHere((int) e.waterPosition.x, (int) e.waterPosition.y))
        {
            foreach(Crawl crawl in crawls)
            {
                crawl.waterAdded(e.waterPosition);
            }
        }
    }

    void WaterRemoved(WaterRemovedEvent e)
    {
        if (CrawlHere((int)e.waterPosition.x, (int)e.waterPosition.y))
        {
            foreach (Crawl crawl in crawls)
            {
                crawl.waterRemoved(e.waterPosition);
            }
        }
    }

    #endregion

}
