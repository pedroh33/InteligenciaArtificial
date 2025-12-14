using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] int _maxHealth;
    private int _currentHealth;
    List<Vector3> _path = new List<Vector3>();
    public GameObject muzzleFlash;
    public GameObject sangre;
    public float health;
    public float _baseSpeed = 4f;
    public float _currentSpeed;
    public float _maxSpeed = 8f;

    Transform _target;
    [SerializeField] bool _seek;
    [SerializeField] float _radiusArrive;

    [SerializeField] bool _shooted;
    float rateOfFire = 3f;
    [SerializeField] float _stopDistance = 1.2f;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] float rayDistance = 20f;
    [SerializeField] LayerMask enemyLayer;

    public Transform firepoint;
    [Header("Obstacle Avoidance")]
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float avoidDistance = 2f;
    [SerializeField] float avoidStrength = 2f;
    [SerializeField] float avoidRadius = 0.6f;

    public Transform cargadorVida;
    bool _isEscaping;
    bool _isGoing;


    public void SetPath(Vector3 positionEnd)
    {
        Node start = ManagerNodes.GetNode(transform.position);
        Node end = ManagerNodes.GetNode(positionEnd);

        var path = Pathfinding.CalculateTheta(start, end);


        _path = new List<Vector3>();

        foreach (var t in path)
            _path.Add(t.transform.position);

        _path.Add(positionEnd);
    }
    public void TraversePath()
    {
        if (_path.Count <= 0)
        {
            return;
        }
        var dir = _path[0] - transform.position;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        Vector3 moveDir = dir.normalized;
        Vector3 avoidance = ObstacleAvoidance(moveDir);

        Vector3 finalDir = (moveDir + avoidance).normalized;

        transform.position += finalDir * _currentSpeed * Time.deltaTime;

        if (dir.magnitude <= 0.3f)
            _path.RemoveAt(0);
    }
    Vector3 ObstacleAvoidance(Vector3 moveDir)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, avoidRadius, moveDir, out RaycastHit hit, avoidDistance, obstacleMask))
        {
            Vector3 dirToObstacle = hit.point - transform.position;
            float angle = Vector3.SignedAngle(transform.forward, dirToObstacle, Vector3.up);

            Vector3 avoidDir = angle >= 0 ? -transform.right : transform.right;

            Debug.DrawRay(origin, avoidDir * avoidDistance, Color.yellow);

            return avoidDir.normalized * avoidStrength;
        }

        return Vector3.zero;
    }



    public void ApplySeek(Transform target)
    {

        _target = target;
        _seek = true;
    }
    public void Seek()
    {
        if (!_seek || _target == null)
            return;

        Vector3 seekDir = _target.position - transform.position;

        Vector3 flatDir = new Vector3(seekDir.x, 0f, seekDir.z);

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 4 * Time.deltaTime);
        }

        float distance = seekDir.magnitude;

        if (distance > _stopDistance)
        {
            transform.position += flatDir.normalized * _currentSpeed * Time.deltaTime;
        }
    }
    public void DeactivateSeek(Transform target)
    {
        _seek = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("tiro");
        }
    }


    IEnumerator RaycastShootRoutine()
    {
        while (_seek)
        {
            ShootRaycast();
            yield return new WaitForSeconds(rateOfFire);
        }

        _shooted = false;
    }
    void ShootRaycast()
    {
        Ray ray = new Ray(firepoint.position, firepoint.forward);
        RaycastHit hit;
        StartCoroutine(MuzzleFlash());

        if (Physics.Raycast(ray, out hit, rayDistance, enemyLayer))
        {
            Debug.Log($"Raycast impactó a: {hit.collider.name}");
            Agent enemigoImpactado = hit.collider.GetComponent<Agent>();

            if (enemigoImpactado != null && enemigoImpactado != this)
            {
                enemigoImpactado.Daniar(20);
            }
        }

        Debug.DrawRay(firepoint.position, firepoint.forward * rayDistance, Color.red, 0.5f);
    }

    public IEnumerator MuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        muzzleFlash.SetActive(false);
    }
    public IEnumerator Sangrar()
    {
        sangre.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        sangre.SetActive(false);
    }
    public void Daniar(int cantidad)
    {
        health -= cantidad;
        StartCoroutine(Sangrar());
        Debug.Log($"{name} recibió {cantidad} de daño. Vida actual: {health}");

        if (health <= 0)
        {
            Debug.Log($"{name} murió");
        }
    }

    public bool HasHealth()
    {
        if (health >= 45)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GoGetHealth()
    {
        SetPath(cargadorVida.position);
        _currentSpeed = _maxSpeed;
    }

    public void ApplyEscape()
    {
        _isEscaping = true;

        if (_isEscaping && !_isGoing)
        {
            GoGetHealth();
            _isGoing = true;
        }
    }



    private void Start()
    {
        _currentHealth = _maxHealth;
        muzzleFlash.SetActive(false);
        _shooted = false;
        sangre.SetActive(false);

        _currentSpeed = _baseSpeed;
    }


   protected virtual void Update()
    {
        if (HasHealth())
        {
            Seek();

            if (_seek && !_shooted)
            {
                _shooted = true;
                StartCoroutine(RaycastShootRoutine());
            }
        }
        else if (!HasHealth() && !_isEscaping)
        {
            ApplyEscape();

        }

        TraversePath();


        //Esto es para recargar la vida cuando llega a un punto de recarga
        float distancia = Vector3.Distance(transform.position, cargadorVida.position);

        if (distancia < 3f)
        {
            health = 120;
            _isGoing = false;
            _isEscaping = false;
            _currentSpeed = _baseSpeed;
        }
    }

}

    //public Vector3 Arrive(Vector3 target)
    //{

    //    Vector3 desired = (target - transform.position);
    //    var distance = desired.magnitude;

    //    if (desired.magnitude > _radiusArrive)
    //    {
    //        return Seek(target);
    //    }

    //    desired.Normalize();
    //    desired *= _maxSpeed * (distance / _radiusArrive);

    //    Vector3 steer = desired - _velocity;
    //    steer = Vector3.ClampMagnitude(steer, _maxSpeed);
    //    return steer;
    //}

    //public void LookAtEnemy()
    //{
    //    Agent nearest = null;
    //    var distance = Mathf.Infinity;
    //    foreach (var target in GameManager.instance.boids)
    //    {
    //        if (!target.isActiveAndEnabled) continue;
    //        float dis = Vector3.Distance(target.transform.position, transform.position);
    //        if (dis <= radiusLook && dis <= distance)
    //        {
    //            distance = dis;
    //            nearest = target;
    //        }
    //    }
    //    if (nearest != null)
    //    {
    //        transform.forward = Vector3.Lerp(transform.forward, (nearest.transform.position - transform.position).normalized, Time.deltaTime * 2);
    //        StartCoroutine(ShootBullet());
    //    }
    //}