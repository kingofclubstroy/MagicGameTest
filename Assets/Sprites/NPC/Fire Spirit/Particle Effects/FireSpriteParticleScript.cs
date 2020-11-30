using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpriteParticleScript : MonoBehaviour
{

    [SerializeField]
    ParticleSystem particleSystem;

    AIMovementHandler AIMovementHandler;
    ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule;

    [SerializeField]
    float particleTransform;

    Direction LastDirection;


    // Start is called before the first frame update
    void Start()
    {
        AIMovementHandler = GetComponentInParent<AIMovementHandler>();

        forceOverLifetimeModule = particleSystem.forceOverLifetime;

    }

    // Update is called once per frame
    void Update()
    {

        float x;

        if (AIMovementHandler.isIdle) {

            x = 6;

        } else
        {
            Vector2 dir = AIMovementHandler.GetTargetDirection();

            if(dir.x <= 0)
            {
               
                x = 6 + (3 *  -dir.x);
                Vector3 pos = transform.localPosition;
                pos.x = particleTransform;
                transform.localPosition = pos;

            } else
            {
                x = -6 - (3 * dir.x);
                Vector3 pos = transform.localPosition;
                pos.x = - particleTransform;
                transform.localPosition = pos;
            }
           
        }

        forceOverLifetimeModule.x = x;
    }

    
}
