using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EventCallbacks;

public class WorldController : MonoBehaviour, IWorldController
{
    public GameObject tilePrefab;
    List<List<TileScript>> tileMap;
    float size;

    public static WorldController instance;

    [SerializeField]
    private int height;

    [SerializeField]
    private int width;


    // Start is called before the first frame update
    void Start()
    {
        size = tilePrefab.GetComponent<Renderer>().bounds.size.x;
        instance = this;
        tileMap = populateWorld(width, height);

        // We want to know whenever a unit dies
        UnitDeathEvent.RegisterListener(UnitDied);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public List<List<TileScript>> populateWorld(int x, int y)
    {

        List<List<TileScript>> tileMap = new List<List<TileScript>>();

        for (int j = 0; j < x; j++)
        {

            for (int i = 0; i < y; i++)
            {

                if (j == 0)
                {
                    tileMap.Add(new List<TileScript>());
                }

                GameObject tile = Instantiate(tilePrefab, new Vector2(transform.position.x + i * size, transform.position.y - j * size), Quaternion.identity);

                TileScript tileScript = tile.GetComponent<TileScript>();

                tileScript.position = new Vector2(i, j);

                float random = Random.Range(0f, 1f);

                tileScript.flamability = Random.Range(0.1f, 10f);

                if (i < x/2 && j < y/2)
                {
                    //the top left corner is on fire
                    tileScript.onFire = true;
                    tileScript.neutrients = Random.Range(0f, 100f);
                    tileScript.fuel = Random.Range(0f, 20f);
                }

                //if (random <= 1f)
                else if(i >= x/2 && j >= y/2)  
                {
                    //80% chance the tile has fuel to burn but isnt on fire
                    tileScript.fuel = Random.Range(10f, 30f);
                    tileScript.growing = true;
                    
                } else
                {
                    tileScript.neutrients = Random.Range(0f, 20f);
                }
              
                //There is a 20% chance there is no fuel or fire to simulate bare ground


                tileMap[i].Add(tileScript);

            }

        }

        return tileMap;

    }




    public List<TileScript> findNeighbours(Vector2 position)
    {

        //need to find the adjacent neighbours ie: north, east, south, west, northWest, northEast, southEast, southWest 
        List<TileScript> adjacentList = new List<TileScript>();

        int x = (int) position.x;
        int y = (int)position.y;

        //North tile
        adjacentList.Add(getTile(x, y - 1));

        //East
        adjacentList.Add(getTile(x + 1, y));

        //South
        adjacentList.Add(getTile(x, y + 1));

        //West
        adjacentList.Add(getTile(x - 1, y));

        //NorthWest
        //adjacentList.Add(getTile(x - 1, y - 1));

        //NorthEast
        //adjacentList.Add(getTile(x + 1, y - 1));

        //SouthEast
        //adjacentList.Add(getTile(x + 1, y + 1));

        //SouthWest
        //adjacentList.Add(getTile(x - 1, y + 1));

        return adjacentList;

    }

    public TileScript getTile(int x, int y)
    {

        if(x < 0 || x >= tileMap.Count || y < 0 || y >= tileMap[x].Count)
        {
            //Tile is out of range, so doesnt exist.
            return null;
        } else
        {
            return tileMap[x][y];
        }

    }

    TileScript getTileFromPosition(float x, float y)
    {
        
        //get the index value of the tile
        int x_pos = Mathf.FloorToInt(x / size);

        int y_pos = Mathf.FloorToInt(-y / size);

        if( x_pos < 0 || x_pos >= width || y_pos < 0 || y_pos >= height)
        {
            //The position is out of index, so no tile there

            Debug.Log($"out of position, x_pos = {x_pos}, y_pos = {y_pos}");

            return null;
        }

        else
        {
            return getTile(x_pos, y_pos);
        }

        
    }

    /// <summary>
    /// Allows a damagable object to get the tile it is under and potentially take damage from it //TODO: it wont only be a damagable object, there will be may ways to interact in the future
    /// I.E. Maybe the tile or object on the tile heals you or maybe you can ask to use elemnts around the tile and we want to split this up
    /// </summary>
    /// <param name="damagableObject">Object interacting with the tiled world</param>
    /// <param name="position">Position the object is in with respect to the world </param>
    /// 
    public void interactWithTile(ITakeDamage damagableObject, Vector3 position) 
    {

        TileScript currentTile = getTileFromPosition(position.x, position.y);

        if(currentTile != null)
        {

            if(currentTile.onFire)
            {
                // Tile is on fire so lets do some fire damage
                //TODO: figure out how much damage the user takes, maybe there is fire resistance or somehthing
                damagableObject.TakeDamage(1);

            }

        }

    }



    void UnitDied(UnitDeathEvent unitDeathEvent)
    {
        Debug.Log("Unit died");
    }

}


