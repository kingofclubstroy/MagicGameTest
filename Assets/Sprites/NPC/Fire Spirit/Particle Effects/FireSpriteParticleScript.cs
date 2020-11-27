using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpriteParticleScript : MonoBehaviour
{

    [SerializeField]
    ParticleSystem particleSystem;

    AIMovementHandler AIMovementHandler;
    ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule;


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
                //x = Mathf.Lerp(6f, 8f, dir.x);
                x = 6 + (2 * dir.x);

            } else
            {
                x = 6 - (2 * dir.x);
            }

           
        }

        forceOverLifetimeModule.x = x;
    }

    
}
