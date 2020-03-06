using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawl : MonoBehaviour
{
    public static int CrawlNumber = 0;

    public int number;

    Texture2D texture;
    SpriteRenderer spriteRenderer;

    System.Random Random;



    #region growth variables

    bool growing = true;
    bool onFire = false;
    int mostGrowthPressure = 0;
    int pixelsGrown = 0;

    Dictionary<Vector2, int> GrowthSpots;
    List<Vector2> GrowthList;

    Dictionary<Vector2, int> CrawlLocations;

    int maxGrowth = 10000;

    CrawlController crawlController;


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        number = CrawlNumber;
        CrawlNumber++;

       
        spriteRenderer = GetComponent<SpriteRenderer>();
        GrowthSpots = new Dictionary<Vector2, int>();
        GrowthList = new List<Vector2>();
        CrawlLocations = new Dictionary<Vector2, int>();

        TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));

        


    }

    public void initialize(CrawlController crawlController, int width, int height)
    {
        this.crawlController = crawlController;

        texture = TextureHelper.MakeTexture(width, height, Color.clear);

       

        Random = new System.Random(CrawlNumber);
    }



    public void growthUpdate()
    {
        if (growing && !onFire && pixelsGrown < maxGrowth)
        {

            if (GrowthList.Count == 0)
            {
                GrowthList.Add(new Vector2(texture.width / 2, texture.height / 2));

            }

            int growthToDraw = 1 + Mathf.FloorToInt(GrowthList.Count/100);

            for (int i = 0; i < growthToDraw; i++)
            {


                GrowPixel(Random.Next(0, GrowthList.Count));

            }

            pixelsGrown += growthToDraw;


            texture.Apply();

        }


    }

    void GrowPixel(int index)
    {

        Vector2 pixelPos = GrowthList[index];
        GrowthList.RemoveAt(index);

        CrawlLocations[pixelPos] = 1;

        GrowByPosition(pixelPos);


    }

    public void GrowByPosition(Vector2 pixelPos)
    {
        texture.SetPixel((int)pixelPos.x, (int)pixelPos.y, Color.green);

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {

                int x = (int)pixelPos.x + i;
                int y = (int)pixelPos.y + j;

                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height && !(x == 0 && y == 0) && texture.GetPixel(x, y) == Color.clear)
                {

                    Vector2 v = new Vector2(x, y);

                    if (!CrawlLocations.ContainsKey(v))
                    {
                        GrowthList.Add(v);
                    }

                }

            }
        }

        texture.Apply();
    }
}
