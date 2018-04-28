using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
        if (Input.GetButtonDown("Fire2"))
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


            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if (cameraBoundryBox.bounds.Contains(targetPoint.position))
                {
                    targetPoint.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 1f);
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
