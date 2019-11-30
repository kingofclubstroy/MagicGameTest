using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTextureScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Texture2D texture = new Texture2D(128, 128);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

       
        

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = ((x & y) != 0 ? Color.black : Color.blue);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        spriteRenderer.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
