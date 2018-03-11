using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerFootsteps : MonoBehaviour
{

    [EventRef]
    public string walkSound;
    [EventRef]
    public string runSound;

    bool playerIsWalking;
    bool playerIsRunning;

    float walkingSpeed = 0.4f;
    float runningSpeed = 0.6f;

    PlayerController pc;

    void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        InvokeRepeating("CallFootsteps", 0, 0.5f);
    }

    void Update()
    {
        walkingSpeed = pc.move.magnitude;
        //Debug.Log(pc.move.magnitude);

        if (pc.move.magnitude >= 0.1f && pc.move.magnitude <= 0.5f && !pc.falling)
        {
            playerIsWalking = true;
            playerIsRunning = false;
        }
        else if (pc.move.magnitude >= 0.5f && !pc.falling)
        {
            playerIsRunning = true;
            playerIsWalking = false;
        }
        else
        {
            playerIsWalking = false;
            playerIsRunning = false;
            Debug.Log("Player is not moving");
        }
    }

    void CallFootsteps()
    {
        if (playerIsWalking == true)
        {
            Debug.Log("Player is moving");
            RuntimeManager.PlayOneShot(walkSound);

        }

        if (playerIsRunning == true)
        {
            Debug.Log("Player is Running");
            RuntimeManager.PlayOneShot(runSound);
        }
    }

    void OnDisable()
    {
        playerIsWalking = false;
        playerIsRunning = false;
    }
}
