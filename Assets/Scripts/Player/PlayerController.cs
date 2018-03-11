using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed;
    public float rotationSmoothSpeed;

    [HideInInspector]
    public bool falling;
    int weapon = 1;
    bool block;
    CharacterController cc;
    Animator anim;

    float horizontal, vertical, fallingTime;

    [HideInInspector]
    public float currentSpeed;
    [HideInInspector]
    public Vector3 move;

    AnimatorStateInfo animationCurrentState;
    AnimatorTransitionInfo transitionInfo;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        currentSpeed = maxMoveSpeed;
        falling = false;
        fallingTime = 0;
    }


    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (!cc.isGrounded)
        {
            fallingTime += Time.deltaTime;
        }
        else
        {
            fallingTime = 0;
        }

        if (fallingTime >= 0.5f && !cc.isGrounded)
        {
            falling = true;
        }
        else
        {
            falling = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            //ChangeWeapon();
        }

        if (Input.GetButton("Fire1"))
        {
            block = true;
        }
        else
        {
            block = false;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            anim.SetTrigger("Attack");
        }

        Movement(horizontal, vertical);

        UpdateAnimator();

    }

    void Movement(float horizontal, float vertical)
    {
        Transform cam = Camera.main.transform;

        Vector3 camForward = Vector3.Scale(cam.up, new Vector3(1, 0, 1)).normalized;

        move = vertical * camForward + horizontal * cam.right;

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        if (move.magnitude <= 0.1f)
        {
            move = Vector3.zero;
        }

        if (move.magnitude != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), rotationSmoothSpeed);
        }

        cc.SimpleMove(move * currentSpeed);
    }

    void UpdateAnimator()
    {
        animationCurrentState = anim.GetCurrentAnimatorStateInfo(0);
        transitionInfo = anim.GetAnimatorTransitionInfo(0);

        anim.SetFloat("Speed", Vector3.SqrMagnitude(move));
        anim.SetBool("Falling", falling);
        anim.SetFloat("FallingTime", fallingTime);

        if (animationCurrentState.IsName("Hard Landing (1)"))
        {
            currentSpeed = Mathf.Lerp(0, maxMoveSpeed, transitionInfo.duration);
            fallingTime = 0;
        }
        else if (animationCurrentState.IsName("Landing to Run") || animationCurrentState.IsName("Landing to Idle"))
        {
            currentSpeed = Mathf.Lerp(maxMoveSpeed / 2, maxMoveSpeed, transitionInfo.duration);
            fallingTime = 0;
        }
        else
        {
            currentSpeed = maxMoveSpeed;
        }

        anim.SetInteger("Weapon", weapon);
        anim.SetBool("Block", block);
    }

    void ChangeWeapon()
    {
        weapon++;
        if (weapon > 1)
        {
            weapon = 0;
        }
    }
}

