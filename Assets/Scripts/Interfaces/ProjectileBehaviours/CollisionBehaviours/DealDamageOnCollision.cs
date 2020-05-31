using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Casting/DealDamageOnCollision")]
public class DealDamageOnCollision : ICast
{

    public override void Cast(SpellParameters spellParameters)
    {
        spellParameters.targetHit.TakeDamage(spellParameters.damage);
    }

}
