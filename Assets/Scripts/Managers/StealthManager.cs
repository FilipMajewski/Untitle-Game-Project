using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthManager : MonoBehaviour
{
    public static bool seeYou;
    public static bool isBreakingLaw;

    // Use this for initialization
    void Start()
    {
        isBreakingLaw = false;
        seeYou = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            isBreakingLaw = true;
        }
        else
        {
            isBreakingLaw = false;
        }
    }
}
