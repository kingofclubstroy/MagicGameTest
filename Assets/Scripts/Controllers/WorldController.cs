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

}


