using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct SpellIconTextureStruct
{
    [SerializeField]
    public SpellTypes type;
    [SerializeField]
    public Texture2D spellIcon;
}


public class SpriteHandler : MonoBehaviour
{
    [SerializeField]
    public SpellIconTextureStruct[] spellIcons;



    public Texture2D GetTexture(SpellTypes spellType)
    {
        for (int i = 0; i < spellIcons.Length; i++)
        {
            if (spellIcons[i].type == spellType)
            {
                return spellIcons[i].spellIcon;
            }
        }

        return null;
    }



}
