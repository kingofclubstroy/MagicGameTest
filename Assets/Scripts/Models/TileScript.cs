using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    //Is fire the same as heat? like the same value, but only different visually and when spells require it?

    public float fire;

    private float _fuel;
    public float fuel
    {
        get
        {
            return _fuel;
        } set
        {
            if (value >= 100)
            {
                _fuel = 100;
            } else
            {
                _fuel = value;
            }
        }
    }

    private bool _growing;
    public bool growing
    {
        get
        {
            return _growing;
        } set
        {
            _growing = value;

            if(_growing)
            {
                WorldController.instance.setActive(this);
            } else
            {
                WorldController.instance.setInactive(this);
            }
        }
    }
    
    public float heat;
    public float growthThreshold;

    public float fuelGrowth;

    public float flamability = 100f;
    public float ignition = 100f;

    public float fireHeatTransfer = 0f;
    public float heatSpread = 0.25f;
    public float fuelToFireConversion = 0f;
    public float heatLoss = 0f;

    public float neutrients = 0f;

    public float fuelDensity = 3f;

    private SpriteRenderer sprite;

    public Vector2 position;


    private bool _onFire = false;
    public bool onFire
    {
        get
        {
            return _onFire;
        }
        set
        {
            _onFire = value;

            if(value)
            {
                WorldController.instance.setActive(this);
            } else
            {
                WorldController.instance.setInactive(this);
            } 

        }
    }

    
    // Start is called before the first frame update
    void Start()
    {

        heat = 0;


        sprite = GetComponent<SpriteRenderer>();

        flamability = 6f;
        fuelDensity = 1.5f;
        fuelGrowth = 20f;
        growthThreshold = 40f;



    }

    // Update is called once per frame
    void Update()
    {
        fireUpdate();
        growthUpdate();
        spriteUpdate();
        
    }


    void fireUpdate()
    {
        if (onFire)
        {

            if (fuel <= 0)
            {
              
                // this is an arbituary number, going to change in the future
                fire -= 20 * Time.deltaTime;

                if (fire <= 0)
                {
                    fire = 0;
                    onFire = false;
                }
                
            }
            else
            {

                // is heat the same as fire when an object is on fire?
                heat = fire;

                fire += 1 * Time.deltaTime * fuelDensity;
                fuel -= 1 * Time.deltaTime;
                neutrients += 1 * Time.deltaTime;

                if (fire >= 100)
                {
                    fire = 100;
                }
                

            }

            


        }
        else 
        {
            if (fuel > 0)
            {
                if (heat > 0)
                {

                    //onFire = true;
                    heat -= Time.deltaTime;
                    fuel += fuelGrowth * Time.deltaTime;

                    if (heat >= ignition)
                    {
                        onFire = true;
                    }
                    else
                    {

                        float random = Random.Range(0f, 100f);
                        if (random <= (heat * flamability * Time.deltaTime))
                        {
                            onFire = true;
                        }
                    }
                }
            }

        }

    }

    void growthUpdate()
    {
        if(growing && !onFire)
        {

            fuel += fuelGrowth + (neutrients / 100f) * Time.deltaTime;

        }
    }


    void spriteUpdate()
    {

        Color mColor;

        if (onFire)
        {

            mColor = new Color(Mathf.Lerp(0.4f, 1, fire/70), 0, 0);

        } else if(fuel <= 0)
        {

            if (neutrients > 0)
            {
                mColor = new Color(0.5f / neutrients, 0.5f / neutrients, 0.5f / neutrients);
            }
            else
            {

                mColor = new Color(0.4f, 0.37058823529f, 0.37058823529f);

            }
            

        } else
        {
            mColor = new Color(0, Mathf.Lerp(0.1f, 1f, fuel/100), 0); 
        } 

        sprite.color = mColor;

    }

}
