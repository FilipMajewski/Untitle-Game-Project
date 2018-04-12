using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Base_NoNavMeshAgent : StateMachineBehaviour
{
    public GameObject npc;
    public DrawFOV_CheckForPlayer fov;

    public GameObject player;

    public float rotation;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        npc = animator.gameObject;
        fov = npc.GetComponent<DrawFOV_CheckForPlayer>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
