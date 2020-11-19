using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICast : ScriptableObject, ICastBehaviour
{
    public abstract void Cast(SpellParameters spellParameters);
}
