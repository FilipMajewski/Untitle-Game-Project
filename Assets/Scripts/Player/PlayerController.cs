﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed;
    public float rotationSmoothSpeed;

    CharacterController cc;
    Animator anim;
    Interaction inter;

    private Vector3 move;

    private bool falling, crouched;
    private float horizontal, vertical, fallingTime, currentSpeed;

    #region Encapsulation
    public float FallingTime
    {
        get
        {
            return fallingTime;
        }
        set
        {
            FallingTime = value;
        }

    }

    public bool Falling
    {
        get
        {
            return falling;
        }

    }

    public float CurrentSpeed
    {
        get
        {
            return currentSpeed;
        }
        set
        {
            currentSpeed = value;
        }

    }

    public Vector3 Move
    {
        get
        {
            return move;
        }

    }

    public bool Crouched
    {
        get
        {
            return crouched;
        }

        set
        {
            crouched = value;
        }
    }

    #endregion

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        inter = GetComponent<Interaction>();
        currentSpeed = maxMoveSpeed;
        falling = false;
        crouched = false;
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

        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Crouched");
            crouched = !crouched;
            StealthManager.IsBreakingLaw = !StealthManager.IsBreakingLaw;
        }

        if (!inter.Hiden)
        {
            Movement(horizontal, vertical);
        }

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

        if (move.magnitude <= 0.3f)
        {
            move = Vector3.zero;
        }

        if (move.magnitude != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), rotationSmoothSpeed);
        }

        if (crouched)
        {
            cc.SimpleMove(move * currentSpeed / 2);
        }
        else
        {
            cc.SimpleMove(move * currentSpeed);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }


}

