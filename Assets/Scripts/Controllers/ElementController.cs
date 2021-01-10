using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementController : MonoBehaviour
{

    public enum ElementPixel
    {
        FIRE,
        Crawl,
        Water,
        WaterOverCrawl,
        BURNT

    }

    Dictionary<Vector2, ElementPixel> ElementDict;
    Texture2D texture;

    SpriteRenderer spriteRenderer;

    [SerializeField] int Height, Width;

    bool textureInitialized = false;
    bool textureChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        ElementDict = new Dictionary<Vector2, ElementPixel>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        texture = TextureHelper.MakeTexture(Width, Height, Color.clear);
    }

    public bool AddElement(Element element, Vector2 pos, Color color)
    {

        ElementPixel elementPixel;

        switch(element)
        {
            //TODO: need to set objects on fire
            case Element.FIRE:
                if(ElementDict.ContainsKey(pos) == false) {
                    return false;
                }

                elementPixel = ElementDict[pos];
                if(elementPixel == ElementPixel.Crawl)
                {
                    CrawlController.instance.SetOnFire((int) pos.x, (int) pos.y);
                    ElementDict[pos] = ElementPixel.FIRE;
                    SetPixel(pos, color);
                    return true;
                }

                return false;
                

            case Element.NATURE:

                if(ElementDict.ContainsKey(pos) == false)
                {

                    if(ObstacleController.instance.ObstacleHere(pos) == false)
                    {
                        //There is nothing here, so lets add a crawl pixel
                        ElementDict[pos] = ElementPixel.Crawl;
                        SetPixel(pos, color);
                        return true;
                    }
                  
                }

                return false;
                

        }


        return false;
    }

    public bool ElementHere(Vector2 pos, ElementPixel element)
    {
        if(ElementDict.ContainsKey(pos) == false)
        {
            return false;
        }

        return element == ElementDict[pos];
        
        
    }

    public bool WithinRange(Vector2 pos)
    {
        return (pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height && !(pos.x == 0 && pos.y == 0));
    }

    void SetPixel(Vector2 pos, Color color)
    {
        if (textureInitialized == false)
        {
            TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0f, 0f));
            textureInitialized = true;
        }

        texture.SetPixel((int)pos.x, (int)pos.y, color);

        if (textureChanged == false)
        {
            textureChanged = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (textureChanged)
        {
            texture.Apply();
            textureChanged = false;
        }
           
    }


}
