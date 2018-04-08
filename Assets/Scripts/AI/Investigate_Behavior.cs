using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Investigate_Behavior : MonoBehaviour
{
    FieldOfView view;
    Renderer rend;
    Transform player;
    PlayerController playerController;

    public Shader shader;
    public Material fovMeshMaterial;

    public Color normalColor;
    public Color investigateColor;
    public Color chaseColor;

    public float lerpSpeed;
    public float crouchedRadiusDivider;

    public bool isHuman;
    public bool seeYou;
    public bool seeThatYouBreakinLaw;
    // Use this for initialization
    void Start()
    {
        view = GetComponentInChildren<FieldOfView>();
        rend = view.GetComponent<Renderer>();

        fovMeshMaterial = new Material(shader)
        {
            enableInstancing = true
        };

        //rend.sharedMaterial = fovMeshMaterial;
        rend.material = fovMeshMaterial;

        fovMeshMaterial.SetColor("_Color", normalColor);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();

        StartCoroutine("CheckForInvestigation", 0.6f);
    }

    // Update is called once per frame
    void Update()
    {

        if (seeYou)
        {
            if (StealthManager.isBreakingLaw)
            {
                if (fovMeshMaterial.GetColor("_Color") != chaseColor)
                {
                    fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, chaseColor, lerpSpeed));
                }
            }
            if (!StealthManager.isBreakingLaw)
            {
                if (fovMeshMaterial.GetColor("_Color") != investigateColor)
                {
                    fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, investigateColor, lerpSpeed));
                }
            }


        }
        else
        {
            if (fovMeshMaterial.GetColor("_Color") != normalColor)
            {
                fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, normalColor, lerpSpeed));
            }
        }

        if (!isHuman)
        {
            if (playerController.Crouched)
            {
                view.CurrentViewRadius = Mathf.Lerp(view.CurrentViewRadius, view.viewRadius / crouchedRadiusDivider, lerpSpeed);
            }
            else
            {
                view.CurrentViewRadius = Mathf.Lerp(view.CurrentViewRadius, view.viewRadius, lerpSpeed);
            }
        }
        else
        {
            if (playerController.Crouched)
            {
                view.CurrentViewRadius = Mathf.Lerp(view.CurrentViewRadius, view.viewRadius, lerpSpeed);
            }
            else
            {
                view.CurrentViewRadius = Mathf.Lerp(view.CurrentViewRadius, 0, lerpSpeed);
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
            seeYou = true;

            if (StealthManager.isBreakingLaw)
            {
                seeThatYouBreakinLaw = true;
            }
        }
        else
        {
            seeYou = false;
            seeThatYouBreakinLaw = false;
        }



    }


}
