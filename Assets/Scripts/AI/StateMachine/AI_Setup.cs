using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
public class AI_Setup : MonoBehaviour
{
    public SO_AI_Parameters parameters;

    DrawFOV_CheckForPlayer fov;
    List<GameObject> waypoints;
    Vector3 sawPlayerAtThisPosition;
    GlobalBlackboard stealthBlackboard;

    private bool isPlayerCrouching, calledToSearchPlayer, isBreakingTheLaw, globalLookingForPlayer;
    private float visionAngle, normalSpeed, chaseSpeed, visionRange, crouchedVisionRange;

    #region Encapsulation

    public float NormalSpeed
    {
        get
        {
            return normalSpeed;
        }

        set
        {
            normalSpeed = value;
        }
    }

    public float ChaseSpeed
    {
        get
        {
            return chaseSpeed;
        }

        set
        {
            chaseSpeed = value;
        }
    }

    public float VisionRange
    {
        get
        {
            return visionRange;
        }

        set
        {
            visionRange = value;
        }
    }

    public float VisionAngle
    {
        get
        {
            return visionAngle;
        }

        set
        {
            visionAngle = value;
        }
    }

    public List<GameObject> Waypoints
    {
        get
        {
            return Waypoints1;
        }

        set
        {
            Waypoints1 = value;
        }
    }

    public float CrouchedVisionRange
    {
        get
        {
            return crouchedVisionRange;
        }

        set
        {
            crouchedVisionRange = value;
        }
    }

    public bool CalledToSearchPlayer
    {
        get
        {
            return calledToSearchPlayer;
        }

        set
        {
            calledToSearchPlayer = value;
        }
    }

    public bool IsPlayerCrouching
    {
        get
        {
            return isPlayerCrouching;
        }

        set
        {
            isPlayerCrouching = value;
        }
    }

    public bool IsBreakingTheLaw
    {
        get
        {
            return isBreakingTheLaw;
        }

        set
        {
            isBreakingTheLaw = value;
        }
    }

    public bool GlobalLookingForPlayer
    {
        get
        {
            return globalLookingForPlayer;
        }

        set
        {
            globalLookingForPlayer = value;
        }
    }

    public Vector3 SawPlayerAtThisPosition
    {
        get
        {
            return sawPlayerAtThisPosition;
        }

        set
        {
            sawPlayerAtThisPosition = value;
        }
    }

    public List<GameObject> Waypoints1
    {
        get
        {
            return waypoints;
        }

        set
        {
            waypoints = value;
        }
    }
    #endregion

    void Awake()
    {
        FeedAIParameters();

    }

    private void Start()
    {
        fov = GetComponent<DrawFOV_CheckForPlayer>();
        stealthBlackboard = GameObject.FindGameObjectWithTag("Manager").GetComponent<GlobalBlackboard>();
        CalledToSearchPlayer = false;

        GameObject[] waypointsArray = GameObject.FindGameObjectsWithTag("Waypoint");

        for (int i = 0; i < waypointsArray.Length; i++)
        {
            Waypoints.Add(waypointsArray[i]);
        }


    }

    private void Update()
    {
        IsPlayerCrouching = stealthBlackboard.GetValue<bool>("isCrouching");
        IsBreakingTheLaw = stealthBlackboard.GetValue<bool>("isBreakingTheLaw");
        GlobalLookingForPlayer = stealthBlackboard.GetValue<bool>("globalLookingForPlayer");

        if (isPlayerCrouching)
        {
            VisionRange = CrouchedVisionRange;
        }
        else
        {
            VisionRange = parameters.visionRange;
        }

    }

    void FeedAIParameters()
    {
        NormalSpeed = parameters.normalSpeed;
        ChaseSpeed = parameters.chaseSpeed;
        VisionRange = parameters.visionRange;
        VisionAngle = parameters.visionAngle;
        CrouchedVisionRange = parameters.crouchedVisionRange;
    }

}
