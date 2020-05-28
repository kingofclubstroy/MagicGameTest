using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasScript : MonoBehaviour
{

    Texture2D UITexture;
    SpriteRenderer SpriteRenderer;

    List<Vector2> lineMap = new List<Vector2>();

    [SerializeField]
    private GameObject player;

    [SerializeField]
    int radius;

    int lastX, lastY;

    SpellTest selectedSpell;

    [SerializeField] CastingUIController castingUI;

    Vector2 castingLocation = Vector2.negativeInfinity;

    // Start is called before the first frame update
    void Start()
    {

        Camera cam = Camera.main;
        float height = 2f * 105.5f;
        float width = height * cam.aspect;

        UITexture = TextureHelper.MakeTexture((int) width, (int) height, Color.clear);
        //UITexture.filterMode = FilterMode.Point;

        SpriteRenderer = GetComponent<SpriteRenderer>();
        //DrawLine(UITexture, 0, 0, UITexture.width, UITexture.height, Color.black);
        TextureHelper.ApplyTexture(UITexture, SpriteRenderer, new Vector2(0.5f, 0.5f));

        initializeEventCallbacks();

    }

    // Update is called once per frame
    void Update()
    {

        //TODO: move this to an interaction controller or something, it doesnt need to be here, but it will do for now

        if (selectedSpell != null)
        {

            Vector2 mousePosition = Input.mousePosition;

            Vector2 endPoint = new Vector2(Mathf.Clamp(Mathf.FloorToInt((mousePosition.x / Screen.width) * UITexture.width), 0, UITexture.width), Mathf.Clamp(Mathf.FloorToInt((mousePosition.y / Screen.height) * UITexture.height), 0, UITexture.height));

            if ((int)endPoint.x != lastX || (int)endPoint.y != lastY)
            {

                lastX = (int)endPoint.x;
                lastY = (int)endPoint.y;

                Vector2 middleScreen;
                if (castingLocation == Vector2.negativeInfinity)
                {
                    middleScreen = new Vector2(UITexture.width / 2, UITexture.height / 2);
                } else
                {
                    middleScreen = castingLocation;
                }

                float angle = GetAngle(middleScreen, endPoint);

                //TODO: fix this, just trying to get seperate casting circle to work

                //List<Vector2> pixelList = castingUI.getPixelList();

                //Vector2 startPoint = pixelList[(int)((angle / 360) * pixelList.Count)] + middleScreen;

                //startPoint.x -= castingUI.GetHalfWidth();
                //startPoint.y -= castingUI.GetHalfHeight();

                Vector2 direction = endPoint - middleScreen;

                Vector2 startPoint = (direction.normalized * radius) + middleScreen;

                if ((endPoint - middleScreen).magnitude < radius)
                {
                    //clearTexture(UITexture);
                    clearLine();
                }
                else
                {
                    
                    DrawLine(UITexture, (int)startPoint.x, (int)startPoint.y, (int)endPoint.x, (int)endPoint.y, selectedSpell.color);

                }

                
            }

        }

    }

    void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {

        clearLine();

        //clearTexture(tex);

        Line(x0, y0, x1, y1, col);
        tex.Apply();
        
    }

    

    private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

    void clearLine()
    {
        foreach(Vector2 position in lineMap)
        {
            UITexture.SetPixel((int)position.x, (int)position.y, Color.clear);
        }

        lineMap.Clear();


    }

   

   
    public void Line(int x0, int y0, int x1, int y1, Color c)
    {
        bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
        if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
        int dX = (x1 - x0), dY = Mathf.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

        for (int x = x0; x <= x1; ++x)
        {
            if (!(steep ? plot(y, x, c) : plot(x, y, c))) return;
            err = err - dY;
            if (err < 0) { y += ystep; err += dX; }
        }
    }


    bool plot(int x, int y, Color c)
    {
        UITexture.SetPixel(x, y, c);
        lineMap.Add(new Vector2(x, y));
        return true;
    }

    void clearTexture(Texture2D tex)
    {
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {

                tex.SetPixel(x, y, Color.clear);

            }
        }

        tex.Apply();
    }

    float GetAngle(Vector2 center, Vector2 endPoint)
    {

        Vector2 vector = new Vector2(center.x - 1, center.y);

     
        float angle = Vector2.SignedAngle(endPoint - center, vector - center);

        if(angle >= 0)
        {
            return angle;
        } else
        {
            return 360 + angle;
        }

    }

    #region eventCallback functions

    void initializeEventCallbacks()
    {

        SpellSelectedEvent.RegisterListener(spellSelected);
        SpellUnSelectedEvent.RegisterListener(spellUnSelected);

        CastingLocationChangedEvent.RegisterListener(castingLocationChanged);
        StoppedCastingEvent.RegisterListener(stoppedCasting);

    }

    void spellSelected(SpellSelectedEvent selectedSpell)
    {

        Debug.Log("UI canvas spell selected");

        if (selectedSpell.selected)
        {



            //if (selectedSpell.spell.spellType == TempSpell.SpellType.PROJECTILE)
            //{

            this.selectedSpell = selectedSpell.spell.spell;

            //}

        }
        else
        {
            this.selectedSpell = null;
            clearTexture(UITexture);
            TextureHelper.ApplyTexture(UITexture, SpriteRenderer, new Vector2(0.5f, 0.5f));
        }

    }

    void spellUnSelected(SpellUnSelectedEvent unSelected)
    {
        selectedSpell = null;
        //clearTexture(UITexture);
        clearLine();
        UITexture.Apply();
    }

    void castingLocationChanged(CastingLocationChangedEvent e)
    {
        castingLocation = e.go.transform.position;
    }

    void stoppedCasting(StoppedCastingEvent e)
    {
        castingLocation = Vector2.negativeInfinity;
    }

    

    #endregion


}
