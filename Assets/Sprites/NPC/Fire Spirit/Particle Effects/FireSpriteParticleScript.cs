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
        Vector3 pos;
        pos.x = 3.7f;
        pos.y = 12.8f;
        pos.z = 0f;


        if (AIMovementHandler.isIdle) {

            x = 6;

            pos.x = 3.7f;

            particleSystem.startSpeed = 4.75f;

        } else
        {
            Vector2 dir = AIMovementHandler.GetTargetDirection();

            if(dir.x <= 0)
            {

                x = 6 + (5 *  -dir.x);

                pos.x = 3.7f;

                particleSystem.startSpeed = 7f;

            } else
            {
                x = -6 - (5 * dir.x);

                pos.x = -3.7f;

                particleSystem.startSpeed = 7f;

            }
           
        }

        forceOverLifetimeModule.x = x;

        particleSystem.transform.localPosition = pos;

    }

    
}
