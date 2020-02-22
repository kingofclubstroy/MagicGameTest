using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingUIController : MonoBehaviour
{

    [SerializeField]
    Texture2D circleTexture;
    SpriteRenderer SpriteRenderer;

    List<Vector2> pixelList;

    Texture2D finalTexture;

    [SerializeField]
    float elementCasted = 0;
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

        if(circleTexture != null)
        {
            makePixelMap();
            
        }

    }

    // Update is called once per frame
    void Update()
    {

        elementCasted += Mathf.Clamp(chargeRate * Time.deltaTime, 0, 100);

        float percent = (elementCasted / 100f);

        int index = Mathf.FloorToInt(percent * pixelList.Count);

        finalTexture.SetPixel((int)pixelList[index].x, (int)pixelList[index].y, Color.red);

        SpriteRenderer.sprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f));


    }

    void makePixelMap()
    {
        pixelList = new List<Vector2>();

        finalTexture = new Texture2D(circleTexture.width, circleTexture.height);

        Vector2 firstPixel = findFirstPixel();

        Vector2 currentPixel = firstPixel;

        if (firstPixel == new Vector2(-1, -1))
        {
            Debug.LogError("couldn't find first pixel?!?!");
            return;
        }

        pixelList.Add(firstPixel);

        while (true)
        {
            currentPixel = findNextPixel(currentPixel);

            if(currentPixel == firstPixel)
            {
                break;
            }

            pixelList.Add(currentPixel);
        }


    }

    Vector2 findFirstPixel()
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

    Quadrent GetQuadrent(Vector2 currentPixel)
    {

        if(currentPixel.y <= circleTexture.height/2)
        {
            //In the top quadrent
            if(currentPixel.x <= circleTexture.width/2)
            {
                //Is the top left quadrent
                return Quadrent.TopLeft;
            } else
            {
                return Quadrent.TopRight;
            }
        } else
        {

            //In the bottom quadrent
            if (currentPixel.x <= circleTexture.width / 2)
            {
                //Is the bottom left quadrent
                return Quadrent.BottomLeft;
            }
            else
            {
                return Quadrent.BottomRight;
            }

        }

    }

    Vector2 findNextPixel(Vector2 currentPixel)
    {

        Quadrent quadrent = GetQuadrent(currentPixel);

        switch(quadrent)
        {
            case Quadrent.TopLeft:

                if(checkTop(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y + 1);
                }
                if (checkTopRight(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y + 1);
                }
                if (checkRight(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y);
                }
                break;

            case Quadrent.TopRight:

                if (checkBottom(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y - 1);
                }
                if (checkBottomRight(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y - 1);
                }
                if (checkRight(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y);
                }
                break;

            case Quadrent.BottomLeft:

                if (checkTop(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y + 1);
                }
                if (checkTopLeft(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y + 1);
                }
                if (checkLeft(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y);
                }
                break;

            case Quadrent.BottomRight:

                if (checkBottom(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x, currentPixel.y - 1);
                }
                if (checkBottomLeft(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y - 1);
                }
                if (checkLeft(currentPixel) == Color.black)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y);
                }
                break;



        }


        return new Vector2(-1, -1);

    }

    Color checkLeft(Vector2 currentPixel)
    {
        if(currentPixel.x - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel( (int) currentPixel.x - 1, (int) currentPixel.y);
    }

    Color checkRight(Vector2 currentPixel)
    {
        if (currentPixel.x + 1 >= circleTexture.width)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y);
    }

    Color checkTop(Vector2 currentPixel)
    {
        if (currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y + 1);
    }

    Color checkBottom(Vector2 currentPixel)
    {
        if (currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y - 1);
    }

    Color checkTopLeft(Vector2 currentPixel)
    {
        if (currentPixel.x - 1 < 0 || currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y + 1);
    }

    Color checkTopRight(Vector2 currentPixel)
    {
        if (currentPixel.x + 1 >= circleTexture.width || currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y + 1);
    }

    Color checkBottomLeft(Vector2 currentPixel)
    {
        if (currentPixel.x - 1 < 0 || currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y - 1);
    }

    Color checkBottomRight(Vector2 currentPixel)
    {
        if (currentPixel.x + 1 >= circleTexture.width || currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y - 1);
    }




}
