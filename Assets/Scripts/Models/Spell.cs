using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Spell
{

    [SerializeField]
    public string name;

    [SerializeField]
    Sprite spellSprite;

    [SerializeField]
    int staminaCost;

    [SerializeField]
    int elementCost;

    [SerializeReference]
    public ICast castingBehaviour;

    [SerializeField]
    CastingUIController.Element element;

    [SerializeField]
    public SpellParameters spellParams;

    public void Cast()
    {
        castingBehaviour.Cast(spellParams);
    }

}
