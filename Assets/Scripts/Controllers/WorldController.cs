using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject tilePrefab;
    List<List<TileScript>> tileMap;
    float size;

    public static WorldController instance;

    // Start is called before the first frame update
    void Start()
    {
        size = tilePrefab.GetComponent<Renderer>().bounds.size.x;
        instance = this;
        populateWorld(40, 40);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void populateWorld(int x, int y)
    {

        tileMap = new List<List<TileScript>>();

        for (int i = 0; i < x; i++)
        {

            for (int j = 0; j < y; j++)
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

                if (i < 20 && j < 20)
                {
                    //the top left corner is on fire
                    tileScript.onFire = true;
                    tileScript.fire = Random.Range(0f, 20f);
                    tileScript.fuel = Random.Range(0f, 20f);
                }

                //if (random <= 1f)
                if(i >= x - 20 && j >= y - 20)  
                {
                    //80% chance the tile has fuel to burn but isnt on fire
                    tileScript.fuel = Random.Range(10f, 30f);
                    tileScript.growing = true;
                    
                } 
              
                //There is a 20% chance there is no fuel or fire to simulate bare ground


                tileMap[i].Add(tileScript);

            }

        }

    }




    List<TileScript> findNeighbours(Vector2 position)
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

    TileScript getTile(int x, int y)
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

    //spreads fire or growth from an active tile to its neighbours
    public bool updateNeighbouringTiles(TileScript tile)
    {

        List<TileScript> adjacentTiles = findNeighbours(tile.position);

        bool effectedNeighbour = false;

        if(tile.onFire) { 

            float heat = tile.heat;

            float totalHeatExchanged = 0;


            foreach (TileScript neighbour in adjacentTiles)
            {

                if (neighbour != null && !neighbour.onFire && neighbour.fuel > 0)
                {
                    effectedNeighbour = true;       
                    float difference = (heat - neighbour.heat) / 8;

                    if (difference > 0)
                    {

                        neighbour.heat += difference;
                        totalHeatExchanged += difference;

                    }
                            


                }

            }

            tile.heat -= totalHeatExchanged;

        } else if (tile.growing)
        {
               
            foreach (TileScript neighbour in adjacentTiles)
            {

                if (neighbour != null && !neighbour.onFire && !neighbour.growing)
                {
                    effectedNeighbour = true;
                    float random = Random.Range(0f, 100f);

                    if(random <= (neighbour.fuelGrowth * tile.fuel/10 * Mathf.Clamp(neighbour.neutrients/10, 1, 10) * Time.deltaTime))
                    {
                        neighbour.growing = true;
                    }
                        

                }

            }
                
        }

        //returns wheather the tile had an effect or not on its neighbours, will decide if the tile is still active or not
        return effectedNeighbour;
            
    }

    

}


