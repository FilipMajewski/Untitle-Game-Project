using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_CameraSweep : AI_Base_NoNavMeshAgent
{
    Quaternion currentRotation;

    private void Awake()
    {
        rotation = 270;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CameraSweep();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    void CameraSweep()
    {
        currentRotation = npc.transform.GetChild(0).localRotation;
        Quaternion targetRotation = Quaternion.AngleAxis(rotation, Vector3.up);

        if (currentRotation != targetRotation)
        {
            Quaternion newRotation = Quaternion.RotateTowards(
                             currentRotation,
                             targetRotation,
                             25 * Time.deltaTime);

            npc.transform.GetChild(0).localRotation = newRotation;

        }
        else
        {
            rotation *= -1;
            currentRotation = targetRotation;
            targetRotation = Quaternion.AngleAxis(rotation, Vector3.up);
            Debug.Log("Getting new rotation");
        }


    }


}
