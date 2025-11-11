using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed;
    List<Vector3> _path = new List<Vector3>();
    [SerializeField] Transform[] _waypointsPatrol;
    int _indexPatrol;
    public void SetPath(Vector3 positionEnd)
    {
        Node start = ManagerNodes.GetNode(transform.position);
        Node end = ManagerNodes.GetNode(positionEnd);

        var path = Pathfinding.CalculateAStar(start, end);


        _path = new List<Vector3>();

        foreach (var t in path)
            _path.Add(t.transform.position);

        _path.Add(positionEnd);
    }

    void Update()
    {
       
        if (_path.Count <= 0) 
        {
            if(!GameManager.Instance.LineOfSight(transform.position, _waypointsPatrol[_indexPatrol].transform.position))
            {
                SetPath(_waypointsPatrol[_indexPatrol].transform.position);
                return;
            }
            var d = _waypointsPatrol[_indexPatrol].transform.position - transform.position;
            
            if (d.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(d);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
            }
            
            transform.position += d.normalized * _speed * Time.deltaTime;

            if(d.magnitude <= 0.2f)
            {
                _indexPatrol++;

                if (_indexPatrol >= _waypointsPatrol.Length)
                    _indexPatrol = 0;
            }

            return;
        }
        var dir = _path[0] - transform.position;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        transform.position += dir.normalized * _speed * Time.deltaTime;

        if(dir.magnitude <= 0.3f)
            _path.RemoveAt(0);
    }
}
