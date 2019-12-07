using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spriteHandleing {
    static public class SpriteHandler
    {
        //This is a class that holds all the sprite information
        //TODO: should read this data from file, and potentially add a tool for designers to add this data more easily
        static public Dictionary<string, BaseSprite> baseSprites = new Dictionary<string, BaseSprite>();
        static int width = 5;
        static int height = 5;

        static public Dictionary<string, ArmorSprite> armorSprites = new Dictionary<string, ArmorSprite>();

        static public void createSprites()
        {

            Pixelnfo[,] testPixels = new Pixelnfo[height, width];

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    testPixels[y, x] = new Pixelnfo(colorAccent.empty);
                }
            }

            testPixels[0, 2] = new Pixelnfo(colorAccent.primary);
            testPixels[1, 1] = new Pixelnfo(colorAccent.secondary);
            testPixels[1, 2] = new Pixelnfo(colorAccent.secondary);
            testPixels[1, 3] = new Pixelnfo(colorAccent.secondary);
            testPixels[2, 0] = new Pixelnfo(colorAccent.tertiary);
            testPixels[2, 1] = new Pixelnfo(colorAccent.tertiary);
            testPixels[2, 2] = new Pixelnfo(colorAccent.tertiary);
            testPixels[2, 3] = new Pixelnfo(colorAccent.tertiary);
            testPixels[2, 4] = new Pixelnfo(colorAccent.tertiary);
            testPixels[3, 1] = new Pixelnfo(colorAccent.secondary);
            testPixels[3, 2] = new Pixelnfo(colorAccent.secondary);
            testPixels[3, 3] = new Pixelnfo(colorAccent.secondary);
            testPixels[4, 2] = new Pixelnfo(colorAccent.primary);

            BaseSprite testSprite = new BaseSprite("testSprite", testPixels);

            testSprite.height = height;
            testSprite.width = width;

            baseSprites.Add("testSprite", testSprite);

            armorSprites.Add("testArmorSprite", new ArmorSprite(Color.green, Color.white, Color.gray, baseSprites["testSprite"]));




        }


    }
}
