using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    
    public static (HashSet<Vector2>, int) MakeCircleHashSet(Vector2 origin, int width, int height, int r, Texture2D texture, Color c)
    {
        HashSet<Vector2> castingList = new HashSet<Vector2>();

        origin += new Vector2(width / 2, height / 2);

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
            Vector2 vMirr = new Vector2(v.x, -1 * v.y);



            list.Add(origin + v);
            list.Add(origin + new Vector2(v.x * -1, v.y * -1));
            list.Add(origin + vMirr);
            list.Add(origin + new Vector2(vMirr.x * -1, vMirr.y * -1));
        }



        int totalGrowth = 0;

        foreach (Vector2 location in list)
        {

            castingList.Add(location);
            if (texture.GetPixel((int)location.x, (int)location.y) == c)
            {
                totalGrowth += 1;
            }
        }

        return (castingList, totalGrowth);
    }

}
