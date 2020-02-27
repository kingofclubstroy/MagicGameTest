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
    float decayRate = 10f;

    [SerializeField]
    float maxElement;

    [SerializeField]
    float maxCastRate;

    enum Quadrent
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [SerializeField]
    GameObject spellIconPrefab;

    List<CastingElements> elementList = new List<CastingElements>();

    List<Spell_Icon_Script> spellIcons = new List<Spell_Icon_Script>();

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

        //TODO: remove delta time dependence, may want to do a 30-60 fps limit, and may want to go with integers and not floats
        if (Input.GetKey(KeyCode.Space))
        {

            elementDifference = Time.deltaTime;

            GetSurroundingElements();

        }
        else
        {
            elementDifference = Time.deltaTime * -1;

        }

        if (elementDifference != 0)
        {

            bool allZero = true;
            int i = 0;
            foreach (CastingElements element in elementList)
            {

                List<Vector2> circleList = pixelList[i];

                float tempElementDifference = 0;

                if (elementDifference < 0)
                {
                    tempElementDifference = decayRate * elementDifference;
                }
                else
                {
                    //TODO: may want to change these numbers, but they will work for now
                    //Setting the amount casted to be related to the ratio amount of element present and the theiritical max amount that could be,
                    // multiplied by the max cast speed im temporarly setting as 50 (maybe there are augments that improve this)
                    tempElementDifference = ((element.updateAmount/900f) * 50f) * elementDifference;

                }

                float elementCasted = Mathf.Clamp(element.amount + tempElementDifference, 0, 100);

               

                if(elementCasted != 0)
                {
                    allZero = false;
                }

                if(elementCasted > element.updateAmount && elementDifference > 0)
                {
                    //The amount of the element casted is more than the amount of element surrounding the player, so lets set tot he amount around
                    elementCasted = element.updateAmount;
                }

                foreach (Spell_Icon_Script icon in spellIcons)
                {
                    if(element.element == icon.spell.element)
                    {
                        icon.setElementCharge(elementCasted);
                    }
                }

                element.amount = elementCasted;

                float percent = (elementCasted / 100f);

                int index = Mathf.FloorToInt(percent * circleList.Count);

                if (index >= 0 && index < circleList.Count)
                {

                    if (index != element.lastIndex)
                    {

                        pixelsChanged = true;

                        Color colorToChange = getColor(element.element);

                        if (index < element.lastIndex)
                        {

                            for (int tempIndex = index; tempIndex <= element.lastIndex; tempIndex++)
                            {

                                finalTexture.SetPixel((int)circleList[tempIndex].x, (int)circleList[tempIndex].y, Color.clear);

                            }

                        }
                        else
                        {

                            for (int tempIndex = element.lastIndex; tempIndex <= index; tempIndex++)
                            {

                                finalTexture.SetPixel((int)circleList[tempIndex].x, (int)circleList[tempIndex].y, colorToChange);

                            }
                        }

                        element.lastIndex = index;


                    }



                }

                element.Reset();

                i++;

            }

            if (pixelsChanged)
            {

                finalTexture.Apply();

                SpriteRenderer.sprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f), 1);

                SpriteRenderer.material.mainTexture = finalTexture as Texture;
                SpriteRenderer.material.shader = Shader.Find("Sprites/Default");

            }

            if(allZero)
            {

                foreach(Spell_Icon_Script icon in spellIcons)
                {
                    icon.destroy();
                }
                //all of the casting elements have decayed to 0, so lets clear the list
                spellIcons.Clear();
                elementList.Clear();
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


    Color getColor(CastingElements.Element element)
    {

        switch(element)
        {
            case CastingElements.Element.FIRE:
                return Color.red;

            case CastingElements.Element.NATURE:
              
                return Color.green;

            case CastingElements.Element.WATER:
                return Color.blue;

            case CastingElements.Element.EARTH:
                //brown
                return new Color(204f/255f, 148f/255f, 115f/255f);

            case CastingElements.Element.WIND:
                return Color.white;
        }

        Debug.Log("default color");

        return Color.white;

    }


    void GetSurroundingElements()
    {

        List<TileScript> neighbouringTiles = WorldController.instance.findNeighbours(WorldController.instance.GetTilePositionFromWorld(this.transform.parent.position), true);

        bool startOfCast = false;

        //Check to see if we have an empty element list, so we know we will have to sort the elements based on casting speed
        if (elementList.Count == 0)
        {
            
            startOfCast = true;
        }

        foreach (TileScript tile in neighbouringTiles)
        {

            if (tile != null)
            {

                if (tile.fire > 0)
                {
                    CastingElements ele = null;

                    foreach (CastingElements e in elementList)
                    {
                        if (e.element == CastingElements.Element.FIRE)
                        {
                            ele = e;
                            break;
                        }
                    }

                    if (ele != null)
                    {
                        ele.addTempAmount(tile.fire);
                    }
                    else
                    {
                        CastingElements c = new CastingElements(CastingElements.Element.FIRE);
                        c.addTempAmount(tile.fire);
                        elementList.Add(c);
                        makeSpellIcon(CastingElements.Element.FIRE);

                        
                    }
                }

                if (tile.fuel > 0)
                {
                    CastingElements ele = null;

                    foreach (CastingElements e in elementList)
                    {
                        if (e.element == CastingElements.Element.NATURE)
                        {
                            ele = e;
                            break;
                        }
                    }

                    if (ele != null)
                    {
                        ele.addTempAmount(tile.fuel);
                    }
                    else
                    {

                        CastingElements c = new CastingElements(CastingElements.Element.NATURE);
                        c.addTempAmount(tile.fuel);
                        elementList.Add(c);
                        makeSpellIcon(CastingElements.Element.NATURE);
                    }
                }
            }

            //TODO: add looking for other elements

        }

        if(startOfCast && elementList.Count > 1)
        {
           
            //We didnt have any elements in the list before we searched and now we have multiple that we need to sort, so lets sort the list
            elementList.Sort((p1, p2) => p1.updateAmount.CompareTo(p2.updateAmount));
            
        }

        if(elementList.Count == 0)
        {
            Debug.Log("no elements around");
        }
    }

    void makeSpellIcon(CastingElements.Element element)
    {

        //TODO: i am assuming an icon has a radius of 4

        Vector3 position;
        float distance = finalTexture.width/2;

        switch (spellIcons.Count) {

            case 0:
                position = new Vector3(this.transform.position.x + (-1 * distance), this.transform.position.y + distance);
                break;

            case 1:
                position = new Vector3(this.transform.position.x + distance, this.transform.position.y + distance);
                break;

            case 2:
                position = new Vector3(this.transform.position.x + distance, this.transform.position.y + (-1 * distance));
                break;

            case 3:
                position = new Vector3(this.transform.position.x + (-1 * distance), this.transform.position.y + (-1 * distance));
                break;

            default:
                position = new Vector3(this.transform.position.x + (-1 * distance), this.transform.position.y + distance);
                break;


        }

        GameObject obj = Instantiate(spellIconPrefab, this.transform);

        obj.transform.position = position;

        //GameObject obj = Instantiate(spellIconPrefab, position, Quaternion.identity);

        Spell_Icon_Script icon = obj.GetComponent<Spell_Icon_Script>();

        float[] overChargeList = new float[2];
        overChargeList[0] = 20;
        overChargeList[1] = 20;

        icon.Initialize(new TempSpell(element, 40, overChargeList), getColor(element));

        spellIcons.Add(icon);



    }

}
