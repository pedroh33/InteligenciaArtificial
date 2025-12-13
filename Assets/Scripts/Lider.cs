using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Lider : MonoBehaviour
{

    [SerializeField] Camera cam;
    List<Vector3> _path = new List<Vector3>();
    public float _speed = 4f;
    public bool liderA;
    bool _seek;
    Transform _target;
    bool _shooted;
    float rateOfFire = 3f;

    [SerializeField] int _maxHealth;
    private int _currentHealth;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
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
        if (_seek)
        {

            if (_target == null)
                return;

            var seekDir = _target.position - transform.position;

            if (seekDir.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(seekDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);

                
            }

            transform.position += seekDir.normalized * _speed * Time.deltaTime;

            
            return;
        }
    }


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
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
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
