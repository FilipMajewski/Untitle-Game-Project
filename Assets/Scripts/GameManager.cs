using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<Transform> wayPointsForAI;

    GameObject[] AIActors;

    // Use this for initialization
    void Start()
    {
        AIActors = GameObject.FindGameObjectsWithTag("AI");
    }

    // Update is called once per frame
    void Update()
    {

    }

}
