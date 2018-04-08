using UnityEngine;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    Transform player;
    Investigate_Behavior investigate_;
    NavMeshAgent agent;


    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float rotSpeed = 5.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        investigate_ = GetComponent<Investigate_Behavior>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {

    }

    [Task]
    public void PickDestination(float x, float z)
    {
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-61, 49), 0, Random.Range(-104, -134));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void TargetPlayer()
    {
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task]
    bool Turn(float angle)
    {
        var p = transform.position + Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        target = p;
        return true;
    }

    [Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.LookRotation(direction),
                                                Time.deltaTime * rotSpeed);

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}",
                Vector3.Angle(transform.forward, direction));

        if (Vector3.Angle(transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool SeePlayer()
    {
        return investigate_.seeYou;
    }

    [Task]
    public bool InDanger(float minDist)
    {
        Vector3 distance = player.transform.position - transform.position;
        return (distance.magnitude < minDist);
    }

    [Task]
    public void TakeCover()
    {
        Vector3 awayFromPlayer = transform.position - player.transform.position;
        Vector3 dest = transform.position + awayFromPlayer * 2;
        agent.SetDestination(dest);
        Task.current.Succeed();
    }


    [Task]
    public void SetTargetDestination()
    {
        agent.SetDestination(target);
        Task.current.Succeed();
    }


}

