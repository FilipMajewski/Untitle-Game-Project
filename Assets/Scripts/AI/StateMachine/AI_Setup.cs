using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Setup : MonoBehaviour
{

    Animator anim;
    DrawFOV_CheckForPlayer fov;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        fov = GetComponent<DrawFOV_CheckForPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("SeePlayer", fov.seeYou);
        anim.SetBool("SeeLawBreaking", fov.seeThatYouBreakinLaw);
    }
}
