using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PriorityQueue<T>
{

    //object array
    List<Node> queue = new List<Node>();
    int heapSize = -1;
    bool _isMinPriorityQueue;
    public int Count { get { return queue.Count; } }

    /// <summary>
    /// If min queue or max queue
    /// </summary>
    /// <param name="isMinPriorityQueue"></param>
    public PriorityQueue(bool isMinPriorityQueue = true)
    {
        _isMinPriorityQueue = isMinPriorityQueue;
    }



    //public void UpdatePriority(T obj, int priority) {...}
    //public bool IsInQueue(T obj) {...}



    /// <summary>
    /// Maintain max heap
    /// </summary>
    /// <param name="i"></param>
    private void BuildHeapMax(int i)
    {
        while (i >= 0 && queue[(i - 1) / 2].priority < queue[i].priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }
    /// <summary>
    /// Maintain min heap
    /// </summary>
    /// <param name="i"></param>
    private void BuildHeapMin(int i)
    {
        while (i >= 0 && queue[(i - 1) / 2].priority > queue[i].priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }


    private void MaxHeapify(int i)
    {
        int left = ChildL(i);
        int right = ChildR(i);

        int heighst = i;

        if (left <= heapSize && queue[heighst].priority < queue[left].priority)
            heighst = left;
        if (right <= heapSize && queue[heighst].priority < queue[right].priority)
            heighst = right;

        if (heighst != i)
        {
            Swap(heighst, i);
            MaxHeapify(heighst);
        }
    }
    private void MinHeapify(int i)
    {
        int left = ChildL(i);
        int right = ChildR(i);

        int lowest = i;

        if (left <= heapSize && queue[lowest].priority > queue[left].priority)
            lowest = left;
        if (right <= heapSize && queue[lowest].priority > queue[right].priority)
            lowest = right;

        if (lowest != i)
        {
            Swap(lowest, i);
            MinHeapify(lowest);
        }
    }

    private void Swap(int i, int j)
    {
        var temp = queue[i];
        queue[i] = queue[j];
        queue[j] = temp;
    }
    private int ChildL(int i)
    {
        return i * 2 + 1;
    }
    private int ChildR(int i)
    {
        return i * 2 + 2;
    }

    /// <summary>
    /// Enqueue the object with priority
    /// </summary>
    /// <param name="priority"></param>
    /// <param name="obj"></param>
    public void Enqueue(Node obj)
    {

        queue.Add(obj);
        heapSize++;
        //Maintaining heap
        if (_isMinPriorityQueue)
            BuildHeapMin(heapSize);
        else
            BuildHeapMax(heapSize);
    }

    /// <summary>
    /// Dequeue the object
    /// </summary>
    /// <returns></returns>
    public Node Dequeue()
    {
        if (heapSize > -1)
        {
            var returnVal = queue[0];
            queue[0] = queue[heapSize];
            queue.RemoveAt(heapSize);
            heapSize--;
            //Maintaining lowest or highest at root based on min or max queue
            if (_isMinPriorityQueue)
                MinHeapify(0);
            else
                MaxHeapify(0);
            return returnVal;
        }
        else
            throw new Exception("Queue is empty");
    }

    /// <summary>
    /// Updating the priority of specific object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="priority"></param>
    public void UpdatePriority(Node obj)
    {
        int i = 0;
        for (; i <= heapSize; i++)
        {
            Node node = queue[i];
            if (object.ReferenceEquals(node, obj))
            {

                if (_isMinPriorityQueue)
                {
                    BuildHeapMin(i);
                    MinHeapify(i);
                }
                else
                {
                    BuildHeapMax(i);
                    MaxHeapify(i);
                }
            }
        }
    }
    /// <summary>
    /// Searching an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool IsInQueue(Node obj)
    {
        foreach (Node node in queue)
            if (node == obj)
                return true;
        return false;
    }
}
