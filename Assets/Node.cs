using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] List<Node> _neighbords = new List<Node>();
    int _x, _y;
    Grid _grid;
    public bool Block { get; private set; }
    public int Cost { get; private set; }

    private void Awake()
    {
        ManagerNodes.Subscribe(this);
    }

    private void OnDestroy()
    {
        ManagerNodes.Unsubscribe(this);
    }

    public void NewNeighbord(Node node)
    {
        if (_neighbords.Contains(node)) return;

        _neighbords.Add(node);
    }

    public List<Node> Neighbords
    {
        get
        {
            if(_neighbords.Count > 0)
                return _neighbords;

            Node right = _grid.GetNode(_x + 1, _y);

            if(right != null)
                _neighbords.Add(right);

            Node left = _grid.GetNode(_x - 1, _y);

            if (left != null)
                _neighbords.Add(left);

            Node down = _grid.GetNode(_x, _y - 1);

            if (down != null)
                _neighbords.Add(down);

            Node up = _grid.GetNode(_x, _y + 1);

            if (up != null)
                _neighbords.Add(up);

            return _neighbords;
        }
    }
    public void Initialize(Grid grid, int x, int y)
    {
        _grid = grid;
        _x = x;
        _y = y;

        UpdateCost(1);
    }

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //        GameManager.Instance.SetStartNode(this);

    //    if (Input.GetMouseButtonDown(1))
    //        GameManager.Instance.SetEndNode(this);

    //    //if (Input.GetKey(KeyCode.UpArrow))
    //    //    UpdateCost(Cost + 1);

    //    //if (Input.GetKey(KeyCode.DownArrow))
    //    //    UpdateCost(Cost - 1);

    //    if (Input.GetMouseButtonDown(2))
    //    {
    //        Block = !Block;

    //        if(Block)
    //            GetComponent<MeshRenderer>().material.color = Color.gray; 
    //        else
    //            GetComponent<MeshRenderer>().material.color = Color.white;    
    //    }           
    //}

    void UpdateCost(int newCost)
    {
        Cost = Mathf.Clamp(newCost, 1, 50);

        if(GetComponentInChildren<TextMeshProUGUI>() != null)
            GetComponentInChildren<TextMeshProUGUI>().text = Cost.ToString();   
    }

    private void OnDrawGizmos()
    {
       // Gizmos.DrawWireSphere(transform.position, ManagerNodes.minNeighbordDist);
    }
}
