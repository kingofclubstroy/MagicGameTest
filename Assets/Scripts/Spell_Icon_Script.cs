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

    // Start is called before the first frame update
    void Start()
    {
        

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

                if (texture.GetPixel(x, y) != Color.clear)
                {

                    texture.SetPixel(x, y, alphaColor);

                }

            }
        }

        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

        spriteRenderer.material.mainTexture = texture as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");


    }

    public void setElementCharge(float charge)
    {
        currentElementCharge = charge;
    }


    void updateSpellIcon()
    {

        bool pixelsChanged = false;

        float percent = (currentElementCharge / spell.castingCost);

        int index = Mathf.FloorToInt(percent * texture.height) - 1;

        if (index >= 0 && index < texture.height)
        {

            if (index != lastIndex)
            {

                pixelsChanged = true;


                if (index < lastIndex)
                {

                    for (int tempIndex = index; tempIndex <= lastIndex; tempIndex++)
                    {

                        for (int x = 0; x < texture.width; x++)
                        {

                            if (texture.GetPixel(x, tempIndex) != Color.clear)
                            {

                                texture.SetPixel(x, tempIndex, alphaColor);

                            }

                        }

                    }

                }
                else
                {

                    for (int tempIndex = lastIndex + 1; tempIndex <= index; tempIndex++)
                    {

                        for (int x = 0; x < texture.width; x++)
                        {

                            if (texture.GetPixel(x, tempIndex) != Color.clear)
                            {

                                texture.SetPixel(x, tempIndex, elementColor);
                            }

                        }

                    }
                }

                lastIndex = index;


            }

            if(pixelsChanged)
            {
                texture.Apply();

                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

                spriteRenderer.material.mainTexture = texture as Texture;
                spriteRenderer.material.shader = Shader.Find("Sprites/Default");

                
            }
        } else
        {
            Debug.Log("outside range");
            Debug.Log("index = " + index);
            Debug.Log("percent = " + percent);
        }

    }


    public void destroy()
    {
        Destroy(gameObject);
    }

    
}
