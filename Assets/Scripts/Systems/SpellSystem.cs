using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellSystem", menuName = "ScriptableObjects/SpellSystem", order = 1)]
public class SpellSystem : ScriptableObject
{

    [SerializeField]
    Spell[] spells;


}
