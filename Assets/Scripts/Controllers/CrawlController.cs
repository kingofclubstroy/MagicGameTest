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

    public void SetPixel(int x, int y, Color color)
    {
        if(castingList != null && castingList.Contains(new Vector2(x, y)))
        {
            totalGrowth += 1;
        }

        texture.SetPixel((int)x, (int)y, color);
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
        if (reset)
        {
            (HashSet<Vector2>, int) values = HelperFunctions.MakeCircleHashSet(origin, width, height, r, texture, Color.green);

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
