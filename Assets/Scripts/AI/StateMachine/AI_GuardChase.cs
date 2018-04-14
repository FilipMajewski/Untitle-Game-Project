using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_GuardChase : AI_Base
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        StealthManager.isBreakingLaw = true;
        StealthManager.globaLookingForPlayer = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.destination = player.transform.position;
        agent.isStopped = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StealthManager.isBreakingLaw = false;
        StealthManager.globaLookingForPlayer = false;
    }

}
