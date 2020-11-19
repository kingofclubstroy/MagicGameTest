using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpellHandler : MonoBehaviour
{

    [SerializeField]
    public List<Spell> spells = new List<Spell>();

    public Spell GetSpell(string spellName)
    {
        foreach(Spell spell in spells)
        {
            if(spell.name == spellName)
            {
                return spell.Copy();
            }
        }

        return null;
    }
}
