using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spriteHandleing
{
    public class testTextureScript : MonoBehaviour
    {
        [SerializeField] public int pixelSize = 40;
        [SerializeField] public int width;
        [SerializeField] public int height;

        Color primaryColor;
        Color secondaryColor;
        Color tertiaryColor;


        // Start is called before the first frame update
        void Start()
        {

            drawSprite(makeArmorSprite("testArmorSprite"));

        }

        // Update is called once per frame
        void Update()
        {

        }

        Color GetColor(colorAccent accent)
        {
            switch (accent)
            {
                case colorAccent.primary:
                    return primaryColor;

                case colorAccent.secondary:
                    return secondaryColor;

                case colorAccent.tertiary:
                    return tertiaryColor;

                case colorAccent.empty:
                    return Color.clear;
            }

            return tertiaryColor;

        }

        Pixelnfo[,] randomPixels(int height, int width)
        {


            Pixelnfo[,] pixelArray = new Pixelnfo[height, width];


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    int random = Random.Range(0, 3);
                    colorAccent accent;

                    switch (random)
                    {
                        case 0:
                            accent = colorAccent.primary;
                            break;
                        case 1:
                            accent = colorAccent.secondary;

                            break;
                        case 2:
                            accent = colorAccent.tertiary;
                            break;
                        default:
                            accent = colorAccent.primary;
                            break;
                    }

                    Pixelnfo pixel = new Pixelnfo(accent);

                    pixelArray[y, x] = pixel;

                }
            }

            return pixelArray;

        }

        ArmorSprite makeArmorSprite(string armorName)
        {

            SpriteHandler.createSprites();

            ArmorSprite armorSprite = SpriteHandler.armorSprites[armorName];

            return armorSprite;

        }


        void drawSprite(ArmorSprite armorSprite)
        {

            width = armorSprite.baseSprite.width;
            height = armorSprite.baseSprite.height;

            primaryColor = armorSprite.colors[0];
            secondaryColor = armorSprite.colors[1];
            tertiaryColor = armorSprite.colors[2];

            Texture2D texture = new Texture2D(width * pixelSize, height * pixelSize);

            texture.filterMode = FilterMode.Point;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            Pixelnfo[,] pixelArray = armorSprite.baseSprite.pixelArray;

            Debug.Log(pixelArray);


            for (int y = 0; y < texture.height; y++)
            {
                int pixelIndexY = y / pixelSize;


                for (int x = 0; x < texture.width; x++)
                {
                    //Color color = ((x & y) != 0 ? Color.black : Color.blue);
                    int pixelIndexX = x / pixelSize;

                    Pixelnfo info = pixelArray[pixelIndexY, pixelIndexX];

                    //Color color = ((x <= 64 && y <= 64) ? Color.black : Color.blue);

                    if(y == 0)
                    {
                        //Debug.Log(GetColor(info.Accent));
                        Debug.Log(info.Accent);
                    }

                    texture.SetPixel(x, y, GetColor(info.Accent));
                }
            }

            texture.Apply();

            spriteRenderer.sprite = sprite;
        }


    }

    
}
