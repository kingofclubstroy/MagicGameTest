using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Casting/GrowCrawlUnder")]
public class GrowCrawlUnder : ICast
{
    public override void Cast(SpellParameters spellParameters)
    {
        Vector2 pos = spellParameters.activeProjectile.transform.position;

        CrawlController.instance.CreateCrawl(pos, spellParameters.maxCrawl, spellParameters.growthRate); 

    }
}
