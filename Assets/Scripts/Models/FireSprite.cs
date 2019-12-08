using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FireSprite
{

    private Sprite[] sprites;

    private int CurrentIndex = 0;

    public FireSprite(FireSprite fireSprite)
    {
        this.sprites = fireSprite.sprites;
    }

    public Sprite getNextSprite()
    {
        if(CurrentIndex >= sprites.Length)
        {
            return null;
        } else
        {
            Sprite sprite = sprites[CurrentIndex];
            CurrentIndex++;
            return sprite;
        }
    }

}
