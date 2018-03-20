using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    public SO_Item _Item;

    private bool isWeapon;

    Rigidbody rb;
    SphereCollider interactionCollider;
    Interaction interaction;

    public bool IsWeapon
    {
        get
        {
            return isWeapon;
        }

    }

    // Use this for initialization
    void Start()
    {
        isWeapon = _Item.isWeapon;
        interactionCollider = GetComponent<SphereCollider>();
        interactionCollider.radius = _Item.interactionArea;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb.isKinematic = true;
            Debug.Log("Can interact with " + _Item.name);
            interaction = other.GetComponent<Interaction>();
            interaction.CanInteract = true;
            interaction.interactWith = gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Can interact with " + _Item.name);
            interaction = other.GetComponent<Interaction>();
            interaction.CanInteract = false;
            interaction.interactWith = null;
            interaction = null;
        }
    }

    //#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _Item.interactionArea);
    }

    //#endif
}
