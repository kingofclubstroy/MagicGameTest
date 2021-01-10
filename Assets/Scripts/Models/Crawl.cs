using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawl
{
    public static int CrawlNumber = 0;

    public int number;

    bool last = false;

    System.Random Random;

    #region growth variables

    int pixelsGrown = 0;

    List<Vector2> GrowthList;

    HashSet<Vector2> CrawlLocations;

    int maxGrowth = 1;

    CrawlController crawlController;

    //Testing values
    List<Vector2> PixelList;

    Vector2 OriginPoint;

    int firstChoiceChance = 60;

    float growthToDrawTest = 0;

    [SerializeField]
    float growthMultiplierTest = 1;

    float growthDivisor;

    #region test variables
    Dictionary<Vector2, List<Vector2>> neighbourDictionary;

    int waterFirstRemoved = -1;

    [SerializeField]
    int waterConsumptionRate = 5;

    int frameCount = 0;

    [SerializeField]
    int waterGrowthDuration = 30;

    #endregion



    #endregion

    #region water variables

    HashSet<Vector2> waterPositions = new HashSet<Vector2>();

    #endregion


    public Crawl (CrawlController crawlController, int maxGrowth, Vector2 position, float growthMultiplier, float growthDivisor)
    {
        this.crawlController = crawlController;

        number = CrawlNumber;
        CrawlNumber++;

        GrowthList = new List<Vector2>();

        PixelList = new List<Vector2>();

        CrawlLocations = new HashSet<Vector2>();

        Random = new System.Random(CrawlNumber);

        this.maxGrowth = maxGrowth;

        //GrowByPosition(position);
        setupTestVariables();

        this.growthDivisor = growthDivisor;

        GrowByPosition(position);

        //PixelList.Add(position);

        //crawlController.SetPixel((int)position.x, (int)position.y, Color.green);

        OriginPoint = position;

        growthMultiplierTest = growthMultiplier;

        


    }



   

   

   


   
    void setupTestVariables()
    {
        neighbourDictionary = new Dictionary<Vector2, List<Vector2>>();

    }

    public void growthUpdate()
    {

        if (GrowthList.Count > 0 && pixelsGrown < maxGrowth)
        {
            if (!last && pixelsGrown >= (maxGrowth * 0.5f))
            {
                last = true;
            }

            if (pixelsGrown < 100)
            {
                growthToDrawTest += ((GrowthList.Count / growthDivisor) * (growthMultiplierTest * 15));
            }
            else
            {
                growthToDrawTest += ((GrowthList.Count / growthDivisor) * growthMultiplierTest);
            }

            while(growthToDrawTest >= 1 && GrowthList.Count > 0)
            {
                if(GrowPixel(Random.Next(0, GrowthList.Count)))
                {
                    growthToDrawTest--;
                    pixelsGrown++;
                }
            }

        }
        else
        {
            crawlController.CrawlComplete(this);
        }

    }

    bool GrowPixel(int index)
    {

        Vector2 pixelPos = GrowthList[index];
        GrowthList.RemoveAt(index);

        if(isBurnt(pixelPos))
        {
            neighbourDictionary.Remove(pixelPos);
            return false;
        }

        neighbourDictionary.Remove(pixelPos);


        return GrowByPosition(pixelPos);

    }

    bool isBurnt(Vector2 pixelPos)
    {

        foreach (Vector2 v in neighbourDictionary[pixelPos])
        {
            if(!CrawlController.instance.BurntSpaces.Contains(v))
            {
                return false;
            }
         
        }

        return true;

    }

    public bool GrowByPosition(Vector2 pixelPos)
    {
       
        if(crawlController.SetCrawlPixel(pixelPos, last)) { 

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {

                    int x = (int)pixelPos.x + i;
                    int y = (int)pixelPos.y + j;

                    Vector2 v = new Vector2(x, y);

                    if ( (i == 0 || j == 0) && crawlController.CanGrow(v))
                    {
                       

                        if (!neighbourDictionary.ContainsKey(v))
                        {
                            GrowthList.Add(v);
                            List<Vector2> neightbourList = new List<Vector2>();
                            neightbourList.Add(pixelPos);
                            neighbourDictionary[v] = neightbourList;
                        } else
                        {

                            //TODO: this may beed to change
                            if (neighbourDictionary[v].Count >= 2)
                            {
                                neighbourDictionary.Remove(v);
                                GrowthList.Remove(v);


                                GrowByPosition(v);
                            }
                            else
                            {
                                neighbourDictionary[v].Add(pixelPos);

                            }
                        }

                    }

                }
            }
        }
        else
        {
            //We have encountered another growth object
            //Debug.Log("found another crawl object... do we merge?");
        }

        return true;

        //texture.Apply();
    }

    public void waterAdded(Vector2 position)
    {
        if (CrawlLocations.Contains(position) == false)
        {
            return;
        }

        waterPositions.Add(position);
    }

    public void waterRemoved(Vector2 position)
    {
        if (CrawlLocations.Contains(position) == false)
        {
            return;
        }

        waterPositions.Remove(position);
    }

    public void RemoveWater(Vector2 position)
    {
        waterPositions.Remove(position);
        growthMultiplierTest += 1;
        

        RemoveWaterEvent e = new RemoveWaterEvent();
        e.position = position;
        e.FireEvent();
    }

    void WaterUpdate()
    {
        frameCount++;

        if (frameCount % waterConsumptionRate == 0)
        {
            if (waterPositions.Count == 0)
            {
                if (growthMultiplierTest > 1 && (frameCount - waterFirstRemoved) > waterGrowthDuration)
                {
                    growthMultiplierTest--;
                }
                else
                {

                    waterFirstRemoved = -1;
                    frameCount = 0;
                }
            }
            else
            {
                foreach (Vector2 pos in waterPositions)
                {
                    waterPositions.Remove(pos);
                    growthMultiplierTest += 1;

                    if (waterFirstRemoved == -1)
                    {
                        waterFirstRemoved = frameCount;
                    }

                    break;
                }

            }

        }
    }

}
