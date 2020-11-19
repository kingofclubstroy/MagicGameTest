using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Custom/Casting/ICastProjectile")]
class ICastProjectile : ICast
{
    public override void Cast(SpellParameters spellParameters)
    {

        Vector2 dir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)spellParameters.positionToCast).normalized;

        GameObject projectile = Instantiate(spellParameters.projectileObject, spellParameters.positionToCast, Quaternion.identity);

        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();

        spellParameters.activeProjectile = projectile;

        projectileScript.spellParameters = spellParameters;

        projectileScript.collisionBehaviour = spellParameters.collisionBehaviour;

        projectileScript.maxRangeBehaviour = spellParameters.maxRangeBehaviour;

        projectileScript.projectileSpeed = spellParameters.projectileSpeed;

        projectileScript.direction = dir;

        projectileScript.maxRange = spellParameters.maxRange;

    }
}
