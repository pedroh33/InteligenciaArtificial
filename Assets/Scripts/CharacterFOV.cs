using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFOV : MonoBehaviour
{
    public Transform target;
    public float viewRadius;
    public float viewAngle;
    Lider thisCharacter;
    public Enemy[] otherCharacter;
    [SerializeField] bool _call;

    private void Start()
    {
        thisCharacter = GetComponent<Lider>();
        this.transform.forward = thisCharacter.transform.forward;
    }

    void Update()
    {
        if (InFOV(target))
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            if (!_call)
            {
                foreach(var other in otherCharacter)
                {
                    other.SetPath(target.position);
                }
                thisCharacter.ApplySeek(target);
                _call = true;
            }
        }
        else
        {
            thisCharacter.DeactivateSeek(target);
            GetComponent<MeshRenderer>().material.color = Color.white;
            _call = false;
        }
        
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
