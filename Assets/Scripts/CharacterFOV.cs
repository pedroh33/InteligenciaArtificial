using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFOV : MonoBehaviour
{
    private Transform target;

    [Header("FOV")]
    public float viewRadius = 10f;
    public float radiusLookTarget = 7f;
    public float viewAngle = 90f;

    Agent thisCharacter;
    public Enemy[] otherCharacter;

    [SerializeField] bool _call;
    private bool _hasTarget;
    BoidFlock thisBoid;
    private void Start()
    {  
        thisCharacter = GetComponent<Agent>();
        if(thisCharacter.TryGetComponent<BoidFlock>(out var boidFlock))
        {
            thisBoid = boidFlock;
            Debug.Log("Instancio");
        }
        
        transform.forward = thisCharacter.transform.forward;
    }

    void Update()
    {
        LookAtEnemy();

        if (target == null)
        {
            thisCharacter.DeactivateSeek(null);
            GetComponent<MeshRenderer>().material.color = Color.white;
            _call = false;
            return;
        }

        if (InFOV(target) && _hasTarget)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;

            if (!_call)
            {
                foreach (var other in otherCharacter)
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

    void LookAtEnemy()
    {
        Agent nearest = null;
        float minDistance = Mathf.Infinity;

        List<Agent> enemyList = GetEnemyList();

        foreach (var agent in enemyList)
        {
            if (!agent.isActiveAndEnabled) continue;
            if (agent == thisCharacter) continue;

            Vector3 dir = agent.transform.position - transform.position;
            float distance = dir.magnitude;

            if (distance > radiusLookTarget) continue;

            if (Vector3.Angle(transform.forward, dir) > viewAngle * 0.5f) continue;
            if (!GameManager.Instance.LineOfSight(transform.position, agent.transform.position)) continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = agent;
            }
        }

        if (nearest != null)
        {
            target = nearest.transform;
            _hasTarget = true;

            if (thisBoid != null) {
                thisBoid.leaderWeight = 0f;
            }         
            transform.forward = Vector3.Lerp(
                transform.forward,
                (nearest.transform.position - transform.position).normalized,
                Time.deltaTime * 2f
            );
        }
        else
        {
            target = null;
            _hasTarget = false;
            if (thisBoid != null)
            {
                thisBoid.leaderWeight = 1.5f;
            }
        }
    }

    List<Agent> GetEnemyList()
    {
        return thisCharacter.tipo
            ? GameManager.Instance.tipoFalseAgents
            : GameManager.Instance.tipoTrueAgents;
    }

    public bool InFOV(Transform obj)
    {
        Vector3 dir = obj.position - transform.position;

        if (dir.magnitude <= viewRadius)
        {
            if (Vector3.Angle(transform.forward, dir) <= viewAngle * 0.5f)
            {
                return GameManager.Instance.LineOfSight(transform.position, obj.position);
            }
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