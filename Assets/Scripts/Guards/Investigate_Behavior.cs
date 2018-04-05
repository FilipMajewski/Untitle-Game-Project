using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Investigate_Behavior : MonoBehaviour
{

    FieldOfView view;
    Transform player;

    public Material fovMeshMaterial;

    public Color normalColor;
    public Color investigateColor;
    public Color chaseColor;
    // Use this for initialization
    void Start()
    {
        fovMeshMaterial.SetColor("_Color", normalColor);
        view = GetComponent<FieldOfView>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine("CheckForInvestigation", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (StealthManager.seeYou && !StealthManager.isBreakingLaw)
        {
            if (fovMeshMaterial.GetColor("_Color") != investigateColor)
            {
                fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, investigateColor, .1f));
            }
        }
        else if (StealthManager.seeYou && StealthManager.isBreakingLaw)
        {
            if (fovMeshMaterial.GetColor("_Color") != chaseColor)
            {
                fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, chaseColor, .1f));
            }
        }
        else
        {
            if (fovMeshMaterial.GetColor("_Color") != normalColor)
            {
                fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, normalColor, .1f));
            }
        }
    }

    IEnumerator CheckForInvestigation(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            LookForInvestigation();
        }
    }

    void LookForInvestigation()
    {
        if (view.visibleTargets.Contains(player))
        {
            Debug.Log("See You!");
            StealthManager.seeYou = true;
        }
        else
        {
            StealthManager.seeYou = false;
        }

        if (StealthManager.seeYou && StealthManager.isBreakingLaw)
        {
            Debug.Log("Breaking Law Reaction");
        }
        else
        {
            Debug.Log("Normal Behavior");
        }
    }


}
