using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class AI_Behavior : MonoBehaviour
{
    NavMeshAgent agent;
    Investigate_Behavior investigate_;

    public Transform path;
    int waypointNumber;
    public bool loop;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        investigate_ = GetComponent<Investigate_Behavior>();
        waypointNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    Vector3 FindNextWaypoint()
    {
        Vector3 waypoint = path.GetChild(waypointNumber).position;
        if (waypointNumber > path.childCount)
        {
            if (!loop)
            {
                Destroy(gameObject);
            }
            else
            {
                waypointNumber = 0;
            }
        }
        return waypoint;
    }

    [Task]
    public void PickDestination()
    {
        Vector3 dest = FindNextWaypoint();
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            waypointNumber++;
            Task.current.Succeed();
        }
    }

    [Task]
    bool SeePlayer()
    {
        return investigate_.seeYou;
    }


}
