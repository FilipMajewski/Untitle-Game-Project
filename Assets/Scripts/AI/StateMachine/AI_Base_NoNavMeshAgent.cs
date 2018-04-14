using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Base_NoNavMeshAgent : StateMachineBehaviour
{
    [HideInInspector]
    public GameObject npc;
    [HideInInspector]
    public DrawFOV_CheckForPlayer fov;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject[] guards;
    [HideInInspector]
    public float rotation;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        guards = GameObject.FindGameObjectsWithTag("AI_Guard");
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
