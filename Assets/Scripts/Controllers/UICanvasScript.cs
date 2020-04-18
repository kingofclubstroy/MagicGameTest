using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasScript : MonoBehaviour
{

    Texture2D UITexture;
    SpriteRenderer SpriteRenderer;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    int radius;

    int lastX, lastY;

    TempSpell selectedSpell;

    [SerializeField] CastingUIController castingUI;

    // Start is called before the first frame update
    void Start()
    {

        Camera cam = Camera.main;
        float height = 2f * 105.5f;
        float width = height * cam.aspect;

        UITexture = new Texture2D((int) width, (int) height);
        UITexture.filterMode = FilterMode.Point;

        SpriteRenderer = GetComponent<SpriteRenderer>();

        DrawLine(UITexture, 0, 0, UITexture.width, UITexture.height, Color.black);
        TextureHelper.ApplyTexture(UITexture, SpriteRenderer, new Vector2(0.5f, 0.5f));

        SpellSelectedEvent.RegisterListener(spellSelected);
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

                Vector2 middleScreen = new Vector2(UITexture.width / 2, UITexture.height / 2);

                float angle = GetAngle(middleScreen, endPoint);

                //TODO: fix this, just trying to get seperate casting circle to work

                //List<Vector2> pixelList = castingUI.GetPixelList(0);

                //Vector2 startPoint = pixelList[(int)((angle / 360) * pixelList.Count)] + middleScreen;

                //startPoint.x -= castingUI.GetHalfWidth();
                //startPoint.y -= castingUI.GetHalfHeight();

                //Vector2 direction = endPoint - middleScreen;

                //Vector2 startPoint = (direction.normalized * radius) + middleScreen;

                if ((endPoint - middleScreen).magnitude < radius)
                {
                    clearTexture(UITexture);
                }
                else
                {
                    //TODO: fix this to draw line
                    //DrawLine(UITexture, (int)startPoint.x, (int)startPoint.y, (int)endPoint.x, (int)endPoint.y, Color.black);

                }

                TextureHelper.ApplyTexture(UITexture, SpriteRenderer, new Vector2(0.5f, 0.5f));
            }

        }

    }

    void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {

        clearTexture(tex);

        Line(x0, y0, x1, y1);
        
    }

    

    private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

   

   
    public void Line(int x0, int y0, int x1, int y1)
    {
        bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
        if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
        int dX = (x1 - x0), dY = Mathf.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

        for (int x = x0; x <= x1; ++x)
        {
            if (!(steep ? plot(y, x) : plot(x, y))) return;
            err = err - dY;
            if (err < 0) { y += ystep; err += dX; }
        }
    }


    bool plot(int x, int y)
    {
        UITexture.SetPixel(x, y, Color.black);
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
    }

    void spellSelected(SpellSelectedEvent selectedSpell) {

        if (selectedSpell.selected)
        {

            if (selectedSpell.spell.spellType == TempSpell.SpellType.PROJECTILE)
            {

                this.selectedSpell = selectedSpell.spell;

            }

        } else
        {
            this.selectedSpell = null;
            clearTexture(UITexture);
            TextureHelper.ApplyTexture(UITexture, SpriteRenderer, new Vector2(0.5f, 0.5f));
        }

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

   
}
