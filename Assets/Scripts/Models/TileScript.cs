using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    static int activeTiles = 0;

    [SerializeField]
    private float fireBuffer = 0.1f;

    public float size;

    private FireObject fireObject;

    static int numTiles;

    public int tileNumber;

    //Is fire the same as heat? like the same value, but only different visually and when spells require it?

    //TODO: remove hidden set functions, make explicit getters and setters and use those when we want extra functionality besides just setting

    [SerializeField]
    private float _fire;
    public float fire
    {
        get
        {
            return _fire;
        } set
        {

            _fire = value;
            
            if(fireObject != null)
            {
                fireObject.FireChanged(_fire);
            }
        }
    }

    [SerializeField]
    private GameObject firePrefab;

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
                isActive = true;
            } else
            {
                checkIfActive();
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

    private SpriteRenderer spriteRenderer;

    public Vector2 position;

    private bool changed;



    [SerializeField] bool isActive;
    
    private bool _onFire = false;

    [SerializeField]
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
                //WorldController.instance.setActive(this);
                isActive = true;
                growing = false;
                SetOnFire();
            } else
            {
               
                checkIfActive();
                
            } 

        }
    }

    
    // Start is called before the first frame update
    void Start()
    {

        numTiles += 1;

        tileNumber = numTiles;

        heat = 0;


        spriteRenderer = GetComponent<SpriteRenderer>();

        flamability = 6f;
        fuelDensity = 1.5f;
        fuelGrowth = 10f;
        growthThreshold = 40f;
        spriteUpdate();



    }

    // Update is called once per frame
    void Update()
    {

        
        changed = false;
        fireUpdate();
        growthUpdate();
        updateNeighbours();

        if (changed)
        {
            spriteUpdate();
        }
        
    }


    void fireUpdate()
    {
        if (onFire)
        {

            if (fuel <= 0)
            {
              
                // LOOKAT: this is an arbituary number, going to change in the future
                fire -= 50 * Time.deltaTime;
                changed = true;

                if (fire <= 0)
                {
                    fire = 0;
                    onFire = false;

                    //now that the tile is not on fire, check neighbours to make active if they should be
                    foreach(TileScript t in WorldController.instance.findNeighbours(this.position))
                    {
                        //TODO: test this
                        if(t != null && t.isActive == false)
                        {
                            t.checkIfActive();
                        }
                    }
                }
                
            }
            else
            {

                float heatDifference = fire - heat;

                if (heatDifference > 0)
                {
                    heat += heatDifference * Time.deltaTime;
                }

                //TODO: this is all abrituary and needs to be adjusted
                fire += 1 * Time.deltaTime * fuelDensity;

                //LOOKAT: Does fuel burn at a constant rate, or does it depend on the amount of fire on the tile
                fuel -= 1 * Time.deltaTime;

                //LOOKAT: Are neutrients tied to fuel consumption as a 1:1 relationship, or does that depend on something, do neutrients even get created? Do they spread around?
                neutrients += 1 * Time.deltaTime;

                if (fire >= 100)
                {
                    fire = 100;
                }

                changed = true;
                

            }

            


        }
        else 
        {
            if (heat > 0)
            {
                if (fuel > 0)
                {
                    if (heat >= ignition)
                    {
                        onFire = true;
                        changed = true;
                    }
                    
                }

                heat -= 1 * Time.deltaTime;
                if(heat < 0)
                {
                    heat = 0;
                }
                
            }
            

        }

    }

    void growthUpdate()
    {
        if(growing && !onFire)
        {

            fuel += (fuelGrowth + (fuelGrowth * (neutrients / 5f))) * Time.deltaTime;
            changed = true;

        }
    }

    void updateNeighbours()
    {
        if(isActive)
        {
            //need to spread the fire or growth to neighbouring tiles
            isActive = updateNeighbouringTiles(this);
        }
    }


    void spriteUpdate()
    {

        Color mColor = spriteRenderer.color;


        if (onFire)
        {
            
           // mColor.r = Mathf.Lerp(0.1f, 1, fire / 70);
            mColor.g = 0;
            mColor.b = 0;
            //mColor = new Color(Mathf.Lerp(0.4f, 1, fire/70), 0, 0);

            mColor = new Color(0, Mathf.Lerp(0.1f, 1f, fuel / 100), 0);

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

        spriteRenderer.color = mColor;

    }

    public void checkIfActive()
    {
        if(onFire || growing)
        {
            isActive = true;
        } else
        {
            isActive = false;
        }
    }

    //spreads fire or growth from an active tile to its neighbours
    public bool updateNeighbouringTiles(TileScript tile)
    {

        List<TileScript> adjacentTiles = WorldController.instance.findNeighbours(tile.position);

        bool effectedNeighbour = false;

        if (tile.onFire)
        {

            float heat = tile.heat;

            float totalHeatExchanged = 0;


            foreach (TileScript neighbour in adjacentTiles)
            {

                if (neighbour != null && !neighbour.onFire && neighbour.fuel > 0)
                {
                    effectedNeighbour = true;
                    float difference = (heat - neighbour.heat) * Time.deltaTime;

                    if (difference > 0)
                    {

                        neighbour.heat += difference;
                        totalHeatExchanged += difference;

                    }



                }

            }

            tile.heat -= totalHeatExchanged;

        }
        else if (tile.growing)
        {

            foreach (TileScript neighbour in adjacentTiles)
            {

                if (neighbour != null && !neighbour.onFire && !neighbour.growing)
                {
                    effectedNeighbour = true;
                    float random = Random.Range(0f, 100f);

                    if (random <= (neighbour.fuelGrowth * tile.fuel / 10 * Mathf.Clamp(neighbour.neutrients / 10, 1, 10) * Time.deltaTime))
                    {
                        neighbour.growing = true;

                        //now that this tile is growing, tell neighbouring tiles to be active if they are on fire
                        foreach(TileScript t in WorldController.instance.findNeighbours(neighbour.position))
                        {
                            if(t != null && t.isActive == false && t.onFire)
                            {
                                t.isActive = true;
                            }
                        }
                    }

                }

            }

        }

        //returns wheather the tile had an effect or not on its neighbours, will decide if the tile is still active or not
        return effectedNeighbour;

    }

    public void setSprite(Sprite sprite)
    {

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (sprite != null)
        {

            spriteRenderer.sprite = sprite;

        }

    }


    public void SetOnFire()
    {

        if (fireObject == null)
        {
            fireObject = new FireObject(transform.position, firePrefab);
        }


    }

   


    /// <summary>
    /// This will decide if fire will be spawned and what size the fire will be
    /// </summary>
    private void spawnFire(float fireValue)
    {

       

    }

}
