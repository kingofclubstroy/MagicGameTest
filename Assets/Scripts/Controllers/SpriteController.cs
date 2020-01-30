using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{

    #region Private Fields

    [SerializeField]
    private FireObject[] fireSprites;

    [SerializeField]
    private Sprite[] grassSprites;

    [SerializeField]
    private Sprite[] groundSprites;

    #endregion



    public FireObject getRandomFireSprite()
    {

        if (fireSprites.Length == 0)
        {

            return null;
        }

        return fireSprites[Random.Range(0, fireSprites.Length)];

    }

    public Sprite getRandomGrassSprite()
    {
        
        if (grassSprites.Length == 0)
        {

            return null;
        }

        return grassSprites[Random.Range(0, grassSprites.Length)];
    }

    public Sprite getRandomGroundSprite()
    {


        if (groundSprites.Length == 0)
        {

            return null;
        }

        return groundSprites[Random.Range(0, groundSprites.Length)];

    }


}
