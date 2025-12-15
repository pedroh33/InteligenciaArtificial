using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    Node _start, _end;
    public Enemy player;
    [SerializeField] LayerMask _mask;

    //public List<Agent> agents = new List<Agent>();
    public List<Agent> tipoTrueAgents = new List<Agent>();
    public List<Agent> tipoFalseAgents = new List<Agent>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ManagerNodes.CompleteNeighbords();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _end != null)
        {
            player.SetPath(_end.transform.position);
        }
    }

    public void SetStartNode(Node start)
    {
        if (_start != null)
            _start.GetComponent<MeshRenderer>().material.color = Color.white;

        _start = start;

        _start.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void SetEndNode(Node end)
    {
        if (_end != null)
            _end.GetComponent<MeshRenderer>().material.color = Color.white;

        _end = end;

        _end.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public bool LineOfSight(Vector3 start, Vector3 end)
    {
        var dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, _mask);
    }
}

