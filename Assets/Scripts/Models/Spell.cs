using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Spell
{

    [SerializeField]
    string name;

    [SerializeField]
    Sprite spellSprite;

    [SerializeField]
    int staminaCost;

    [SerializeField]
    int elementCost;

    [SerializeReference]
    CastingAbility ICast;

    [SerializeField]
    CastingElements.Element element;

    public void Cast()
    {
        ICast.Cast();
    }

}
