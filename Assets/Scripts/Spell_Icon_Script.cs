using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Icon_Script : MonoBehaviour
{

    [SerializeField]
    Texture2D SampleTexture;

    public TempSpell spell;

    SpriteRenderer spriteRenderer;

    Color alphaColor;
    Color elementColor;

    Texture2D texture;

    int lastIndex = -1;

    float currentElementCharge = 0;

    List<List<Vector2>> overChargePixelsList = new List<List<Vector2>>();
    



    // Start is called before the first frame update
    void Start()
    {

        GetComponent<BoxCollider2D>().size = new Vector2(SampleTexture.width, SampleTexture.height);
       
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spell != null && texture != null)
        {
            //DO stuff
            updateSpellIcon();

        }
        //} else
        //{
        //    Debug.Log("spell or texture null");
        //    Debug.Log("spell = ");
        //    Debug.Log(spell);
        //    Debug.Log("texture = ");
        //    Debug.Log(texture);
        //}
    }

    public void Initialize(TempSpell spell, Color elementColor)
    {
        
        this.spell = spell;
        this.elementColor = elementColor;
        spriteRenderer = GetComponent<SpriteRenderer>();

        colorTexture(elementColor);

        initializeOverchargePixels();
       

        

    }

    void colorTexture(Color elementColor)
    {

        texture = new Texture2D(SampleTexture.width, SampleTexture.height);

        texture.SetPixels(SampleTexture.GetPixels());

        texture.filterMode = FilterMode.Point;

        alphaColor = elementColor;

        alphaColor.a = 0.5f;

        texture.filterMode = FilterMode.Point;

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {

                if (texture.GetPixel(x, y).a == 1)
                {

                    texture.SetPixel(x, y, alphaColor);

                }

            }
        }

        applyTexture();


    }

    public void setElementCharge(float charge)
    {
        currentElementCharge = charge;
    }


    void updateSpellIcon()
    {

        bool pixelsChanged = false;
        bool overCharged = false;
        int chargeIndex = -1;

        float percent;
        int index;

        if (currentElementCharge > spell.castingCost && spell.OverCharge.Length > 0)
        {
            bool found = false;

            float overChargeAmount = currentElementCharge - spell.castingCost;
            overCharged = true;
            foreach (float chargeAmount in spell.OverCharge)
            {
                chargeIndex++;
                if (overChargeAmount <= chargeAmount)
                {
                    found = true;
                    break;
                }

                overChargeAmount -= chargeAmount;
            }

            if (found)
            {

                percent = overChargeAmount / (spell.OverCharge[chargeIndex]);
               
                index = Mathf.FloorToInt(percent * overChargePixelsList[chargeIndex].Count) - 1;
                

            } else
            {
                //Its overcharged so lets return
                
                return;
            }

        }
        else
        {

            percent = (currentElementCharge / spell.castingCost);
            index = Mathf.FloorToInt(percent * texture.height) - 1;

        }

        if (index >= 0 && (index < texture.height || overCharged))
        {

            if (index != lastIndex)
            {

                pixelsChanged = true;


                if (index < lastIndex)
                {
                   
                   
                    for (int tempIndex = index; tempIndex <= lastIndex; tempIndex++)
                    {

                        if (overCharged)
                        {
                            Vector2 pixelVector = overChargePixelsList[chargeIndex][tempIndex];
                            texture.SetPixel((int)pixelVector.x, (int)pixelVector.y, Color.clear);
                        }
                        else
                        {

                            for (int x = 0; x < texture.width; x++)
                            {

                                Color pixelColor = texture.GetPixel(x, tempIndex);

                                if (pixelColor.a > 0 && pixelColor != Color.white)
                                {

                                    texture.SetPixel(x, tempIndex, alphaColor);

                                }

                            }
                        }

                    }
                    

                }
                else
                {

                    for (int tempIndex = lastIndex + 1; tempIndex <= index; tempIndex++)
                    {

                        if (overCharged)
                        {
                            Vector2 pixelVector = overChargePixelsList[chargeIndex][tempIndex];
                            texture.SetPixel((int)pixelVector.x, (int)pixelVector.y, elementColor);
                        }
                        else
                        {

                            for (int x = 0; x < texture.width; x++)
                            {

                                Color pixelColor = texture.GetPixel(x, tempIndex);

                                if (pixelColor.a > 0 && pixelColor != Color.white)
                                {

                                    texture.SetPixel(x, tempIndex, elementColor);
                                }

                            }
                        }

                    }
                }

                lastIndex = index;


            }

            if(pixelsChanged)
            {
                applyTexture();

                
            }
        } else
        {
           
        }

    }


    public void destroy()
    {
        Destroy(gameObject);
    }

    void initializeOverchargePixels()
    {

        Dictionary<int, Vector2> previousPixelMap = null;

        //Loop through the number of times the spell can be overcharged, for now we will only do once
        for(int i = 0; i < spell.OverCharge.Length; i++)
        {

            previousPixelMap = GetOverChargePixelMap(previousPixelMap);

            List<Vector2> overChargedList = new List<Vector2>();

            for(int index = 0; index < previousPixelMap.Count; index++)
            {
                overChargedList.Add(previousPixelMap[index]);
            }

            overChargePixelsList.Add(overChargedList);
            

        }

    }

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


    Dictionary<int, Vector2> GetOverChargePixelMap(Dictionary<int, Vector2> previousPixelMap)
    {
        //This will keep track of order in the dictionary
        int index = 0;

        //this dictionary will hold an int as a reference to order the pixels go through, and a vector to show actual pixel position
        //TODO: this may not be the best way to do this, don know how fast dictionary.containsKey() runs...
        Dictionary<int, Vector2> pixelMap = new Dictionary<int, Vector2>();

        //Start by getting the first pixel, starting at the leftmost pixel that is in the middle

        Vector2 initialPixel = new Vector2(-1, -1);

       
        for (int x = 0; x < texture.width; x++)
        {
            if (texture.GetPixel(x, texture.height / 2).a > 0.1f)
            {
                initialPixel = new Vector2(x - overChargePixelsList.Count, texture.height / 2);
                break;
            }
        }
        

        if (initialPixel == new Vector2(-1, -1))
        {
            return null;
        }

        Vector2 currentPixel = initialPixel;

        Vector2 testingVector;
        int number = -1;
        while (true)
        {
            number++;
            if (number != 0 && currentPixel == initialPixel)
            {
                
                break;
            }

            if (currentPixel.x < texture.width / 2)
            {
               
                //we on the left side
                if (currentPixel.y >= texture.height / 2)
                {
                    //we on the leftTop side
                   
                    //Look left and top to potentially add pixel
                    testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    testingVector = GetDirectionVector(Direction.UP, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    //Now lets find the next pixel by checking up-topRight-right
                    testingVector = GetDirectionVector(Direction.UP, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.TOPRIGHT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                }
                else
                {
                    //we on the leftBot side

                    //Look down and left to potentially add pixel
                    testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    //Now lets find the next pixel by checking left-topleft-top
                    testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.TOPLEFT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.UP, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }
                }
            }
            else
            {
                //we on the Right side
                if (currentPixel.y >= texture.height / 2)
                {
                    //we on the RightTop side

                    //Look Top and right to potentially add pixel
                    testingVector = GetDirectionVector(Direction.UP, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    //Now lets find the next pixel by checking right-bottomRight-down
                    testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.BOTTOMRIGHT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                }
                else
                {
                    //we on the RightBot side

                    //Look down and right to potentially add pixel
                    testingVector = GetDirectionVector(Direction.RIGHT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) && pixelMap.ContainsValue(testingVector) == false)
                    {
                        pixelMap[index] = testingVector;
                        index++;
                    }

                    //Now lets find the next pixel by checking down-bottomleft-left
                    testingVector = GetDirectionVector(Direction.DOWN, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.BOTTOMLEFT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }

                    testingVector = GetDirectionVector(Direction.LEFT, currentPixel);
                    if (lookDirection(testingVector, previousPixelMap) == false)
                    {
                        currentPixel = testingVector;
                        continue;
                    }
                }

            }


        }

        return pixelMap;
    }

    public void unselect()
    {
        texture.SetPixel((texture.width / 2) - 1, texture.height / 2, Color.clear);
        texture.SetPixel(texture.width / 2, texture.height / 2, Color.clear);
        texture.SetPixel((texture.width / 2) - 1, (texture.height / 2) - 1, Color.clear);
        texture.SetPixel(texture.width / 2, (texture.height / 2) - 1, Color.clear);

        applyTexture();
    }

    public void select()
    {
        texture.SetPixel((texture.width / 2) - 1, texture.height / 2, Color.white);
        texture.SetPixel(texture.width / 2, texture.height / 2, Color.white);
        texture.SetPixel((texture.width / 2) - 1, (texture.height / 2) - 1, Color.white);
        texture.SetPixel(texture.width / 2, (texture.height / 2) - 1, Color.white);

        applyTexture();
    }

    void applyTexture()
    {
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

        spriteRenderer.material.mainTexture = texture as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");
    }

   
}
