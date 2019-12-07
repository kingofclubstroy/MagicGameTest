using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;


public class world_controller_tests
{
    // A Test behaves as an ordinary method
    [Test]
    public void get_tile_recieves_tile()
    {

        //ARRANGE
        IWorldController worldControler = Substitute.For<IWorldController>();

        //ACT
        List<TileScript> list = worldControler.findNeighbours(new Vector2(0, 0));

        //ASSERT
        Assert.AreEqual(null, list[0]);


    }

        
}

