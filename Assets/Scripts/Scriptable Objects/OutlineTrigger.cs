using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineTrigger : MonoBehaviour
{

    SphereCollider sphereCollider;
    public SO_Item item;
    Outline outline;

    // Use this for initialization
    void Start()
    {
        //item = GetComponentInParent<InteractableObject>()._Item;
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = item.outlineRadius;
        outline = transform.parent.GetComponentInChildren<Outline>();

        outline.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            outline.enabled = !outline.enabled;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            outline.enabled = !outline.enabled;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, item.outlineRadius);
    }
}
