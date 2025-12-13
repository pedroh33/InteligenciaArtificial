using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding 
{    
    public static List<Node> CalculateAStar(Node start, Node goal)
    {
        var frontier = new PriorityQueue<Node>(); //nueva Queue de nodos
        frontier.Enqueue(start, 0); // Nodo de inicio con costo

        var cameFrom = new Dictionary<Node, Node>(); //Para guardarme de donde vengo
        cameFrom.Add(start, null);

        var costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(start, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
            {
                List<Node> path = new List<Node>();

                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Reverse();
                return path;
            }
            foreach (var node in current.Neighbords)
            {
                if (node.Block) continue;

                float newCost = costSoFar[current] + node.Cost;
                newCost += Vector3.Distance(node.transform.position, goal.transform.position);

                if (!cameFrom.ContainsKey(node))
                {
                    frontier.Enqueue(node, newCost);
                    cameFrom.Add(node, current);
                    costSoFar.Add(node, newCost);
                }
                else if (costSoFar[node] > newCost)
                {
                    frontier.Enqueue(node, newCost);
                    cameFrom[node] = current;
                    costSoFar[node] = newCost;
                }
            }
        }

        return new List<Node>();
    }


    public static List<Node> CalculateTheta(Node start, Node goal)
    {
        var aStar = CalculateAStar(start, goal);

        int current = 0;

        while (current + 2 < aStar.Count)
        {
            if (GameManager.Instance.LineOfSight(aStar[current].transform.position, aStar[current + 2].transform.position))
                aStar.RemoveAt(current + 1);
            else
                current++;
        }

        return aStar;
    }

}
