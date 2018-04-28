using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_AnimationController : MonoBehaviour
{

    NavMeshAgent agent;
    Animator anim;
    AI_Setup setup;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        setup = GetComponent<AI_Setup>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", agent.speed / setup.ChaseSpeed);
    }
}
