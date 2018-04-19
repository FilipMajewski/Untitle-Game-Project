using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthManager : MonoBehaviour
{
    [SerializeField]
    private static bool isBreakingLaw, globaLookingForPlayer, isCrouching;


    #region Encapsulation
    public static bool IsCrouching
    {
        get
        {
            return isCrouching;
        }

        set
        {
            isCrouching = value;
        }
    }

    public static bool GlobaLookingForPlayer
    {
        get
        {
            return globaLookingForPlayer;
        }

        set
        {
            globaLookingForPlayer = value;
        }
    }

    public static bool IsBreakingLaw
    {
        get
        {
            return isBreakingLaw;
        }

        set
        {
            isBreakingLaw = value;
        }
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        IsBreakingLaw = false;
        GlobaLookingForPlayer = false;
        IsCrouching = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
