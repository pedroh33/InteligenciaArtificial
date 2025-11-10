using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node prefab;
    public int width, height;
    public float offset;
    Node[,] _grid;

    void Start()
    {
        _grid = new Node[width, height];

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = Instantiate(prefab);
                _grid[x, y] = node;
                node.transform.position = new Vector3(x, 0, y) * offset;
                node.Initialize(this, x, y);
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if(x >= width ||  y >= height || x < 0 || y < 0) 
            return null;

        return _grid[x, y];
    }

}
