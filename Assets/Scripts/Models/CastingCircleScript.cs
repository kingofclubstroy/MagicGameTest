using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingCircleScript : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    Texture2D initialTexture;

    Texture2D texture;

    List<Vector2> pixelList;

    public float amount = 0;

    public int lastIndex = -1;

    public float updateAmount = 0;

    public int sortingOrder;

    bool textureInitialized = false;

    public bool isFirst = false;

    List<Vector2> lineList, previousLine;

    Color color;
    Color circleColor = new Color(0, 0, 0, 70f / 255f);

    public void addTempAmount(float temp)
    {
        amount = temp;
    }

    public void updateCastingCircleTexture(float percent, Color colorToChange)
    {
        color = colorToChange;
        bool pixelsChanged = false;

        if (pixelList == null)
        {
            //if (isFirst) { 
            //texture = new Texture2D(initialTexture.width, initialTexture.height);
            //texture.SetPixels(initialTexture.GetPixels());

            //} else
            //{
            texture = TextureHelper.MakeTexture(initialTexture.width, initialTexture.height, new Color(0,0,0,0.01f));
            //texture = new Texture2D(initialTexture.width, initialTexture.height);
            texture.filterMode = FilterMode.Point;
            //}
            pixelList = HelperFunctions.makePixelMap(initialTexture);
            //pixelList = new List<Vector2>();
            pixelsChanged = true;

           

            Color c = circleColor;

            if(!isFirst)
            {
                c = new Color(0, 0, 0, 0.01f);
            }

            //for(int i = 0; i < texture.width; i++)
            //{
            //    for(int j = 0; j < texture.height; j++)
            //    {
            //        texture.SetPixel(i, j, new Color(0, 0, 0));
            //    }
            //}

            

            foreach (Vector2 position in pixelList)
            {
                texture.SetPixel((int)position.x, (int)position.y, c);
            }

            texture.Apply();
            TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
            //spriteRenderer.sortingOrder = sortingOrder;
            textureInitialized = true;

        }

        int index = Mathf.FloorToInt(percent * pixelList.Count);

        if (index >= 0 && index < pixelList.Count)
        {

            if (index !=lastIndex)
            {

                pixelsChanged = true;

                if (index > lastIndex)
                {


                    for (int tempIndex = lastIndex; tempIndex <= index; tempIndex++)
                    {

                        texture.SetPixel((int)pixelList[tempIndex].x, (int)pixelList[tempIndex].y, colorToChange);

                    }

                } else
                {
                    Color c = Color.clear;
                    if(isFirst)
                    {
                        c = circleColor;
                    }
                    for (int tempIndex = index; tempIndex <= lastIndex; tempIndex++)
                    {
                        texture.SetPixel((int)pixelList[tempIndex].x, (int)pixelList[tempIndex].y, c);
                    }
                }

                lastIndex = index;


            }

        }


        if (pixelsChanged)
        {

            texture.Apply();

            if (!textureInitialized)
            {
                TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
                //spriteRenderer.sortingOrder = sortingOrder;
                textureInitialized = true;
            }

        }

    }

    /// <summary>
    /// This will be called when a charcater has stopped casting, and will slowly fade the casting circle away before destroying it
    /// </summary>
    public void initializeDestruction()
    {

        StartCoroutine(Fade());
        if(lineList != null)
        {
            StartCoroutine(RemoveLine(new List<Vector2>(lineList)));
        }

    }

    IEnumerator Fade()
    {
        int number = 0;
        for (float ft = 1f; ft >= 0; ft -= 0.01f)
        {

            loopPixels(ft, number);

            texture.Apply();
            number++;
            yield return new WaitForSeconds(.01f);
        }

        Destroy(gameObject);

    }

    /// <summary>
    /// This function is responsable for looping the pixels and fading them away, used when casting has stopped
    /// </summary>
    /// <param name="ft"> is the alpha to set the colored pixels</param>
    /// <param name="number">is a itteration number to propersly set the casting circle color's alpah</param>
    void loopPixels(float ft, int number)
    {

        Color matCol = spriteRenderer.material.color;
        matCol.a = ft;
        spriteRenderer.material.color = matCol;

        int index = lastIndex - (int)(pixelList.Count / 50);
        if (index < 0)
        {
            index = 0;
        }

        //if(!isFirst)
        //{
        //    Debug.Log("fire index = " + index);
        //    Debug.Log("fire last index = " + lastIndex);
        //}

        for (int i = 0; i < pixelList.Count; i++)
        {

            Vector2 position = pixelList[i];
            Color c = texture.GetPixel((int)position.x, (int)position.y);

            if(c == Color.clear)
            {
                continue;
            }

            if (i <= lastIndex && index <= i)
            {
                if (isFirst)
                {
                    c = circleColor;
                } else
                {
                    c = Color.clear;
                }


                //c.a = Mathf.Clamp(c.a - (Time.deltaTime / 2f), 0f, 1f);

                texture.SetPixel((int)position.x, (int)position.y, c);

            }

        }

        lastIndex = index;
    }

    public List<Vector2> getPixelList()
    {
        return pixelList;
    }

    public void UpdateSpellSelectedLine(int direction, Color color)
    {
        //if(previousLine != null)
        //{
        //    StartCoroutine(RemoveLine(previousLine));
        //}

        StartCoroutine(DrawLine(color, direction));
        //setCenter();
    }

    IEnumerator DrawLine(Color c, int direction)
    {

        lineList = MakeLine(direction);

        for (int i = 0; i < lineList.Count; i++)
        {

            if(previousLine != null)
            {
                Vector2 p = previousLine[previousLine.Count - i];
                texture.SetPixel((int)p.x, (int)p.y, new Color(0, 0,0,0.01f));
            }

           
            Vector2 pos = lineList[i];
            texture.SetPixel((int)pos.x, (int)pos.y, c);
            
            texture.Apply();
            yield return new WaitForSeconds(.01f);
        }

        previousLine = lineList;

    }

    void setCenter()
    {
        for(int i = 0; i < texture.width; i++)
        {
            for(int j = 0; j < texture.height; j++)
            {
                try
                {
                    texture.SetPixel(i, j, Color.black);
                }
                catch
                {
                    Debug.Log("error pixel");
                }
            }
        }
       
        texture.Apply();
    }

    IEnumerator RemoveLine(List<Vector2> lineList)
    {

        for (int i = lineList.Count - 1; i >= 0; i--)
        {
            Vector2 pos = lineList[i];
            texture.SetPixel((int)pos.x, (int)pos.y, Color.clear);

            texture.Apply();
            yield return new WaitForSeconds(.01f);
        }
    }

    List<Vector2> MakeLine(int direction)
    {
        List<Vector2> l = new List<Vector2>();
        int x_dir = 0, y_dir = 0;

        Debug.Log("direction = " + direction);

        switch(direction)
        {
            case 0:
                x_dir = -1;
                y_dir = 1;
                break;
            case 1:
                x_dir = 1;
                y_dir = 1;
                break;
            case 2:
                x_dir = 1;
                y_dir = -1;
                break;
            case 3:
                x_dir = -1;
                y_dir = -1;
                break;
        }

        int current_X = (texture.width) / 2, current_Y = (texture.height) / 2;
        while(texture.GetPixel(current_X, current_Y).a <= 0.1f && l.Count < 17)
        {
            l.Add(new Vector2(current_X, current_Y));
            current_X += x_dir;
            current_Y += y_dir;
        }

        Debug.Log("count = " + l.Count);

        return l;

    }

}
