using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public int projectileSpeed;

    public int maxRange;

    public ICast collisionBehaviour;

    public ICast maxRangeBehaviour;

    public Vector2 direction;

    float distanceTraveled = 0;

    public int damage;

    public SpellParameters spellParameters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveAmount = (direction * projectileSpeed) * Time.deltaTime;

        float maxMagnitude = maxRange - distanceTraveled;

        distanceTraveled += moveAmount.magnitude;

        transform.position = (Vector2) transform.position + Vector2.ClampMagnitude(moveAmount, maxMagnitude);

        if (distanceTraveled >= maxRange)
        {

            if (maxRangeBehaviour != null)
            {
                maxRangeBehaviour.Cast(spellParameters);
            }

            Destroy(gameObject);

        }


    }

    // called when the cube hits the floor
    void OnTriggerEnter(Collider collider)
    {
        bool hitSomething = false;
        MonoBehaviour[] list = collider.gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in list)
        {
            //TODO: may need to change or add objects that set off this trigger
            if (mb is ITakeDamage)
            {
               if(collisionBehaviour != null)
                {
                    spellParameters.targetHit = (ITakeDamage)mb;
                    collisionBehaviour.Cast(spellParameters);
                    spellParameters.targetHit = null;
                }
               hitSomething = true;
            }
        }

        if(hitSomething)
        {
            Destroy(this);
        }
    }

}
