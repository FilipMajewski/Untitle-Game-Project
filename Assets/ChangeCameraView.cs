using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeCameraView : MonoBehaviour
{

    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera topCamera;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            normalCamera.gameObject.SetActive(!normalCamera.gameObject.activeSelf);
            topCamera.gameObject.SetActive(!topCamera.gameObject.activeSelf);
        }
    }
}
