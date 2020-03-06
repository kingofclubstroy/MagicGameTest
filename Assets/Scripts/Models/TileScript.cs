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

    Dictionary<Vector2, int> GrowthSpots;

    Texture2D texture;

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

    int pixelsGrown = 0;
    int mostGrowthPressure = 0;



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

        Texture2D mtexture = spriteRenderer.sprite.texture;

        texture = new Texture2D(mtexture.width, mtexture.height);
        texture.filterMode = FilterMode.Point;

        GrowthSpots = new Dictionary<Vector2, int>();

        flamability = 6f;
        fuelDensity = 1.5f;
        fuelGrowth = 10f;
        growthThreshold = 40f;
        //spriteUpdate();

        //remove this!!!
        SetWhite(texture);



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
            //spriteUpdate();
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

            if (fuel < 100)
            {

                if (fuel == 0 && GrowthSpots.Count == 0)
                {
                    GrowthSpots[new Vector2(0, 0)] = 1;
                    mostGrowthPressure = 1;
                }

                float fuelChange = (fuelGrowth + (fuelGrowth * (neutrients / 5f))) * Time.deltaTime;

                

                fuel += fuelChange;

                int growthToDraw = ((int)((texture.width * texture.height) * (fuel / 100f)) - pixelsGrown);

                

                for (int i = 0; i < growthToDraw; i++)
                {

                    List<Vector2> potentalPixels = new List<Vector2>();
                    int rand = Random.Range(1, mostGrowthPressure + 1);

                    
                    foreach (Vector2 key in GrowthSpots.Keys)
                    {
                        if (GrowthSpots[key] >= rand)
                        {

                            potentalPixels.Add(key);
                        }
                        
                    }



                    GrowPixel(potentalPixels[Random.Range(0, potentalPixels.Count)]);
                }

                pixelsGrown += growthToDraw;

                changed = true;
                ApplyTexture(texture);

            }

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

    void SetWhite(Texture2D tex)
    {
        for(int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                tex.SetPixel(x, y, Color.white);
            }
        }

        ApplyTexture(tex);
    }

    void ApplyTexture(Texture2D tex)
    {
        tex.Apply();

        spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1);

        spriteRenderer.material.mainTexture = tex as Texture;
        spriteRenderer.material.shader = Shader.Find("Sprites/Default");
    }

    void GrowPixel(Vector2 pixelPos)
    {
        GrowthSpots.Remove(pixelPos);

        texture.SetPixel((int)pixelPos.x, (int) pixelPos.y, Color.green);

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {

                int x = (int)pixelPos.x + i;
                int y = (int)pixelPos.y + j;

                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height && !(x == 0 && y == 0) && texture.GetPixel(x, y) == Color.white)
                {

                    Vector2 v = new Vector2(x, y);

                    if(GrowthSpots.ContainsKey(v))
                    {
                        GrowthSpots[v] += 1;
                    } else
                    {
                        GrowthSpots[v] = 1;
                    }

                }

            }
        }

        int best = 0;

        foreach(int val in GrowthSpots.Values)
        {
            if(val > best)
            {
                best = val;
            }
        }

        mostGrowthPressure = best;
    }


   

}
