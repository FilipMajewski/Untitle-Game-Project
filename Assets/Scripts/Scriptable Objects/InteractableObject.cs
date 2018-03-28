using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    public SO_Item _Item;

    SphereCollider interactionCollider;
    Interaction interaction;

    // Use this for initialization
    void Start()
    {
        interactionCollider = GetComponent<SphereCollider>();
        interactionCollider.radius = _Item.interactionArea;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Can interact with " + _Item.name);
            interaction = other.GetComponent<Interaction>();
            interaction.CanInteract = true;
            interaction.CanPickup = _Item.canPickup;
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
            interaction.CanPickup = false;
            interaction.interactWith = null;
            interaction = null;
        }
    }

    private void OnDestroy()
    {
        interaction = GameObject.FindGameObjectWithTag("Player").GetComponent<Interaction>();
        interaction.CanInteract = false;
        interaction.interactWith = null;
        interaction.CanPickup = false;
        interaction = null;
    }

    //#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _Item.interactionArea);
    }

    //#endif
}
