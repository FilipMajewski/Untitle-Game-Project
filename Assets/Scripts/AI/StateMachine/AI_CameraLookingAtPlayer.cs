using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_CameraLookingAtPlayer : AI_Base_NoNavMeshAgent
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 targetPoint = new Vector3(player.transform.position.x, npc.transform.position.y,
            player.transform.position.z) - npc.transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);

        npc.transform.GetChild(0).rotation = Quaternion.Slerp(npc.transform.GetChild(0).rotation,
            targetRotation, Time.deltaTime * 2.0f);

        if (StealthManager.GlobaLookingForPlayer)
        {
            animator.SetBool("SeeLawBreaking", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
