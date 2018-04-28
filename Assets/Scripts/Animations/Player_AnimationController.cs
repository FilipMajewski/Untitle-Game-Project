using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimationController : MonoBehaviour
{

    public Transform lHand;
    public Transform rHand;

    AnimatorStateInfo animationCurrentState;
    AnimatorTransitionInfo transitionInfo;
    PlayerController playerController;

    private Animator anim;
    public Animator Anim
    {
        get
        {
            return anim;
        }

    }

    // Use this for initialization
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        animationCurrentState = anim.GetCurrentAnimatorStateInfo(0);
        transitionInfo = anim.GetAnimatorTransitionInfo(0);

        anim.SetFloat("Speed", Vector3.SqrMagnitude(playerController.Move));
        anim.SetBool("Falling", playerController.Falling);
        anim.SetFloat("FallingTime", playerController.FallingTime);

        if (animationCurrentState.IsName("Hard Landing (1)"))
        {
            playerController.CurrentSpeed = Mathf.Lerp(0, playerController.maxMoveSpeed, transitionInfo.duration);
            playerController.FallingTime = 0;
        }
        else if (animationCurrentState.IsName("Landing to Run") || animationCurrentState.IsName("Landing to Idle"))
        {
            playerController.CurrentSpeed = Mathf.Lerp(playerController.maxMoveSpeed / 2, playerController.maxMoveSpeed, transitionInfo.duration);
            playerController.FallingTime = 0;
        }
        else
        {
            playerController.CurrentSpeed = playerController.maxMoveSpeed;
        }

    }







}
