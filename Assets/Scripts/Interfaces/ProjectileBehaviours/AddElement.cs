using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Casting/AddElement")]
public class AddElement : ICast
{

    public override void Cast(SpellParameters spellParameters)
    {
        switch(spellParameters.element)
        {
            case Element.WATER:
                WaterControllerScript.instance.AddWater(spellParameters.activeProjectile.transform.position, spellParameters.elementAmount);
                break;
            case Element.FIRE:
                FireControllerScript.instance.AddFire(spellParameters.activeProjectile.transform.position);
                break;

            case Element.NATURE:
                CrawlController.instance.CreateCrawl(spellParameters.activeProjectile.transform.position, spellParameters.elementAmount, 3);
                break;
        }
    }

}
