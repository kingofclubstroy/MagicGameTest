using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This handles all the crawl in a given room, any interaction with crawl will be asked here and handled
 * */

public class CrawlController : MonoBehaviour
{
    

    [SerializeField]
    GameObject crawlPrefab;

    List<Crawl> crawls;

    #region growth variables

    Dictionary<Vector2, int> CrawlLocations;

    #endregion

    // Start is called before the first frame update
    void Start()
    {


        crawls = new List<Crawl>();

        //InvokeRepeating("updateCrawlObjects", 0f, 0.2f);

    }

    void updateCrawlObjects()
    {
        foreach (Crawl crawl in crawls)
        {
            crawl.growthUpdate();
        }
    }

    private void Update()
    {
        updateCrawlObjects();
    }




    public Crawl CreateCrawl(Vector3 position, int width, int height)
    {
        GameObject crawlObject = Instantiate(crawlPrefab, new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)), Quaternion.identity);
        Crawl crawlScript = crawlObject.GetComponent<Crawl>();

        crawlScript.initialize(this, width, height);
        crawls.Add(crawlScript);

        return crawlScript;

    }
}
