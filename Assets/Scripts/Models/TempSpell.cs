using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpell
{

    public CastingElements.Element element;

    public float castingCost;

    public float[] overCharge;

    public TempSpell(CastingElements.Element element, float castingCost, float[] overCharge = null)
    {
        this.castingCost = castingCost;
        this.element = element;

        if (overCharge != null)
        {
            this.overCharge = overCharge;
        } else
        {
            this.overCharge = new float[0];
        }
    }

}
