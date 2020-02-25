using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpell
{

    public CastingElements.Element element;

    public float castingCost;

    public TempSpell(CastingElements.Element element, float castingCost)
    {
        this.castingCost = castingCost;
        this.element = element;
    }

}
