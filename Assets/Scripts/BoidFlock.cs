using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class BoidFlock : MonoBehaviour
{
    public static List<BoidFlock> AllBoids = new List<BoidFlock>();

    [Header("Movement")]
    [SerializeField] float _maxVelocity = 5f;
    [SerializeField] float _maxForce = 10f;

    Vector3 _velocity;
    Vector3 _acceleration;

    [Header("Flocking")]
    [SerializeField] float _radiusSeparation = 2f;
    [SerializeField] float _radiusDetect = 5f;

    [SerializeField] float weightSeparation = 2.5f;
    [SerializeField] float weightAlignment = 1.0f;
    [SerializeField] float weightCohesion = 1.0f;

    [Header("Leader")]
    [SerializeField] Transform leader;
    [SerializeField] float leaderRadius = 15f;
    [SerializeField] float leaderWeight = 1.5f;

    void OnEnable()
    {
        if (!AllBoids.Contains(this))
            AllBoids.Add(this);
    }

    void OnDisable()
    {
        AllBoids.Remove(this);
    }

    void Start()
    {
        _velocity = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized * _maxVelocity;
    }

    void Update()
    {
        Flocking();
        Move();
    }

    void AddForce(Vector3 force)
    {
        _acceleration += force;
    }

    void Move()
    {
        _velocity += _acceleration * Time.deltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);

        transform.position += _velocity * Time.deltaTime;

        if (_velocity.sqrMagnitude > 0.0001f)
            transform.forward = _velocity.normalized;

        _acceleration = Vector3.zero;
    }
    void Flocking()
    {
        AddForce(Separation(AllBoids, _radiusSeparation) * weightSeparation);
        AddForce(Alignment(AllBoids, _radiusDetect) * weightAlignment);
        AddForce(Cohesion(AllBoids, _radiusDetect) * weightCohesion);
        AddForce(FollowLeader(leader, leaderRadius) * leaderWeight);
    }
    Vector3 Separation(List<BoidFlock> boids, float radius)
    {
        Vector3 desired = Vector3.zero;

        foreach (BoidFlock boid in boids)
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

    Vector3 Alignment(List<BoidFlock> boids, float radius)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (BoidFlock boid in boids)
        {
            if (boid == this) continue;

            float dist = Vector3.Distance(transform.position, boid.transform.position);
            if (dist > radius) continue;

            sum += boid._velocity;
            count++;
        }

        if (count == 0)
            return Vector3.zero;

        Vector3 desired = (sum / count).normalized * _maxVelocity;
        Vector3 steering = desired - _velocity;

        return Vector3.ClampMagnitude(steering, _maxForce);
    }

    Vector3 Cohesion(List<BoidFlock> boids, float radius)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (BoidFlock boid in boids)
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

        if (dist > radius)
            return Vector3.zero;

        Vector3 desired = dir.normalized * _maxVelocity;
        desired *= dist / radius;

        Vector3 steering = desired - _velocity;
        return Vector3.ClampMagnitude(steering, _maxForce);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radiusSeparation);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radiusDetect);

        if (leader != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, leaderRadius);
        }
    }
}