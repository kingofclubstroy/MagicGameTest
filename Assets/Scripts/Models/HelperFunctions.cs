using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    public static Color CastingColor;

    enum Quadrent
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public static (HashSet<Vector2>, int) MakeCircleHashSet(Vector2 startOrigin, int width, int height, int r, Texture2D texture, Color c)
    {
        HashSet<Vector2> castingList = new HashSet<Vector2>();

        Vector2 origin = new Vector2((int)startOrigin.x, (int)startOrigin.y);

        origin += new Vector2(width / 2, height / 2);

        origin.x = (int)origin.x;
        origin.y = (int)origin.y;

        List<Vector2> tmpList = new List<Vector2>();
        List<Vector2> list = new List<Vector2>();
        float rSquared = r * r; // using sqared reduces execution time (no square root needed)
        for (int x = 1; x <= r; x++)
            for (int y = 0; y <= r; y++)
            {
                Vector2 v = new Vector2(x, y);
                if (Vector2.SqrMagnitude(v) <= rSquared)
                    tmpList.Add(v);
                else
                    break;
            }

        list.Add(origin);
        foreach (Vector2 v in tmpList)
        {
            Vector2 vMirr = new Vector2((int)v.x, (int) -1 * v.y);
            list.Add(origin + v);
            list.Add(origin + new Vector2((int)v.x * -1, (int) v.y * -1));
            list.Add(origin + vMirr);
            list.Add(origin + new Vector2((int)vMirr.x * -1, (int) vMirr.y * -1));
        }



        int totalGrowth = 0;

        foreach (Vector2 location in list)
        {

            //Debug.Log(location);

            if (!castingList.Contains(location))
            {
                castingList.Add(location);
                if (texture.GetPixel((int)location.x, (int)location.y) == c)
                {
                    totalGrowth += 1;
                }
            }
        }

        return (castingList, totalGrowth);
    }

    public static List<Vector2> makePixelMap(Texture2D circleTexture)
    {

        List<Vector2> circleList = new List<Vector2>();

        Vector2 firstPixel = findFirstPixel(circleTexture);

        Vector2 currentPixel = firstPixel;

        if (firstPixel == new Vector2(-1, -1))
        {
            Debug.LogError("couldn't find first pixel?!?!");
            return null;
        }

        circleList.Add(firstPixel);

        while (true)
        {
            currentPixel = findNextPixel(currentPixel, circleTexture);

            if (currentPixel == firstPixel)
            {
                return circleList;
            }

            circleList.Add(currentPixel);
        }

    }

    static Vector2 findNextPixel(Vector2 currentPixel, Texture2D circleTexture)
    {
        Quadrent quadrent = GetQuadrent(currentPixel, circleTexture);

        switch (quadrent)
        {
            case Quadrent.TopLeft:

                if (checkTop(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x, currentPixel.y + 1);
                }
                if (checkTopRight(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y + 1);
                }
                if (checkRight(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y);
                }
                break;

            case Quadrent.TopRight:

                if (checkBottom(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x, currentPixel.y - 1);
                }
                if (checkBottomRight(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y - 1);
                }
                if (checkRight(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x + 1, currentPixel.y);
                }
                break;

            case Quadrent.BottomLeft:

                if (checkTop(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x, currentPixel.y + 1);
                }
                if (checkTopLeft(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y + 1);
                }
                if (checkLeft(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y);
                }
                break;

            case Quadrent.BottomRight:

                if (checkBottom(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x, currentPixel.y - 1);
                }
                if (checkBottomLeft(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y - 1);
                }
                if (checkLeft(currentPixel, circleTexture) != Color.clear)
                {
                    return new Vector2(currentPixel.x - 1, currentPixel.y);
                }
                break;



        }


        return new Vector2(-1, -1);

    }

    static Vector2 findFirstPixel(Texture2D circleTexture)
    {

        for (int i = 0; i < circleTexture.width; i++)
        {

            if (circleTexture.GetPixel(i, circleTexture.height / 2) != Color.clear)
            {
                CastingColor = circleTexture.GetPixel(i, circleTexture.height / 2);
                return new Vector2(i, circleTexture.height / 2);
            }

        }

        return new Vector2(-1, -1);

    }

    static Quadrent GetQuadrent(Vector2 currentPixel, Texture2D circleTexture)
    {

        if (currentPixel.y >= circleTexture.height / 2)
        {
            //In the top quadrent
            if (currentPixel.x <= circleTexture.width / 2)
            {
                //Is the top left quadrent

                return Quadrent.TopLeft;
            }
            else
            {

                return Quadrent.TopRight;
            }
        }
        else
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

    static Color checkLeft(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y);
    }

    static Color checkRight(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x + 1 >= circleTexture.width)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y);
    }

    static Color checkTop(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y + 1);
    }

    static Color checkBottom(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y - 1);
    }

    static Color checkTopLeft(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x - 1 < 0 || currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y + 1);
    }

    static Color checkTopRight(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x + 1 >= circleTexture.width || currentPixel.y + 1 >= circleTexture.height)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y + 1);
    }

    static Color checkBottomLeft(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x - 1 < 0 || currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x - 1, (int)currentPixel.y - 1);
    }

    static Color checkBottomRight(Vector2 currentPixel, Texture2D circleTexture)
    {
        if (currentPixel.x + 1 >= circleTexture.width || currentPixel.y - 1 < 0)
        {
            return Color.clear;
        }

        return circleTexture.GetPixel((int)currentPixel.x + 1, (int)currentPixel.y - 1);
    }

}
