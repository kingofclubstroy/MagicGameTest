using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpellTest
{
    [SerializeField]
    String name;

    [SerializeField]
    CastingUIController.Element element;

    [SerializeField]
    int castingCost;

    [SerializeField]
    Texture2D spellTexture;
   
    public SpellTest()
    {

    }

    public CastingUIController.Element getElement()
    {
        return element;
    }

    public Texture2D getTexture()
    {
        return spellTexture;
    }

    public int getCastingCost()
    {
        return castingCost;
    }
}
