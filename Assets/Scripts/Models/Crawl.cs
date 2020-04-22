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

    #endregion



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

        GrowByPositionTest(position);

        //PixelList.Add(position);

        //crawlController.SetPixel((int)position.x, (int)position.y, Color.green);

        OriginPoint = position;

        growthMultiplierTest = growthMultiplier;

        


    }



    public void growthUpdate()
    {

        if (GrowthList.Count > 0 && pixelsGrown < maxGrowth)
        {

            if(!last && pixelsGrown >= (maxGrowth * 0.95f))
            {
                last = true;
            }

            int growthToDraw = 1 + Mathf.FloorToInt(GrowthList.Count / 100);

            for (int i = 0; i < growthToDraw; i++)
            {


                GrowPixel(Random.Next(0, GrowthList.Count));

            }

            pixelsGrown += growthToDraw;

        } else
        {
            crawlController.CrawlComplete(this);
        }

    }

    void GrowPixel(int index)
    {

        Vector2 pixelPos = GrowthList[index];
        GrowthList.RemoveAt(index);
        CrawlLocations.Remove(pixelPos);

        GrowByPosition(pixelPos);

    }

    public void GrowByPosition(Vector2 pixelPos)
    {

        if (crawlController.CrawlHere((int)pixelPos.x, (int)pixelPos.y))
        {

            crawlController.SetPixel((int)pixelPos.x, (int)pixelPos.y, last);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {

                    int x = (int)pixelPos.x + i;
                    int y = (int)pixelPos.y + j;

                    if (x >= 0 && x < crawlController.GetWidth() && y >= 0 && y < crawlController.GetHeight() && !(x == 0 && y == 0) && crawlController.GetPixel(x, y) == Color.clear && (i == 0 || j == 0))
                    {

                        Vector2 v = new Vector2(x, y);

                        if (!CrawlLocations.Contains(v))
                        {
                            GrowthList.Add(v);
                            CrawlLocations.Add(v);
                        }

                    }

                }
            }
        } else
        {
            //We have encountered another growth object
            //Debug.Log("found another crawl object... do we merge?");
        }

        //texture.Apply();
    }


    public void GrowthUpdate2()
    {

        if (PixelList.Count > 0 && pixelsGrown < maxGrowth)
        {

            int growthToDraw = 1 + Mathf.FloorToInt(PixelList.Count / 100);

            for (int i = 0; i < growthToDraw; i++)
            {

                if(PixelList.Count == 0)
                {
                    return;
                }

                if(!GrowPixel2(Random.Next(0, PixelList.Count)))
                {
                    i--;
                }

            }

            pixelsGrown += growthToDraw;

        }
        else
        {
            crawlController.CrawlComplete(this);
        }


    }

    bool GrowPixel2(int index)
    {
        Vector2 pixelPos = PixelList[index];

        return GrowByPosition2(pixelPos, index);
    }

    public bool GrowByPosition2(Vector2 pixelPos, int index)
    {

        Vector2 direction = (pixelPos - OriginPoint);

        Vector2[] options = new Vector2[4];

        List<Vector2> selection = new List<Vector2>();

        if(direction.x == 0 && direction.y == 0)
        {

            selection.Add(new Vector2(1, 0));

            selection.Add(new Vector2(0, 1));

            selection.Add(new Vector2(0, -1));

            selection.Add(new Vector2(-1, 0));

            while (selection.Count > 0)
            {

                int rand = Random.Next(0, selection.Count);

                Vector2 v = selection[rand];
                

                if(IsGreen(pixelPos, v))
                {
                    Vector2 final = pixelPos + v;

                    PixelList.Add(final);
                    crawlController.SetPixel((int)final.x, (int)final.y, last);

                    return true;
                }

                selection.RemoveAt(rand);



            }

            PixelList.RemoveAt(index);
            return false;


        }

        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
           

            options[0] = new Vector2(Mathf.Clamp(direction.x, -1, 1), 0);

            if(direction.y >= 0)
            {
                options[1] = new Vector2(0, 1);

                options[2] = new Vector2(0, -1);

            } else
            {

                options[1] = new Vector2(0, -1);
                options[2] = new Vector2(0, 1);

            }

            options[3] = new Vector2(options[0].x * -1, 0);

        } else
        {



            options[0] = new Vector2(0, Mathf.Clamp(direction.y, -1, 1));

            if (direction.x >= 0)
            {
                options[1] = new Vector2(1, 0);

                options[2] = new Vector2(-1, 0);

            }
            else
            {

                options[1] = new Vector2(-1, 0);
                options[2] = new Vector2(1, 0);

            }

            options[3] = new Vector2(0, options[0].y * -1);

        }

        Debug.Log("options length = " + options.Length);

        for(int i = 0; i < options.Length; i++ )
        {

            Vector2 v = options[i];
            Debug.Log(v);

            if (IsGreen(pixelPos, v))
            {
                selection.Add(v + pixelPos);
            }
        }

       

        if(selection.Count == 0)
        {
            PixelList.RemoveAt(index);
            return false;

        } else if (selection.Count == 1)
        {

            PixelList.Add(selection[0]);

            crawlController.SetPixel((int)selection[0].x, (int)selection[0].y, last);

            return true;

        } else
        {
            int rand = Random.Next(0,100);

            int increment = (100 - firstChoiceChance) / (selection.Count - 1);

            int currentChance = firstChoiceChance;

            for(int i = 0; i < selection.Count; i++)
            {
                if(currentChance <= rand)
                {
                    PixelList.Add(selection[i]);

                    crawlController.SetPixel((int)selection[i].x, (int)selection[i].y, last);

                    return true;
                }

                currentChance += increment;
               
            }

            return false;
            
        }

    }


    bool IsGreen(Vector2 pixelPos, Vector2 direction)
    {

        if (crawlController.GetPixel((int)(pixelPos.x + direction.x), (int)(pixelPos.y + direction.y)) == Color.clear)
        {
            return true;
        }


        return false;
    }

    void setupTestVariables()
    {
        neighbourDictionary = new Dictionary<Vector2, List<Vector2>>();

    }

    public void growthUpdateTest()
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
                if(GrowPixelTest(Random.Next(0, GrowthList.Count)))
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

    bool GrowPixelTest(int index)
    {

        Vector2 pixelPos = GrowthList[index];
        GrowthList.RemoveAt(index);

        if(isBurnt(pixelPos))
        {
            neighbourDictionary.Remove(pixelPos);
            return false;
        }

        neighbourDictionary.Remove(pixelPos);


        GrowByPositionTest(pixelPos);

        return true;

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

    public void GrowByPositionTest(Vector2 pixelPos)
    {

        if (!crawlController.CrawlHere((int)pixelPos.x, (int)pixelPos.y))
        {

            crawlController.SetPixel((int)pixelPos.x, (int)pixelPos.y, last);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {

                    int x = (int)pixelPos.x + i;
                    int y = (int)pixelPos.y + j;

                    Color c = crawlController.GetPixel(x, y);

                    if (x >= 0 && x < crawlController.GetWidth() && y >= 0 && y < crawlController.GetHeight() && !(x == 0 && y == 0) &&  (c == Color.clear || c == Color.black) && (i == 0 || j == 0))
                    {

                        Vector2 v = new Vector2(x, y);

                        if (!neighbourDictionary.ContainsKey(v))
                        {
                            GrowthList.Add(v);
                            List<Vector2> neightbourList = new List<Vector2>();
                            neightbourList.Add(pixelPos);
                            neighbourDictionary[v] = neightbourList;
                        } else
                        {
                            if (neighbourDictionary[v].Count >= 2)
                            {
                                neighbourDictionary.Remove(v);
                                GrowthList.Remove(v);


                                GrowByPositionTest(v);
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

        //texture.Apply();
    }
}
