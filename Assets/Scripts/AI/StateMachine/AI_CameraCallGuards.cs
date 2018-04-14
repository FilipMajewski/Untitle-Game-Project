using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_CameraCallGuards : AI_Base_NoNavMeshAgent
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        CallGuards();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CallGuards();
    }

    void CallGuards()
    {
        GameObject nearestGuard = GetClosestGuard(guards);
        nearestGuard.GetComponent<NavMeshAgent>().destination = player.transform.position;
        //nearestGuard.GetComponent<AI_Setup>().sawPlayerAtThisPosition = player.transform.position;
        Animator anim = nearestGuard.GetComponent<Animator>();
        anim.SetBool("Called", true);
    }

    void LookAtPlayer()
    {
        Vector3 targetPoint = new Vector3(player.transform.position.x, npc.transform.position.y,
            player.transform.position.z) - npc.transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);

        npc.transform.GetChild(0).rotation = Quaternion.Slerp(npc.transform.GetChild(0).rotation,
            targetRotation, Time.deltaTime * 2.0f);
    }

    GameObject GetClosestGuard(GameObject[] guards)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = npc.transform.position;
        foreach (GameObject potentialTarget in guards)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}
