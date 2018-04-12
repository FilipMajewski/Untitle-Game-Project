using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Setup : MonoBehaviour
{
    public SO_AI_Parameters parameters;

    Animator anim;
    DrawFOV_CheckForPlayer fov;
    NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        fov = GetComponent<DrawFOV_CheckForPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("SeePlayer", fov.seeYou);
        anim.SetBool("SeeLawBreaking", fov.seeThatYouBreakinLaw);
    }

    void FeedNavAgent()
    {
        agent.speed = parameters.maxSpeed;
        agent.angularSpeed = parameters.turnSpeed;
    }
}
