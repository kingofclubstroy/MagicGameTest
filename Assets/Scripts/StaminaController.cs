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
    Color fillColor, emptyColor;

    List<Vector2> pixelList;

    [SerializeField]
    float castingDrain;

    [SerializeField]
    float regenRate;

    bool isCasting = false;

    int lastIndex;

    bool fadingOut = false;

    bool fadingIn = false;

    SpriteRenderer spriteRenderer;

    bool texInitialized = false;

    // Start is called before the first frame update
    void Start()
    {

        stamina = maxStamina;

        initializeEventCallbacks();

        spriteRenderer = GetComponent<SpriteRenderer>();

       

        staminaTex = TextureHelper.MakeTexture(initialTex.width, initialTex.height, Color.clear);  //new Color(0, 0, 0, 0.01f)

        pixelList = HelperFunctions.makePixelMap(initialTex);

        lastIndex = 0;

       
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
            drainStamina(castingDrain);
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

        int index = pixelList.Count - (int) (percent * pixelList.Count);

        if(index == lastIndex)
        {
            return;
        }

        if(index < lastIndex)
        {
            for(int i = index; i < lastIndex; i++)
            {
                Vector2 position = pixelList[i];
                staminaTex.SetPixel((int)position.x, (int)position.y, fillColor);
            }
        } else
        {
            for(int i = lastIndex; i < index; i++)
            {
                Vector2 position = pixelList[i];
                staminaTex.SetPixel((int)position.x, (int)position.y, emptyColor);
            }
        }

        lastIndex = index;
        staminaTex.Apply();

    }

    void initializeEventCallbacks()
    {
        StartedCastingEvent.RegisterListener(castingStarted);
        StoppedCastingEvent.RegisterListener(castingStopped);
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
        if(fadingIn)
        {
            StopCoroutine(FadeIn());
            fadingIn = false;
        }
        StartCoroutine(FadeOut());
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
}
