using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthManager : MonoBehaviour
{

    public static bool isBreakingLaw;
    public static bool globaLookingForPlayer;

    // Use this for initialization
    void Start()
    {
        isBreakingLaw = false;
        globaLookingForPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Breaking Law " + isBreakingLaw);
        Debug.Log("Global looking for player " + globaLookingForPlayer);
    }
}
