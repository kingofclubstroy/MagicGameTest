using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaController : MonoBehaviour
{
    [SerializeField]
    Texture2D initialTex;

    Texture2D staminaTex;

    [SerializeField]
    float maxStamina;

    float stamina;

    [SerializeField]
    Color fillColor, emptyColor, afterColor;

    List<Vector2> pixelList;

    [SerializeField]
    float castingDrain;

    [SerializeField]
    float regenRate;

    bool isCasting = false;

    //int lastIndex;

    bool fadingOut = false;

    bool fadingIn = false;

    SpriteRenderer spriteRenderer;

    bool texInitialized = false;

    int staminaCost = -1;

    bool changed = true;

    bool castingProjectionActive = false;

    // Start is called before the first frame update
    void Start()
    {

        stamina = maxStamina;

        initializeEventCallbacks();

        spriteRenderer = GetComponent<SpriteRenderer>();

        staminaTex = TextureHelper.MakeTexture(initialTex.width, initialTex.height, Color.clear);  //new Color(0, 0, 0, 0.01f)

        pixelList = HelperFunctions.makePixelMap(initialTex);

        //lastIndex = 0;

       
        staminaTex.filterMode = FilterMode.Point;

        foreach (Vector2 position in pixelList)
        {
            staminaTex.SetPixel((int)position.x, (int)position.y, fillColor);
        }

        staminaTex.Apply();
        TextureHelper.initializeTexture(staminaTex, spriteRenderer, new Vector2(0.5f, 0.5f));
        Color matColor = spriteRenderer.material.color;
        matColor.a = 0f;

        spriteRenderer.material.color = matColor;




    }

    // Update is called once per frame
    void Update()
    {
        
        if(isCasting)
        {
            if (castingProjectionActive || changed)
            {
                drainStamina(castingDrain);
            }
        } else
        {
            regenStamina(regenRate);
        }

    }

    void drainStamina(float amount)
    {
        
        if (stamina > 0)
        {
            
            stamina = Mathf.Clamp(stamina - amount, 0, maxStamina);
            updateStaminaBar();
            changed = false;
            

        } else if(isCasting)
        {
            //Twe dont have any stamina, so we cant cadt anymore, so tell everyone shows over
            isCasting = false;
            StopCastingCall e = new StopCastingCall();
            e.FireEvent();
        }
    }

    void regenStamina(float amount)
    {
        if (stamina < maxStamina)
        {
            stamina = Mathf.Clamp(stamina + amount, 0, maxStamina);
            updateStaminaBar();
        }
    }

    void updateStaminaBar()
    {

        float percent = (float) stamina / maxStamina;

        int index = (int) (percent * pixelList.Count);

        //if (index != lastIndex)
        //{

            //if (index < lastIndex)
            //{
        for (int i = 0; i <= pixelList.Count; i++)
        {
            if (i < pixelList.Count)
            {
                if (i <= index)
                {
                    Vector2 position = pixelList[i];
                    staminaTex.SetPixel((int)position.x, (int)position.y, fillColor);
                }
                    
                else
                {
                    Vector2 position = pixelList[i];
                    staminaTex.SetPixel((int)position.x, (int)position.y, emptyColor);
                }
            }
            
        }
        
        if (staminaCost != -1)
        {
            
            float afterPercent = (float)(stamina - staminaCost) / maxStamina;

            int afterIndex = Mathf.Clamp((int)(afterPercent * pixelList.Count), 0, (int) maxStamina);

            for(int i = afterIndex; i <= index; i++)
            {
                if (i < pixelList.Count)
                {
                    Vector2 position = pixelList[i];
                    staminaTex.SetPixel((int)position.x, (int)position.y, afterColor);
                }
            }


        }

        //lastIndex = index;
        staminaTex.Apply();

    }

    void clearTexture()
    {
        foreach(Vector2 pos in pixelList)
        {
            staminaTex.SetPixel((int) pos.x, (int) pos.y, emptyColor);
        }

        staminaTex.Apply();
    }

    IEnumerator FadeOut()
    {
        fadingOut = true;

        while(spriteRenderer.material.color.a > 0)
        {
            Color matCol = spriteRenderer.material.color;

            matCol.a -= 0.03f;
            spriteRenderer.material.color = matCol;

            yield return new WaitForSeconds(0.01f);
        }

        fadingOut = false;
    }

    IEnumerator FadeIn()
    {

        fadingIn = true;

        while (spriteRenderer.material.color.a < 1)
        {
            Color matCol = spriteRenderer.material.color;

            matCol.a += 0.03f;
            spriteRenderer.material.color = matCol;

            yield return new WaitForSeconds(0.01f);
        }

        fadingIn = false;
    }

    public bool EnoughStamina()
    {
        if(staminaCost == -1 || stamina - staminaCost < 0)
        {
            return false;
        }

        return true;
    }

    public bool EnoughStamina(int cost)
    {
        return stamina >= cost;
    }

    #region Callback functions

    void initializeEventCallbacks()
    {
        StartedCastingEvent.RegisterListener(castingStarted);
        StoppedCastingEvent.RegisterListener(castingStopped);
        SpellSelectedEvent.RegisterListener(SpellSelected);
        SpellCastEvent.RegisterListener(SpellCast);
        SpellUnSelectedEvent.RegisterListener(SpellUnselected);
        CastingProjectionDestroyedEvent.RegisterListener(CastingProjectionDestroyed);
        CastingProjectionCreatedEvent.RegisterListener(CastingProjectionCreated);
        CastingLocationChangedEvent.RegisterListener(CastingLocationChanged);
    }

    void castingStarted(StartedCastingEvent e)
    {
        isCasting = true;
        if (fadingOut)
        {
            StopCoroutine(FadeOut());
            fadingOut = false;
        }
        StartCoroutine(FadeIn());
    }

    void castingStopped(StoppedCastingEvent e)
    {
        isCasting = false;
        if (fadingIn)
        {
            StopCoroutine(FadeIn());
            fadingIn = false;
        }

        //if(staminaCost != - 1)
        //{
        //    float percent = (float)stamina / maxStamina;
        //    int index = pixelList.Count - (int)(percent * pixelList.Count);
        //    for(int i = 0; i <= index; i++)
        //    {
        //        if(i < pixelList.Count)
        //        {
        //            Vector2 position = pixelList[i];
        //            staminaTex.SetPixel((int)position.x, (int)position.y, fillColor);
        //        }
        //    }

        //}

        StartCoroutine(FadeOut());
    }

    void SpellSelected(SpellSelectedEvent e)
    {
        staminaCost = e.spell.spell.spellParams.staminaCost;
        changed = true;

    }

    void SpellCast(SpellCastEvent e)
    {
        stamina -= staminaCost;
        staminaCost = -1;
        float percent = (float)stamina / maxStamina;
        //lastIndex = pixelList.Count - (int)(percent * pixelList.Count);
        changed = true;
        //clearTexture();
    }

    void SpellUnselected(SpellUnSelectedEvent e)
    {
        staminaCost = -1;
        changed = true;
    }

    void CastingProjectionCreated(CastingProjectionCreatedEvent e)
    {
        castingProjectionActive = true;
    }

    void CastingProjectionDestroyed(CastingProjectionDestroyedEvent e)
    {
        castingProjectionActive = false;
    }

    void CastingLocationChanged(CastingLocationChangedEvent e)
    {
        castingProjectionActive = false;
    }


    #endregion
}
