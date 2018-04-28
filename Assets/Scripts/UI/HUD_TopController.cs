using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_TopController : MonoBehaviour
{

    public TextMeshProUGUI valueText;

    // Use this for initialization
    void Start()
    {
        valueText.text = "0";
    }

    // Update is called once per frame
    void LateUpdate()
    {
        valueText.text = GameManager.currentCash.ToString() + "$";
    }
}
