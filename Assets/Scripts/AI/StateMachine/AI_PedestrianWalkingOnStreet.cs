using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_PedestrianWalkingOnStreet : AI_Base
{
    int currentWaypoint;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (waypoints.Length == 0)
            return;

        agent.destination = waypoints[0].transform.position;
        agent.isStopped = false;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                DestroyPedestrianAndSpawnNew();
            }

            agent.destination = waypoints[currentWaypoint].transform.position;
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    void DestroyPedestrianAndSpawnNew()
    {
        Destroy(npc);
    }


}
