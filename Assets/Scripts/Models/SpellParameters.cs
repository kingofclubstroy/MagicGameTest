using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SpellParameters
{
    public int damage;

    public int fireStrength;

    public int waterStrength;

    public int earthStrength;

    public int natureStrength;

    public CastingUIController.Element element;

    public ICast castBehaviour;

    public GameObject projectileObject;

    public ICast collisionBehaviour;

    public ICast maxRangeBehaviour;
   
}
