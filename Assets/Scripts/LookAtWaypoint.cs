using UnityEngine;

public class LookAtWaypoint : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float rotationSpeed = 10f;

    int index;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 direction = waypoints[index].position - transform.position;
        direction.y = 0; 

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, waypoints[index].position) <= 0.3f)
        {
            index++;
            if (index >= waypoints.Length)
                index = 0;
        }
    }
}