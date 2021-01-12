using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Icon_Script : MonoBehaviour
{

    public Spell spell;

    public CharacterController character;

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

    float castTime = 0f;

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

    public bool Charged
    {
        get
        {
            return castTime >= spell.spellParams.castTime;
        }
        set
        {
            animator.SetBool("IsCharged", value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        spriteHandler = FindObjectOfType<SpriteHandler>();
        initializeCallbacks();
        
    }

    private void OnDestroy()
    {
        unRegisterCallbacks();
    }

    public void Initialize(Spell spell, Color elementColor, CharacterController character)
    {

        //this.spell = spell;
        this.elementColor = elementColor;
        spriteRenderer = GetComponent<SpriteRenderer>();

        this.spell = spell;

        this.character = character;

        //colorTexture(elementColor);

        //initializeOverchargePixels();




    }

    public void updateSpellIcon(float charge)
    {

        bool pixelsChanged = false;

        if (pixelList == null)
        {
            setupTexture();
        }

        if(character.staminaController.EnoughStamina(spell.spellParams.staminaCost) && charge >= spell.spellParams.elementCost)
        {
            castTime = Mathf.Clamp(castTime + Time.deltaTime, 0, spell.spellParams.castTime);
        } else
        {
            castTime = Mathf.Clamp(castTime - Time.deltaTime, 0, spell.spellParams.castTime);
        }

        if(castTime >= spell.spellParams.castTime)
        {
            Charged = true;
        } else
        {
            Charged = false;
        }

        float percent = Mathf.Clamp(castTime / spell.spellParams.castTime , 0, 1);
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

    public void destroy()
    {
        Destroy(gameObject);
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


    

    void setupTexture()
    {
        
        if (spriteHandler == null)
        {
            spriteHandler = FindObjectOfType<SpriteHandler>();
        }

        Texture2D tex = spriteHandler.GetTexture(spell.spellType);

        texture = new Texture2D(tex.width, tex.height);
        texture.SetPixels(tex.GetPixels());
        texture.filterMode = FilterMode.Point;
        pixelList = HelperFunctions.makePixelMap(tex);
        iconMap = HelperFunctions.makeSpellIconMap(tex);
        
        GetComponent<BoxCollider2D>().size = new Vector2(tex.width, tex.height);

        applyTexture();

    }

    void applyTexture()
    {
        texture.Apply();

        spriteRenderer.sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

        spriteRenderer.material.mainTexture = texture as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");
    }

    #region callbacks

    void initializeCallbacks()
    {
        StoppedCastingEvent.RegisterListener(StoppedCasting);

        SpellCastEvent.RegisterListener(SpellCast);
    }

    void unRegisterCallbacks()
    {
        StoppedCastingEvent.UnregisterListener(StoppedCasting);
        SpellCastEvent.UnregisterListener(SpellCast);
    }

    void StoppedCasting(StoppedCastingEvent e)
    {
        if (Selected)
        {
            Selected = false;
        }
        
    }

    void SpellCast(SpellCastEvent e)
    {
        if (Selected)
        {
            Selected = false;
        }

    }

    #endregion


}
