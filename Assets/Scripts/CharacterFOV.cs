using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFOV : MonoBehaviour
{
    private Transform target;
    
    public float viewRadius;
    public float radiusLookTarget = 7;
    public float viewAngle;
    Agent thisCharacter;
    public Enemy[] otherCharacter;
    [SerializeField] bool _call;
    private bool _hasTarget;

    private void Start()
    {
        thisCharacter = GetComponent<Agent>();
        this.transform.forward = thisCharacter.transform.forward;
    }

    void Update()
    {
       





        //if (target != null)
        //{




        //    //if (InFOV(target) && _hasTarget)
        //    //{
        //    //    GetComponent<MeshRenderer>().material.color = Color.red;
        //    //    if (!_call)
        //    //    {
        //    //        foreach(var other in otherCharacter)
        //    //        {
        //    //            other.SetPath(target.position);
        //    //        }
        //    //        thisCharacter.ApplySeek(target);
        //    //        _call = true;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    thisCharacter.DeactivateSeek(target);
        //    //    GetComponent<MeshRenderer>().material.color = Color.white;
        //    //    _call = false;
        //    //}
        //}
        
    }

    public bool InFOV(Transform obj)
    {
        var dir = obj.position - transform.position;

        if(dir.magnitude <= viewRadius)
        {
            if(Vector3.Angle(transform.forward, dir) <= viewAngle * 0.5f)
                return GameManager.Instance.LineOfSight(transform.position, obj.position);

        }

        return false;
    }

    public void LookAtEnemy()
    {
        Agent nearest = null;
        var distance = Mathf.Infinity;


        foreach (var targetAgent in GameManager.Instance.agents)
        {
            if (!targetAgent.isActiveAndEnabled) continue;

            float dis = Vector3.Distance(targetAgent.transform.position, transform.position);
            if (dis <= radiusLookTarget && dis <= distance )
            {
                distance = dis;
                nearest = targetAgent;
            }
        }
        if (nearest != null)
        {
            transform.forward = Vector3.Lerp(transform.forward, (nearest.transform.position - transform.position).normalized, Time.deltaTime * 2);
            target = nearest.transform;
            _hasTarget = true;
        }
        else
        {
            _hasTarget = false;
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }
}
