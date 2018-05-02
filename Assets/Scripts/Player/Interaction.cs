using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Interaction : MonoBehaviour
{
    Player_AnimationController animationController;
    CharacterController cc;

    bool canInteract;
    bool canPickup;
    bool hiden;

    public GameObject interactWith;

    int valueToAdd;

    #region Encapsulation

    public bool CanInteract
    {
        get
        {
            return canInteract;
        }

        set
        {
            canInteract = value;
        }
    }

    public bool CanPickup
    {
        get
        {
            return canPickup;
        }

        set
        {
            canPickup = value;
        }
    }

    public bool Hiden
    {
        get
        {
            return hiden;
        }

        set
        {
            hiden = value;
        }
    }

    #endregion

    // Use this for initialization
    void Start()
    {
        valueToAdd = 0;
        animationController = GetComponentInChildren<Player_AnimationController>();
        cc = GetComponent<CharacterController>();

        hiden = false;
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        var inputDevice = InputManager.ActiveDevice;

        if (canInteract && canPickup)
        {
            if (inputDevice.Action2.WasPressed)
            {
                Grab(interactWith);
            }
        }
        if (canInteract && !canPickup)
        {
            if (!hiden && inputDevice.Action2.WasPressed)
            {
                Hide();
                Debug.Log("Hiding");
            }
            else if (hiden && inputDevice.Action2.WasPressed)
            {
                Unhide();
                Debug.Log("Visible");
            }
        }

    }

    void Grab(GameObject interactionObject)
    {
        //play grab animation
        animationController.Anim.SetTrigger("Pickup");
        valueToAdd += interactionObject.GetComponent<InteractableObject>()._Item.value;
        interactionObject.GetComponent<InteractableObject>().enabled = false;
        Destroy(interactionObject);
        interactionObject = null;
    }

    void Hide()
    {
        hiden = true;
        gameObject.layer = 9;
    }

    void Unhide()
    {
        hiden = false;
        gameObject.layer = 8;
    }

}
