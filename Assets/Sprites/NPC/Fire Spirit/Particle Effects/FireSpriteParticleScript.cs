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
        Vector3 pos;
        pos.x = (float)3.7;
        pos.y = (float)12.8;
        pos.z = (float)1;


        if (AIMovementHandler.isIdle) {

            x = 6;

            pos.x = (float) 3.7;

            particleSystem.startSpeed = (float) 4.75;

        } else
        {
            Vector2 dir = AIMovementHandler.GetTargetDirection();

            if(dir.x <= 0)
            {
                //x = Mathf.Lerp(6f, 8f, dir.x);
                x = 6 + (5 *  -dir.x);

                pos.x = (float) 3.7;

                particleSystem.startSpeed = (float)7;

            } else
            {
                x = -6 - (5 * dir.x);

                pos.x = (float) -3.7;

                particleSystem.startSpeed = (float)7;
            }
           
        }

        forceOverLifetimeModule.x = x;

        particleSystem.transform.localPosition = pos;

    }

    
}
