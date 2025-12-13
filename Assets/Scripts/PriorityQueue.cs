using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue <T>
{
    Dictionary<T, float> _allElements = new Dictionary<T, float>();

    public int Count { get { return _allElements.Count; } }
    public void Enqueue(T element, float cost)
    {
        if(!_allElements.ContainsKey(element))
            _allElements.Add(element, cost);
        else
            _allElements[element] = cost;
    }

    public T Dequeue()
    {
        T minElem = default;
        float minCost = Mathf.Infinity;

        foreach(var item in _allElements)
        {
            if(item.Value < minCost)
            {
                minElem = item.Key;
                minCost = item.Value;
            }
        }

        _allElements.Remove(minElem);
        return minElem;
    }
}
