using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 9f;
    [SerializeField] float lifeTime = 2f;

    Rigidbody rb;


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();


        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            //rb.velocity += transform.forward * projectileSpeed * Time.fixedDeltaTime;
            rb.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Npc"))
        {
            Destroy(gameObject);
        }
    }

}
