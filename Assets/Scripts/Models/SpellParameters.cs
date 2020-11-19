using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SpellParameters
{
    public int staminaCost;

    public int elementCost;

    public float castTime;

    public int damage;

    public int fireStrength;

    public int waterStrength;

    public int earthStrength;

    public int natureStrength;

    public Element element;

    public ICast castBehaviour;

    public GameObject projectileObject;

    public int projectileSpeed;

    public int maxRange;

    public ICast collisionBehaviour;

    public ICast maxRangeBehaviour;

    public GameObject activeProjectile;

    public ICast updateBehaviour;

    public Vector2 positionToCast;

    public ITakeDamage targetHit;

    public int maxCrawl;

    public float growthRate;

    public int elementAmount;

}
