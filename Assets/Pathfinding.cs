using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding 
{    
    public static List<Node> CalculateAStar(Node start, Node goal)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<Node, Node>();
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
}
