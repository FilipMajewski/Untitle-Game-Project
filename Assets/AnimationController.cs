using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public Transform lHand;
    public Transform rHand;

    bool pickupAnimation;

    AnimatorStateInfo animationCurrentState;
    AnimatorTransitionInfo transitionInfo;
    Animator anim;

    PlayerController playerController;

    GameObject toPickup;
    GameObject inHand;

    // Use this for initialization
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();

        anim.SetInteger("Weapon", playerController.Weapon);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Throw();
        }
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


        anim.SetBool("Block", playerController.Block);
    }

    public void Grab(GameObject interactionObject)
    {
        //play grab animation
        anim.SetTrigger("Pickup");
        toPickup = interactionObject;
        interactionObject = null;

    }

    public void Throw()
    {
        inHand.transform.parent = null;
        inHand.GetComponent<Rigidbody>().isKinematic = false;
        inHand.GetComponent<BoxCollider>().isTrigger = false;
        inHand.GetComponent<InteractableObject>().enabled = true;

        anim.SetInteger("Weapon", 0);

        inHand = null;
    }

    public void AEPickup()
    {
        toPickup.GetComponent<Rigidbody>().isKinematic = true;
        toPickup.transform.SetParent(rHand);
        toPickup.GetComponent<BoxCollider>().isTrigger = true;
        toPickup.transform.localPosition = new Vector3(12.3f, 2.2f, -21.3f);
        toPickup.transform.localRotation = Quaternion.Euler(-80.5f, 93.3f, -95.1f);

        if (toPickup.GetComponent<InteractableObject>().IsWeapon)
        {
            anim.SetInteger("Weapon", toPickup.GetComponent<InteractableObject>()._Item.weaponNumber);
        }

        toPickup.GetComponent<InteractableObject>().enabled = false;
        inHand = toPickup;
        toPickup = null;
    }


}
