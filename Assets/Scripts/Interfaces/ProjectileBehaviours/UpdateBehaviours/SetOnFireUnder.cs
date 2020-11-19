using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Casting/SetOnFireUnder")]
public class SetOnFireUnder : ICast
{
    public override void Cast(SpellParameters spellParameters)
    {
        Vector2 pos = spellParameters.activeProjectile.transform.position;

        CrawlController.instance.AddFire(pos);

    }
}
