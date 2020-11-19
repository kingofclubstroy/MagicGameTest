using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IComparable<Node>
{
    public Vector2 position;
    public Node parent;
    public int priority;

    public List<Node> neignbours;

    public Node(Vector2 position, Node parent)
    {
        this.parent = parent;
        this.position = position;
    }

    public int CompareTo(Node other)
    {
        if (this.priority < other.priority) return -1;
        else if (this.priority > other.priority) return 1;
        else return 0;
    }

    public List<Node> GetNeighbours()
    {
        List<Node> neighbours = new List<Node>();

        //need to make a node for each direction including diagionals
        //TODO: need to handle edge cases, ie off the map situations

        //west
        neighbours.Add(new Node(new Vector2(position.x - 1, position.y), this));
        //northwest
        neighbours.Add(new Node(new Vector2(position.x - 1, position.y + 1), this));
        //north
        neighbours.Add(new Node(new Vector2(position.x, position.y + 1), this));
        //northeast
        neighbours.Add(new Node(new Vector2(position.x + 1, position.y + 1), this));
        //east
        neighbours.Add(new Node(new Vector2(position.x + 1, position.y), this));
        //southeast
        neighbours.Add(new Node(new Vector2(position.x + 1, position.y - 1), this));
        //south
        neighbours.Add(new Node(new Vector2(position.x, position.y - 1), this));
        //southwest
        neighbours.Add(new Node(new Vector2(position.x - 1, position.y - 1), this));

        return neighbours;

    }



}
