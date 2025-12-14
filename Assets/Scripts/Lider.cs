using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Lider : MonoBehaviour
{
    public float health;
    [SerializeField] Camera cam;
    public bool liderA;
    List<Vector3> _path = new List<Vector3>();

    [SerializeField] int _maxHealth;
    private int _currentHealth;

    public float _speed = 4f;
    public float _maxSpeed = 8f;

    Transform _target;
    [SerializeField] bool _seek;
    [SerializeField] float _radiusArrive;

    [SerializeField] bool _shooted;
    float rateOfFire = 3f;
    [SerializeField] float _stopDistance = 1.2f;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;

    public Transform firepoint;
    public void ClickPosition()
    {
        if (Input.GetMouseButtonDown(0) && liderA)
        {
            GetMousePosition();
        }
        if (Input.GetMouseButtonDown(1) && !liderA)
        {
            GetMousePosition();
        }
    }

    public void GetMousePosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 punto = hit.point;
            Debug.Log("Click en: " + punto);
            SetPath(punto);
            return;
        }
    }

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

        transform.position += dir.normalized * _speed * Time.deltaTime;

        if (dir.magnitude <= 0.3f)
            _path.RemoveAt(0);
    }


    public void ApplySeek(Transform target)
    {

        _target = target;
        _seek = true;
    }

    public void DeactivateSeek(Transform target)
    {
        _seek = false;
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
            transform.position += flatDir.normalized * _speed * Time.deltaTime;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("tiro");
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


    IEnumerator ShootRoutine()
    {
        while (!_shooted)
        {
            Shoot();
            yield return new WaitForSeconds(rateOfFire);
        }

        _shooted = false;
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firepoint.transform.position, Quaternion.identity);
        projectile.transform.forward = transform.forward;
        //Rigidbody rb = projectile.AddComponent<Rigidbody>();
        //rb.useGravity = false;
        //rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);

        _shooted = true;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;

        _shooted = false;
    }


    void Update()
    {
        ClickPosition();
        TraversePath();
        Seek();

        if (_seek && !_shooted)
        {
            StartCoroutine(ShootRoutine());
        }

    }
}
