using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSprite : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    private float sizeMultiplier;

    private float startFire = 0f;

    public Vector3 position;

    private FireObject fireObject;


    public void SetupFire(float fireStart, float sizeMultiplier, FireObject fireObject)
    {

        startFire = fireStart;

        this.sizeMultiplier = sizeMultiplier;

        this.fireObject = fireObject;

    }


    public void fireChanged(float fire)
    {

        float adjustedFire = ((fire - startFire) * sizeMultiplier);

        if (adjustedFire < 0)
        {
            //PutOutFire();
        }
        else
        {

            animator.SetFloat("Fire", (fire - startFire) * sizeMultiplier);

        }

    }

    public void PutOutFire()
    {

        fireObject.FireIsOut(this);

        Destroy(gameObject);

    }



}
