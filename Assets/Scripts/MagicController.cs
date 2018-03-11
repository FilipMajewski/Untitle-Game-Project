using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{

    public GameObject particlePrefab;

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(particlePrefab, transform.position, transform.rotation);
        }
    }
}
