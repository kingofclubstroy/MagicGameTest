using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingUIController : MonoBehaviour
{
    [SerializeField]
    Sprite sprite;

    [SerializeField]
    Texture2D[] circleTextureList = new Texture2D[4];
    

    SpriteRenderer SpriteRenderer;

    List<List<Vector2>> pixelList;

    Texture2D finalTexture;

    [SerializeField]
    int[] lastIndexes = new int[4];

    [SerializeField]
    float[] elementCharge = new float[4];

    
    [SerializeField]
    float chargeRate = 10;

    enum Quadrent
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    // Start is called before the first frame update
    void Start()
    {

        SpriteRenderer = GetComponent<SpriteRenderer>();

        Texture2D spriteTexture = sprite.texture;

        pixelList = new List<List<Vector2>>();


        foreach(Texture2D texture in circleTextureList)
        {
            makePixelMap(texture);
        }

    }

    // Update is called once per frame
    void Update()
    {

        float elementDifference = 0;

        bool pixelsChanged = false;

        if (Input.GetKey(KeyCode.Space))
        {

            elementDifference = chargeRate * Time.deltaTime;

        }
        else
        {
            elementDifference = (chargeRate * Time.deltaTime * -1);

        }

        if (elementDifference != 0)
        {

            for (int i = 0; i < pixelList.Count; i++)
            {

                List<Vector2> circleList = pixelList[i];

                float elementCasted = Mathf.Clamp(elementCharge[i] + elementDifference, 0, 100);

                int lastIndex = lastIndexes[i];

                float percent = (elementCasted / 100f);

                int index = Mathf.FloorToInt(percent * circleList.Count) - 1;

                if (index >= 0 && index < circleList.Count)
                {

                    if (index != lastIndex)
                    {

                        pixelsChanged = true;

                        Color colorToChange = getColor(i);

                        if (index < lastIndex)
                        {

                            for (int tempIndex = index; tempIndex <= lastIndex; tempIndex++)
                            {

                                finalTexture.SetPixel((int)circleList[tempIndex].x, (int)circleList[tempIndex].y, Color.clear);

                            }

                        }
                        else
                        {

                            for (int tempIndex = lastIndex; tempIndex <= index; tempIndex++)
                            {

                                finalTexture.SetPixel((int)circleList[tempIndex].x, (int)circleList[tempIndex].y, colorToChange);

                            }
                        }

                        lastIndex = index;

                    }



                }

            }

            if (pixelsChanged)
            {

                finalTexture.Apply();

                SpriteRenderer.sprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f), 1);

                SpriteRenderer.material.mainTexture = finalTexture as Texture;
                SpriteRenderer.material.shader = Shader.Find("Sprites/Default");

            }

        }


    }

    void makePixelMap(Texture2D circleTexture)
    {
        List<Vector2> circleList = new List<Vector2>();
        
        finalTexture = new Texture2D(circleTexture.width, circleTexture.height, TextureFormat.ARGB32, false);

        finalTexture.filterMode = FilterMode.Point;

        for (int y = 0; y < finalTexture.height; y++)
        {
            for (int x = 0; x < finalTexture.width; x++)
            {
                finalTexture.SetPixel(x, y, Color.clear);
            }
        }

        Vector2 firstPixel = findFirstPixel(circleTexture);

        Debug.Log(firstPixel);

        Vector2 currentPixel = firstPixel;

        if (firstPixel == new Vector2(-1, -1))
        {
            Debug.LogError("couldn't find first pixel?!?!");
            return;
        }

        circleList.Add(firstPixel);

        

        while(true)
        {
            currentPixel = findNextPixel(currentPixel, circleTexture);

            Debug.Log(currentPixel);

            if (currentPixel == firstPixel)
            {
                pixelList.Add(circleList);
                return;
            }

            circleList.Add(currentPixel);
        }


    }

    Vector2 findFirstPixel(Texture2D circleTexture)
    {

        for(int i = 0; i < circleTexture.width; i++)
        {

            if(circleTexture.GetPixel(i, circleTexture.height/2) == Color.black)
            {
                return new Vector2(i, circleTexture.height / 2);
            }

        }

        return new Vector2(-1, -1);

    }

    Quadrent GetQuadrent(Vector2 currentPixel, Texture2D circleTexture)
    {

        if(currentPixel.y >= circleTexture.height/2)
        {
            //In the top quadrent
            if(currentPixel.x <= circleTexture.width/2)
            {
                //Is the top left quadrent
                Debug.Log("top left quadrent");
                return Quadrent.TopLeft;
            } else
            {
                Debug.Log("top right quadrent");
                return Quadrent.TopRight;
            }
        } else
        {

            //In the bottom quadrent
            if (currentPixel.x <= circleTexture.width / 2)
            {
                Debug.Log("bottom left quadrent");
                //Is the bottom left quadrent
                return Quadrent.BottomLeft;
            }
            else
            {
                Debug.Log("bottom right quadrent");
                return Quadrent.BottomRight;
            }

        }

    }

    Vector2 findNextPixel(Vector2 currentPixel, Texture2D circleTexture)
    {

        Quadrent quadrent = GetQuadrent(currentPixel, circleTexture);

        switch(quadrent)
        {
            case Quadrent.TopLeft:

                if(checkTop(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y + 1);
                }
                if (checkTopRight(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y + 1);
                }
                if (checkRight(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y);
                }
                break;

            case Quadrent.TopRight:

                if (checkBottom(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y - 1);
                }
                if (checkBottomRight(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y - 1);
                }
                if (checkRight(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y);
                }
                break;

            case Quadrent.BottomLeft:

                if (checkTop(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y + 1);
                }
                if (checkTopLeft(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y + 1);
                }
                if (checkLeft(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y);
                }
                break;

            case Quadrent.BottomRight:

                if (checkBottom(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y - 1);
                }
                if (checkBottomLeft(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y - 1);
                }
                if (checkLeft(currentPixel, circleTexture) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y);
                }
                break;



        }


        return new Vector2(-1, -1);

    }

    Color checkLeft(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel( (int) currentPixel.x - 1, (int) currentPixel.y);
    }

    Color checkRight(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x + 1 >= circleTexture.width)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y);
    }

    Color checkTop(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y + 1);
    }

    Color checkBottom(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y - 1);
    }

    Color checkTopLeft(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x - 1 < 0 || currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y + 1);
    }

    Color checkTopRight(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x + 1 >= circleTexture.width || currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y + 1);
    }

    Color checkBottomLeft(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x - 1 < 0 || currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y - 1);
    }

    Color checkBottomRight(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x + 1 >= circleTexture.width || currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y - 1);
    }


    Color getColor(int index)
    {

        switch(index)
        {
            case 0:
                return Color.red;

            case 1:
                return Color.green;

            case 2:
                return Color.blue;

            case 3:
                //brown
                return new Color(204f, 148f, 115f);
        }

        return Color.white;

    }

}
