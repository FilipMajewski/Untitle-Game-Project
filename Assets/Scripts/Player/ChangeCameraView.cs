using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeCameraView : MonoBehaviour
{

    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera topCamera;

    Camera cam;
    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            //cam.orthographic = !cam.orthographic;
            normalCamera.gameObject.SetActive(!normalCamera.gameObject.activeSelf);
            topCamera.gameObject.SetActive(!topCamera.gameObject.activeSelf);
        }
    }
}
