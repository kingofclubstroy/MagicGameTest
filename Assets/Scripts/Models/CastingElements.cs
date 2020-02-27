using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingElements
{

    public enum Element
    {
        FIRE,
        NATURE,
        EARTH,
        WIND,
        WATER
    }

    public Element element;

    public float amount = 0;
    public int numTiles = 0;

    public int lastIndex = 0;

    public float updateAmount = 0;
    
    public CastingElements(Element element)
    {

        this.element = element;

    }

    public void addTempAmount(float temp)
    {
        updateAmount += temp;
        numTiles += 1;
    }

    public void Reset()
    {
        updateAmount = 0;
        numTiles = 0;
    }

}
