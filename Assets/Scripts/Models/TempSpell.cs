using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpell
{

    public Element element;

    public float castingCost;

    private float[] overCharge;

    public SpellType spellType;

    public float[] OverCharge { get => overCharge; set => overCharge = value; }

    public TempSpell(Element element, float castingCost, float[] overCharge = null, SpellType spellType = SpellType.PROJECTILE)
    {
        this.castingCost = castingCost;
        this.element = element;
        this.spellType = spellType;

        if (overCharge != null)
        {
            this.OverCharge = overCharge;
        } else
        {
            this.OverCharge = new float[0];
        }
    }

    public enum SpellType {

        PROJECTILE,
        SPAWNED

    }


}
