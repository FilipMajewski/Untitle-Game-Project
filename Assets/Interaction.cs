using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    AnimationController animationController;

    bool canInteract;
    public GameObject interactWith;

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

    #endregion

    // Use this for initialization
    void Start()
    {
        animationController = GetComponentInChildren<AnimationController>();
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                animationController.Grab(interactWith);
            }
        }
    }


}
