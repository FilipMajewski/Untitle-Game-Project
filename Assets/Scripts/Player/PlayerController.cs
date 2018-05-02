using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed;
    public float rotationSmoothSpeed;
    public bool haveDiferentCamera;
    CharacterController cc;
    Animator anim;
    Interaction inter;
    ChangeCameraView cameraView;
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

        if (haveDiferentCamera)
        {
            cameraView = GetComponent<ChangeCameraView>();
        }

        currentSpeed = maxMoveSpeed;
        falling = false;
        crouched = false;
        fallingTime = 0;
    }


    void Update()
    {
        var inputDevice = InputManager.ActiveDevice;

        horizontal = inputDevice.LeftStickX;
        vertical = inputDevice.LeftStickY;

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

        if (inputDevice.Action1.WasPressed)
        {
            anim.SetTrigger("Crouched");
            crouched = !crouched;
        }

        if (haveDiferentCamera)
        {
            if (cameraView.IsTopdownCameraActive || inter.Hiden)
            {
                Movement(0, 0);
            }
            else
            {
                Movement(horizontal, vertical);
            }
        }
        else
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

}

