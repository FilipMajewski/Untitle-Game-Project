using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_PedestrianCallGuards : AI_Base
{
    GameObject[] guards;

    private void Awake()
    {
        guards = GameObject.FindGameObjectsWithTag("AI_Guard");
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        CallGuards();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    void CallGuards()
    {
        GameObject nearestGuard = GetClosestGuard(guards);
        nearestGuard.GetComponent<AI_Setup>().sawPlayerAtThisPosition = player.transform.position;
        Animator anim = nearestGuard.GetComponent<Animator>();
        anim.SetBool("Called", true);
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
