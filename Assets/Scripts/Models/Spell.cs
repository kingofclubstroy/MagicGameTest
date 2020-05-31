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

    [SerializeReference]
    public ICast castingBehaviour;

    [SerializeField]
    Element element;

    [SerializeField]
    public SpellParameters spellParams;

    [SerializeField]
    public SpellTypes spellType;

    public Color color;

    public void Cast()
    {
        castingBehaviour.Cast(spellParams);
    }

}
