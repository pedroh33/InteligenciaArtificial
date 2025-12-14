using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ManagerNodes 
{
    static List<Node> _totalNodes = new List<Node>();
    public static float minNeighbordDist = 15;
    public static void CompleteNeighbords()
    {
        foreach (var node in _totalNodes)
        {
            foreach(var otherNode in _totalNodes)
            {
                if (node == otherNode) continue;

                if (GameManager.Instance.LineOfSight(node.transform.position, otherNode.transform.position))
                    if(Vector3.Distance(node.transform.position, otherNode.transform.position) <= minNeighbordDist)
                        node.NewNeighbord(otherNode);
            }
        }
    }

    public static void Subscribe(Node node)
    {
        if(_totalNodes.Contains(node)) return;

        _totalNodes.Add(node);
    }

    public static void Unsubscribe(Node node)
    {
        if (!_totalNodes.Contains(node)) return;

        _totalNodes.Remove(node);
    }

    public static Node GetNode(Vector3 position)
    {
        float minDist = Mathf.Infinity;
        Node minNode = null;
        Debug.Log("GetNode");
        foreach (Node node in _totalNodes)
        {
            var currentDist = Vector3.Distance(node.transform.position, position);
            
            if (currentDist < minDist)
            {
                minDist = currentDist;
                minNode = node;
            }
        }

        return minNode;
    }
}
