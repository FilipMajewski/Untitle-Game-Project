using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFOV_CheckForPlayer : MonoBehaviour
{
    FieldOfView view;
    Renderer rend;
    Transform player;
    PlayerController playerController;
    AI_Setup _AI_Setup;

    Shader shader;
    Material fovMeshMaterial;

    Color normalColor;
    Color investigateColor;
    Color chaseColor;

    float lerpSpeed;
    float crouchedRadiusDivider;

    bool isCamera;
    bool isGuard;

    public bool seeYou;
    public bool seeThatYouBreakinLaw;
    public bool lookingForYou;

    private void Awake()
    {

    }

    void Start()
    {
        SetupVision();

        view = GetComponentInChildren<FieldOfView>();
        rend = view.GetComponent<Renderer>();

        view.VisionRange = _AI_Setup.parameters.visionRange;
        view.ViewAngle = _AI_Setup.parameters.visionAngle;

        fovMeshMaterial = new Material(shader)
        {
            enableInstancing = true
        };

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
            if (lookingForYou)
            {
                if (fovMeshMaterial.GetColor("_Color") != investigateColor)
                {
                    fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, investigateColor, lerpSpeed));
                }
            }

            else
            {
                if (fovMeshMaterial.GetColor("_Color") != normalColor)
                {
                    fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, normalColor, lerpSpeed));
                }
            }
        }


        if (isCamera || isGuard)
        {
            if (playerController.Crouched)
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, view.VisionRange / crouchedRadiusDivider, lerpSpeed);
            }
            else
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, view.VisionRange, lerpSpeed);
            }
        }
        else
        {
            if (playerController.Crouched)
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, view.VisionRange, lerpSpeed);
            }
            else
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, 0, lerpSpeed);
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

    void SetupVision()
    {
        _AI_Setup = GetComponent<AI_Setup>();

        shader = _AI_Setup.parameters.shader;
        fovMeshMaterial = _AI_Setup.parameters.fovMeshMaterial;
        normalColor = _AI_Setup.parameters.normalColor;
        investigateColor = _AI_Setup.parameters.investigateColor;
        chaseColor = _AI_Setup.parameters.chaseColor;
        lerpSpeed = _AI_Setup.parameters.lerpSpeed;
        crouchedRadiusDivider = _AI_Setup.parameters.crouchedRadiusDivider;
        isCamera = _AI_Setup.parameters.isCamera;
        isGuard = _AI_Setup.parameters.isGuard;
    }
}
