using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class BoidFlock : Agent
{
    public static List<Agent> AllBoids;

    [Header("Movement")]
    [SerializeField] float _maxVelocity = 5f;
    [SerializeField] float _maxForce = 10f;

    Vector3 _velocity;
    Vector3 _acceleration;

    [Header("Flocking")]
    [SerializeField] float _radiusSeparation = 2f;
    [SerializeField] float _radiusDetect = 5f;

    public float weightSeparation = 2.5f;
    public float weightAlignment = 1.0f;
    public float weightCohesion = 1.0f;
    public float leaderWeight = 1.5f;

    [Header("Leader")]
    [SerializeField] Transform leader;
    [SerializeField] float leaderRadius = 15f;
    [SerializeField] public float leaderWeightValue;
    [SerializeField] public float weightAlignmentValue;
    [SerializeField] public float weightCohesionValue;
    [SerializeField] public float weightSeparationValue;



    /*  void OnEnable()
      {
          if (!AllBoids.Contains(this))
              AllBoids.Add(this);
      }

      void OnDisable()
      {
          AllBoids.Remove(this);
      }
    */
    protected override void Start()
    {
        AllBoids = GameManager.Instance.tipoTrueAgents;
        base.Start();
        _velocity = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized * _maxVelocity;

    }

    protected override void Update()
    {
        if (!HasHealth())
        {
            ApplyEscape(); 
            TraversePath();
            float distancia = Vector3.Distance(transform.position, cargadorVida.position);
            if (distancia < 3f)
            {
                health = 120;
                _isEscaping = false;
                _isGoing = false;
                _currentSpeed = _baseSpeed;
                return;
            }
            return; 
        }

        Move();
        Seek();
        Flocking();
        AddForce(FollowLeader(leader, leaderRadius) * leaderWeight);
        if (_seek && !_shooted)
        {
            _shooted = true;
            StartCoroutine(RaycastShootRoutine());
        }

     /*   float distancia = Vector3.Distance(transform.position, cargadorVida.position);
        if (distancia < 10f)
        {
            Debug.Log("Recargo vida");
            health = 120;
            _isGoing = false;
            _isEscaping = false;
            _currentSpeed = _baseSpeed;
            _velocity = transform.forward * _maxVelocity * 0.5f;
        }*/
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void AddForce(Vector3 force)
    {
        _acceleration += force; 
    }

    void Move()
    {

        Vector3 avoidance = ObstacleAvoidance(_velocity.normalized);
        AddForce(avoidance * 3f); // peso extra

        _velocity += _acceleration * Time.deltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);

        _velocity.y = 0f;
        transform.position += _velocity * Time.deltaTime;

        if (_velocity.sqrMagnitude > 0.0001f && !_seek)
            transform.forward = _velocity.normalized;

        _acceleration = Vector3.zero;
    }


    void Flocking()
    {
        AddForce(Separation(AllBoids, _radiusSeparation) * weightSeparation);
        AddForce(Alignment(AllBoids, _radiusDetect) * weightAlignment);
        AddForce(Cohesion(AllBoids, _radiusDetect) * weightCohesion);
        
    }
    Vector3 Separation(List<Agent> boids, float radius)
    {
        Vector3 desired = Vector3.zero;

        foreach (Agent boid in boids)
        {
            if (boid == this) continue;

            Vector3 dir = transform.position - boid.transform.position;
            float dist = dir.magnitude;

            if (dist > radius || dist <= 0.0001f) continue;

            desired += dir.normalized / dist;
        }

        if (desired == Vector3.zero)
            return Vector3.zero;

        desired = desired.normalized * _maxVelocity;

        Vector3 steering = desired - _velocity;
        return Vector3.ClampMagnitude(steering, _maxForce);
    }

    Vector3 Alignment(List<Agent> boids, float radius)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Agent boid in boids)
        {
            if (boid == this) continue;

            float dist = Vector3.Distance(transform.position, boid.transform.position);
            if (dist > radius) continue;

            sum += this._velocity;
            count++;
        }

        if (count == 0)
            return Vector3.zero;

        Vector3 desired = (sum / count).normalized * _maxVelocity;
        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _maxForce);
    }

    Vector3 Cohesion(List<Agent> boids, float radius)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Agent boid in boids)
        {
            if (boid == this) continue;

            float dist = Vector3.Distance(transform.position, boid.transform.position);
            if (dist > radius) continue;

            sum += boid.transform.position;
            count++;
        }

        if (count == 0)
            return Vector3.zero;

        Vector3 center = sum / count;
        Vector3 dir = center - transform.position;

        Vector3 desired = dir.normalized * _maxVelocity;
        desired *= dir.magnitude / radius;

        Vector3 steering = desired - _velocity;
        return Vector3.ClampMagnitude(steering, _maxForce);
    }

    Vector3 FollowLeader(Transform leader, float radius)
    {
        if (leader == null)
            return Vector3.zero;

        Vector3 dir = leader.position - transform.position;
        float dist = dir.magnitude;

        if (dist < radius)
            return Vector3.zero;

        Vector3 desired = dir.normalized * _maxVelocity;
        desired *= dist / radius;

        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _maxForce);
    }

   
}