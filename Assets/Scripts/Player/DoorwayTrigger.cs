using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayTrigger : MonoBehaviour
{

    Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Render everything *except* layer 14
            //cam.cullingMask = ~(1 << 14);

            // Switch off layer 14, leave others as-is
            cam.cullingMask &= ~(1 << 11);


            // Switch on layer 14, leave others as-is
            //cam.cullingMask |= (1 << 14);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Render everything *except* layer 14
            //cam.cullingMask = ~(1 << 14);

            // Switch off layer 14, leave others as-is
            //cam.cullingMask &= ~(1 << 11);


            // Switch on layer 14, leave others as-is
            cam.cullingMask |= (1 << 11);
        }
    }
}
