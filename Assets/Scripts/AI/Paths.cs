using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{

    public bool loop;

    private void Start()
    {

    }

    private void OnDrawGizmos()
    {
        Transform pathHolder = transform;

        Vector3 startingPosition = transform.GetChild(0).position;
        Vector3 previousPosition = startingPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        if (loop)
        {
            Gizmos.DrawLine(previousPosition, startingPosition);
        }
    }
}
