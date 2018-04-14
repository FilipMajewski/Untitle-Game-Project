using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Guard_AI : MonoBehaviour
{
    public SO_AI_Parameters parameters;

    Animator anim;
    DrawFOV_CheckForPlayer fov;
    [HideInInspector]
    public Vector3 sawPlayerAtThisPosition;
    [HideInInspector]
    public float lookingRadius;
    [HideInInspector]
    public bool calledToSearchPlayer;
    public GameObject[] waypoints;
    NavMeshAgent agent;
    GameObject player;
    NavMeshHit hit;
    public float secondsForLooking;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<DrawFOV_CheckForPlayer>();
        lookingRadius = parameters.lookingRadius;
        calledToSearchPlayer = false;

        FeedNavAgent();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FeedNavAgent()
    {
        agent.speed = parameters.maxSpeed;
        agent.angularSpeed = parameters.turnSpeed;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    void GuardCalledByCamera()
    {
        agent.destination = sawPlayerAtThisPosition;
        calledToSearchPlayer = true;
        StealthManager.globaLookingForPlayer = true;

        if (agent.remainingDistance < agent.stoppingDistance || fov.seeYou)
        {
            anim.SetBool("Called", false);
        }
    }

    void GuardChase()
    {
        StealthManager.isBreakingLaw = true;
        StealthManager.globaLookingForPlayer = true;
        agent.destination = player.transform.position;
        agent.isStopped = false;
    }

    void GuardLooking()
    {
        agent.destination = player.transform.position;
        secondsForLooking = 15.0f;
        anim.SetFloat("LookingTime", secondsForLooking);
        fov.lookingForYou = true;
        StealthManager.globaLookingForPlayer = true;

        secondsForLooking -= Time.deltaTime;
        anim.SetFloat("LookingTime", secondsForLooking);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = RandomNavmeshLocation(lookingRadius);
        }

        secondsForLooking -= Time.deltaTime;
        anim.SetFloat("LookingTime", secondsForLooking);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = RandomNavmeshLocation(lookingRadius);
        }
    }


}
