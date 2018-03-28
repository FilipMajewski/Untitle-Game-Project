using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    AnimationController animationController;

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
        animationController = GetComponentInChildren<AnimationController>();
        hiden = false;
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract && canPickup)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Grab(interactWith);
            }
        }
        if (canInteract && !canPickup)
        {
            if (!hiden && Input.GetKeyDown(KeyCode.P))
            {
                Hide();
                Debug.Log("Hiding");
            }
            else if (hiden && Input.GetKeyDown(KeyCode.P))
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
    }

    void Unhide()
    {
        hiden = false;
    }

}
