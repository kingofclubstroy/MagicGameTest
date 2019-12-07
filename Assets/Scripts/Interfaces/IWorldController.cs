using System.Collections.Generic;
using UnityEngine;

public interface IWorldController
{
    List<TileScript> findNeighbours(Vector2 position);

    TileScript getTile(int x, int y);

    List<List<TileScript>> populateWorld(int x, int y);
}