using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This handles all the crawl in a given room, any interaction with crawl will be asked here and handled
 * */

public class CrawlController : MonoBehaviour
{

    [SerializeField]
    int maxCrawl;

    List<Crawl> crawls;

    public static CrawlController instance;

    HashSet<Vector2> castingList;

    int totalGrowth = 0;

    public HashSet<Vector2> BurntSpaces;

    [SerializeField]
    Color growthColor, burntColor, lastColor;

    Color otherColor;

    #region growth variables

    Dictionary<Vector2, int> CrawlLocations;

    [SerializeField]
    float growthMultiplier = 1.5f;

    [SerializeField]
    float growthDivisor = 100f;

    ElementController elementController;

    #endregion

    // Start is called before the first frame update
    void Start()
    {

        crawls = new List<Crawl>();
        
        InvokeRepeating("updateCrawlObjects", 0f, 0.1f);

        instance = this;

        BurntSpaces = new HashSet<Vector2>();

        initializedCallbacks();

       
        otherColor = new Color(0.165f, 0.784f, 0.263f, 1f);

        elementController = GetComponent<ElementController>();

    }

    void updateCrawlObjects()
    {
       
        for (int i = crawls.Count -1; i >= 0; i--)
        {

            crawls[i].growthUpdate();
        }

        
    }

    private void Update()
    {
        //updateCrawlObjects();
    }




    public Crawl CreateCrawl(Vector3 position)
    {
      
        Crawl crawlScript = new Crawl(this, maxCrawl, position, growthMultiplier, growthDivisor);

        crawls.Add(crawlScript);

        return crawlScript;

    }

    public Crawl CreateCrawl(Vector3 position, int maxCrawl, float growthMultiplier)
    {
      
        Crawl crawlScript = new Crawl(this, maxCrawl, position, growthMultiplier, growthDivisor);

        crawls.Add(crawlScript);

        return crawlScript;

    }

    public void AddFire(Vector2 position)
    {
      ;
        position.x = (int)position.x;
        position.y = (int)position.y;
        FireControllerScript.instance.AddFire(position);
    }

   
    public bool SetCrawlPixel(Vector2 pos, bool last)
    {

        if (castingList != null && castingList.Contains(pos))
        {
            totalGrowth += 1;
        }

        Color color = growthColor;

        if (last)
        {
            color = lastColor;
            
        }

        return elementController.AddElement(Element.NATURE, pos, color);
        
    }

    public bool CanGrow(Vector2 pos)
    {
        return (CrawlHere(pos) == false && WithinRange(pos));
    }

    public void SetOnFire(int x, int y)
    {
        if (castingList != null && castingList.Contains(new Vector2(x, y)))
        {
            totalGrowth -= 1;
            
        } 

        //texture.SetPixel((int)x, (int)y, burntColor);
    }

    public ElementController GetElementController()
    {
        return elementController;
    }

    public bool CrawlHere(Vector2 pos)
    {
        
        return elementController.ElementHere(pos, ElementController.ElementPixel.Crawl);
    }

    public bool WithinRange(Vector2 pos)
    {
        return elementController.WithinRange(pos);
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
            (HashSet<Vector2>, int, List<Vector2>) values = HelperFunctions.MakeCircleHashSet(origin, r, this);

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
       
        List<Vector2> SurfaceAreaMap = new List<Vector2>();
        Queue<Vector2> consumeQueue = new Queue<Vector2>();
        HashSet<Vector2> triedPositions = new HashSet<Vector2>();

      
       
        SurfaceAreaMap.Add(position);

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
            
            if(CrawlHere(p))
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
                //TODO: i dont know why im doing a coroutine
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
        if(CrawlHere(e.waterPosition))
        {
            foreach(Crawl crawl in crawls)
            {
                crawl.waterAdded(e.waterPosition);
            }
        }
    }

    void WaterRemoved(WaterRemovedEvent e)
    {
        if (CrawlHere(e.waterPosition))
        {
            foreach (Crawl crawl in crawls)
            {
                crawl.waterRemoved(e.waterPosition);
            }
        }
    }

    #endregion

}
