using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Image healthImage;

    public Color green;
    public Color red;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            healthImage.fillAmount -= 0.1f;
            healthImage.color = Color.Lerp(red, green, healthImage.fillAmount);
        }
    }
}
