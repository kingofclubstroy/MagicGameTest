using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Icon_Script : MonoBehaviour
{

    public Spell spell;

    SpriteRenderer spriteRenderer;

    SpriteHandler spriteHandler;

    Color alphaColor;
    public Color elementColor;

    Texture2D texture;

    int lastIndex = -1;

    int lastIconIndex = -1;

    float currentElementCharge = 0;

    List<Vector2> pixelList;

    Color circleColor = new Color(0, 0, 0, 70f / 255f);

    bool textureInitialized = false;

    List<List<Vector2>> iconMap;

    [SerializeField]
    Animator animator;

    public int direction;

    bool _selected = false;
    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            animator.SetBool("IsSelected", value);
        }
    }

    bool _charged = false;
    public bool Charged
    {
        get
        {
            return _charged;
        }
        set
        {
            _charged = value;
            animator.SetBool("IsCharged", value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        spriteHandler = FindObjectOfType<SpriteHandler>();
        
    }

    public void Initialize(Spell spell, Color elementColor)
    {

        //this.spell = spell;
        this.elementColor = elementColor;
        spriteRenderer = GetComponent<SpriteRenderer>();

        this.spell = spell;

        //colorTexture(elementColor);

        //initializeOverchargePixels();




    }

    public void updateSpellIcon(float charge)
    {

        bool pixelsChanged = false;

        if (pixelList == null)
        {

            if (spell == null)
            {
                Debug.Log("spell is null");
            } else if (spell.spellType == null)
            {
                Debug.Log("no spell type");
            } else if (spriteHandler == null)
            {
                Debug.Log("SpriteHandler is null");
            }

            if(spriteHandler == null)
            {
                spriteHandler = FindObjectOfType<SpriteHandler>();
            }

            Texture2D tex = spriteHandler.GetTexture(spell.spellType);

            

            texture = new Texture2D(tex.width, tex.height);
            texture.SetPixels(tex.GetPixels());
            texture.filterMode = FilterMode.Point;
            pixelList = HelperFunctions.makePixelMap(tex);
            iconMap = HelperFunctions.makeSpellIconMap(tex);
            pixelsChanged = true;
            GetComponent<BoxCollider2D>().size = new Vector2(tex.width, tex.height);

        }

        float percent = Mathf.Clamp(charge / spell.spellParams.elementCost , 0, 1);
        int index = Mathf.FloorToInt(percent * pixelList.Count - 1);

        int iconIndex = (iconMap.Count - 1) - (pixelList.Count - 1 - index);

        pixelsChanged = updateCirclePixels(index, pixelsChanged);
        pixelsChanged = updateIconPixels(iconIndex, pixelsChanged);


        if (pixelsChanged)
        {

            texture.Apply();

            if (!textureInitialized)
            {
                TextureHelper.initializeTexture(texture, spriteRenderer, new Vector2(0.5f, 0.5f));
                textureInitialized = true;
            }

        }

    }

    public bool updateCirclePixels(int index, bool pixelsChanged)
    {
        if (index >= 0 && index < pixelList.Count)
        {

            if (index != lastIndex)
            {

                if (index == pixelList.Count - 1)
                {
                    Charged = true;
                }
                else if (Charged)
                {
                    Charged = false;
                }

                pixelsChanged = true;

                if (index > lastIndex)
                {


                    for (int tempIndex = lastIndex + 1; tempIndex <= index; tempIndex++)
                    {

                        texture.SetPixel((int)pixelList[tempIndex].x, (int)pixelList[tempIndex].y, elementColor);

                    }

                }
                else
                {
                    for (int tempIndex = index; tempIndex <= lastIndex; tempIndex++)
                    {
                        texture.SetPixel((int)pixelList[tempIndex].x, (int)pixelList[tempIndex].y, circleColor);
                    }
                }

                lastIndex = index;


            }

        }

        return pixelsChanged;
    }

    public bool updateIconPixels(int index, bool pixelsChanged)
    {
        if (index >= 0 && index < iconMap.Count)
        {

            if (index != lastIconIndex)
            {

                pixelsChanged = true;

                if (index > lastIconIndex)
                {


                    for (int tempIndex = lastIconIndex; tempIndex <= index; tempIndex++)
                    {
                        if(tempIndex == -1)
                        {
                            continue;
                        }

                        foreach (Vector2 position in iconMap[tempIndex])
                        {

                            texture.SetPixel((int) position.x, (int)position.y, elementColor);
                        }

                    }

                }
                else
                {
                    for (int tempIndex = index; tempIndex <= lastIconIndex; tempIndex++)
                    {
                        if (tempIndex == -1)
                        {
                            continue;
                        }
                        try
                        {
                            foreach (Vector2 position in iconMap[tempIndex])
                            {

                                texture.SetPixel((int)position.x, (int)position.y, circleColor);
                            }
                        } catch
                        {
                            Debug.LogError("failed, temp index is " + tempIndex + " and size is " + iconMap.Count);
                        }
                        
                    }
                }

                lastIconIndex = index;


            }

        } else if(lastIconIndex >= 0)
        {
            for(int i = lastIconIndex; i >= 0; i--)
            {
                foreach (Vector2 position in iconMap[i])
                {

                    texture.SetPixel((int)position.x, (int)position.y, circleColor);
                }
            }

            lastIconIndex = -1;
        }

        return pixelsChanged;
    }

    public void initializeDestruction()
    {

        StartCoroutine(Fade());

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

        int index = lastIndex - (int)(pixelList.Count / 30f);
        if (index < 0)
        {
            index = 0;
        }

        int iconIndex = (iconMap.Count - 1) -  (pixelList.Count - 1 - index); 

        updateCirclePixels(index, true);
        updateIconPixels(iconIndex, true);

        

        //for (int i = 0; i < pixelList.Count; i++)
        //{

        //    Vector2 position = pixelList[i];
        //    Color c = texture.GetPixel((int)position.x, (int)position.y);

        //    if (c == Color.clear)
        //    {
        //        continue;
        //    }

        //    if (i <= lastIndex && index <= i)
        //    {
        //        c = circleColor;
        //    }

        //    //c.a = Mathf.Clamp(c.a - (Time.deltaTime / 2f), 0f, 1f);

        //    texture.SetPixel((int)position.x, (int)position.y, c);

        //}

        //lastIndex = index;


    }

    public void SetElementNumber(int number)
    {
        animator.SetInteger("Element", number);
    }


    //void updateSpellIcon()
    //{

    //    bool pixelsChanged = false;
    //    bool overCharged = false;
    //    int chargeIndex = -1;

    //    float percent;
    //    int index;

    //    //if (currentElementCharge > spell.castingCost && spell.OverCharge.Length > 0)
    //    //{
    //    if (currentElementCharge > castingCost)
    //    {
    //        bool found = false;

    //        float overChargeAmount = currentElementCharge - spell.castingCost;
    //        overCharged = true;
    //        foreach (float chargeAmount in spell.OverCharge)
    //        {
    //            chargeIndex++;
    //            if (overChargeAmount <= chargeAmount)
    //            {
    //                found = true;
    //                break;
    //            }

    //            overChargeAmount -= chargeAmount;
    //        }

    //        if (found)
    //        {

    //            percent = overChargeAmount / (spell.OverCharge[chargeIndex]);

    //            index = Mathf.FloorToInt(percent * overChargePixelsList[chargeIndex].Count) - 1;


    //        } else
    //        {
    //            //Its overcharged so lets return

    //            return;
    //        }

    //    }
    //    else
    //    {

    //        percent = (currentElementCharge / spell.castingCost);
    //        index = Mathf.FloorToInt(percent * texture.height) - 1;

    //    }

    //    if (index >= 0 && (index < texture.height || overCharged))
    //    {

    //        if (index != lastIndex)
    //        {

    //            pixelsChanged = true;


    //            if (index < lastIndex)
    //            {


    //                for (int tempIndex = index; tempIndex <= lastIndex; tempIndex++)
    //                {

    //                    if (overCharged)
    //                    {
    //                        Vector2 pixelVector = overChargePixelsList[chargeIndex][tempIndex];
    //                        texture.SetPixel((int)pixelVector.x, (int)pixelVector.y, Color.clear);
    //                    }
    //                    else
    //                    {

    //                        for (int x = 0; x < texture.width; x++)
    //                        {

    //                            Color pixelColor = texture.GetPixel(x, tempIndex);

    //                            if (pixelColor.a > 0 && pixelColor != Color.white)
    //                            {

    //                                texture.SetPixel(x, tempIndex, alphaColor);

    //                            }

    //                        }
    //                    }

    //                }


    //            }
    //            else
    //            {

    //                for (int tempIndex = lastIndex + 1; tempIndex <= index; tempIndex++)
    //                {

    //                    if (overCharged)
    //                    {
    //                        Vector2 pixelVector = overChargePixelsList[chargeIndex][tempIndex];
    //                        texture.SetPixel((int)pixelVector.x, (int)pixelVector.y, elementColor);
    //                    }
    //                    else
    //                    {

    //                        for (int x = 0; x < texture.width; x++)
    //                        {

    //                            Color pixelColor = texture.GetPixel(x, tempIndex);

    //                            if (pixelColor.a > 0 && pixelColor != Color.white)
    //                            {

    //                                texture.SetPixel(x, tempIndex, elementColor);
    //                            }

    //                        }
    //                    }

    //                }
    //            }

    //            lastIndex = index;


    //        }

    //        if(pixelsChanged)
    //        {
    //            applyTexture();


    //        }
    //    } else
    //    {

    //    }

    //}


    public void destroy()
    {
        Destroy(gameObject);
    }

    //void initializeOverchargePixels()
    //{

    //    Dictionary<int, Vector2> previousPixelMap = null;

    //    //Loop through the number of times the spell can be overcharged, for now we will only do once
    //    for(int i = 0; i < spell.OverCharge.Length; i++)
    //    {

    //        previousPixelMap = GetOverChargePixelMap(previousPixelMap);

    //        List<Vector2> overChargedList = new List<Vector2>();

    //        for(int index = 0; index < previousPixelMap.Count; index++)
    //        {
    //            overChargedList.Add(previousPixelMap[index]);
    //        }

    //        overChargePixelsList.Add(overChargedList);
            

    //    }

    //}

    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT
    }

    /*Checks if the pixel is clear in a given direction
     * 
     **/
    bool lookDirection (Vector2 testingVector, Dictionary<int, Vector2> pixelDict = null)
    {

        if (pixelDict == null)
        {
            return texture.GetPixel((int) testingVector.x, (int) testingVector.y).a < 0.1f;
        }
        else
        {
            return pixelDict.ContainsValue(testingVector) == false;
        }


    }

    

    Vector2 GetDirectionVector(Direction direction, Vector2 currentPixel)
    {

        Vector2 testingVector;

        switch (direction)
        {
            case Direction.LEFT:
                testingVector = new Vector2(currentPixel.x - 1, currentPixel.y);
                break;

            case Direction.RIGHT:
                testingVector = new Vector2(currentPixel.x + 1, currentPixel.y);
                break;

            case Direction.UP:
                testingVector = new Vector2(currentPixel.x, currentPixel.y + 1);
                break;

            case Direction.DOWN:
                testingVector = new Vector2(currentPixel.x, currentPixel.y - 1);
                break;

            case Direction.TOPLEFT:
                testingVector = new Vector2(currentPixel.x - 1, currentPixel.y + 1);
                break;

            case Direction.TOPRIGHT:
                testingVector = new Vector2(currentPixel.x + 1, currentPixel.y + 1);
                break;

            case Direction.BOTTOMLEFT:
                testingVector = new Vector2(currentPixel.x - 1, currentPixel.y - 1);
                break;

            case Direction.BOTTOMRIGHT:
                testingVector = new Vector2(currentPixel.x + 1, currentPixel.y - 1);
                break;

            default:
                testingVector = currentPixel;
                break;
        }

        return testingVector;
    }


    //Dictionary<int, Vector2> GetOverChargePixelMap(Dictionary<int, Vector2> previousPixelMap)
    //{
    //    This will keep track of order in the dictionary
    //    int index = 0;

    //    this dictionary will hold an int as a reference to order the pixels go through, and a vector to show actual pixel position
    //    TODO: this may not be the best way to do this, don know how fast dictionary.containsKey() runs...
    //    Dictionary<int, Vector2> pixelMap = new Dictionary<int, Vector2>();

    //    Start by getting the first pixel, starting at the leftmost pixel that is in the middle

    //    Vector2 initialPixel = new Vector2(-1, -1);


    //    for (int x = 0; x < texture.width; x++)
    //    {
    //        if (texture.GetPixel(x, texture.height / 2).a > 0.1f)
    //        {
    //            initialPixel = new Vector2(x - overChargePixelsList.Count, texture.height / 2);
    //            break;
    //        }
    //    }


    //    if (initialPixel == new Vector2(-1, -1))
    //    {
    //        return null;
    //    }

    //    Vector2 currentPixel = initialPixel;

    //    Vector2 testingVector;
    //    int number = -1;
    //    while (true)
    //    {
    //        number++;
    //        if (number != 0 && currentPixel == initialPixel)
    //        {

    //            break;
    //        }

    //        if (currentPixel.x < texture.width / 2)
    //        {

    //            we on the left side
    //            if (currentPixel.y >= texture.height / 2)
    //            {
    //                we on the leftTop side

    //                Look left and top to potentially add pixel
    //                testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {

    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                testingVector = GetDirectionVector(Direction.UP, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                Now lets find the next pixel by checking up - topRight - right
    //                testingVector = GetDirectionVector(Direction.UP, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.TOPRIGHT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //            }
    //            else
    //            {
    //                we on the leftBot side

    //                Look down and left to potentially add pixel
    //                testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                Now lets find the next pixel by checking left - topleft - top
    //                testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.TOPLEFT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.UP, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            we on the Right side
    //            if (currentPixel.y >= texture.height / 2)
    //            {
    //                we on the RightTop side

    //                Look Top and right to potentially add pixel
    //                testingVector = GetDirectionVector(Direction.UP, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                Now lets find the next pixel by checking right - bottomRight - down
    //                testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.BOTTOMRIGHT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //            }
    //            else
    //            {
    //                we on the RightBot side

    //                Look down and right to potentially add pixel
    //                testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
    //                {
    //                    pixelMap[index] = testingVector;
    //                    index++;
    //                }

    //                Now lets find the next pixel by checking down - bottomleft - left
    //                testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.BOTTOMLEFT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }

    //                testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
    //                if (lookDirection(testingVector, previousPixelMap) == false)
    //                {
    //                    currentPixel = testingVector;
    //                    continue;
    //                }
    //            }

    //        }


    //    }

    //    return pixelMap;
    //}

    void applyTexture()
    {
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

        spriteRenderer.material.mainTexture = texture as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");
    }

   
}
