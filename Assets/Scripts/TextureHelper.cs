using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureHelper
{
    
    public static Texture2D MakeTexture(int width, int height, Color color)
    {

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        tex.filterMode = FilterMode.Point;

        Color fillColor = color;
        Color[] fillPixels = new Color[tex.width * tex.height];

        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = fillColor;
        }

        tex.SetPixels(fillPixels);

        tex.Apply();

        return tex;

    } 

    public static void initializeTexture(Texture2D tex, SpriteRenderer spriteRenderer, Vector2 pivet)
    {
        spriteRenderer.sprite = UnityEngine.Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivet, 1);

        spriteRenderer.material.mainTexture = tex as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");
    }

    public static void ApplyTexture(Texture2D tex, SpriteRenderer spriteRenderer, Vector2 pivet)
    {
      
        tex.Apply();

        spriteRenderer.sprite = UnityEngine.Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivet, 1);

        spriteRenderer.material.mainTexture = tex as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");



    }

}
