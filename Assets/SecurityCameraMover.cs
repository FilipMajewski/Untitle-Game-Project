﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraMover : MonoBehaviour
{

    public Transform cameraToMove;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cameraToMove.localRotation.y == -90)
        {

        }
    }
}