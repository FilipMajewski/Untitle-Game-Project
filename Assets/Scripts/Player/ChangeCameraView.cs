using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using InControl;

public class ChangeCameraView : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera topCamera;

    [Header("Other")]
    Camera cam;
    Transform targetPoint;
    public Collider cameraBoundryBox;
    GameObject player;

    bool isTopdownCameraActive;

    public bool IsTopdownCameraActive
    {
        get
        {
            return isTopdownCameraActive;
        }

        set
        {
            isTopdownCameraActive = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
        targetPoint = GameObject.FindGameObjectWithTag("CameraTarget").transform;
        IsTopdownCameraActive = false;
        targetPoint.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var inputDevice = InputManager.ActiveDevice;

        if (inputDevice.Action4.WasPressed)
        {
            cam.orthographic = !cam.orthographic;
            normalCamera.gameObject.SetActive(!normalCamera.gameObject.activeSelf);
            topCamera.gameObject.SetActive(!topCamera.gameObject.activeSelf);

            targetPoint.position = player.transform.position;
            IsTopdownCameraActive = !IsTopdownCameraActive;
        }

        if (topCamera.isActiveAndEnabled)
        {
            topCamera.Follow = targetPoint;


            if (inputDevice.LeftStickX != 0 || inputDevice.LeftStickY != 0)
            {
                if (cameraBoundryBox.bounds.Contains(targetPoint.position))
                {
                    targetPoint.Translate(new Vector3(inputDevice.LeftStickX, 0, inputDevice.LeftStickY) * 1f);
                }
                else
                {
                    targetPoint.position = cameraBoundryBox.ClosestPointOnBounds(targetPoint.position);
                }
            }
        }
        else
        {
            topCamera.Follow = player.transform;
        }
    }
}
