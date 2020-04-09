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

    public int lastIndex = 0;

    public float updateAmount = 0;
    
    public CastingElements(Element element, int update)
    {

        this.element = element;
        this.updateAmount = update;

    }

    public void addTempAmount(float temp)
    {
        updateAmount = temp;
    }

}
