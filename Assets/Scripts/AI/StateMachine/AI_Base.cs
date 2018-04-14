using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Base : StateMachineBehaviour
{
    [HideInInspector]
    public GameObject npc;
    [HideInInspector]
    public DrawFOV_CheckForPlayer fov;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public NavMeshHit hit;
    [HideInInspector]
    public GameObject[] waypoints;
    [HideInInspector]
    public float lookingRadius;
    [HideInInspector]
    public bool calledToSearchPlayer;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        npc = animator.gameObject;
        waypoints = npc.GetComponent<AI_Setup>().waypoints;
        lookingRadius = npc.GetComponent<AI_Setup>().lookingRadius;
        fov = npc.GetComponent<DrawFOV_CheckForPlayer>();
        agent = npc.GetComponent<NavMeshAgent>();
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
