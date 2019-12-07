using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace spriteHandleing
{
    public class ArmorSprite
    {
        public Color[] colors = new Color[3];
        public BaseSprite baseSprite;

        public ArmorSprite(Color primary, Color secondary, Color tertiary, BaseSprite baseSprite)
        {
            colors[0] = primary;
            colors[1] = secondary;
            colors[2] = tertiary;

            this.baseSprite = baseSprite;
        }
    }
}
