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

    private bool seeThatYouBreakinLaw, lookingForYou, seeYou;

    #region Encapsulation
    public bool SeeYou
    {
        get
        {
            return seeYou;
        }

        set
        {
            seeYou = value;
        }
    }

    public bool SeeThatYouBreakinLaw
    {
        get
        {
            return seeThatYouBreakinLaw;
        }

        set
        {
            seeThatYouBreakinLaw = value;
        }
    }

    public bool LookingForYou
    {
        get
        {
            return lookingForYou;
        }

        set
        {
            lookingForYou = value;
        }
    }
    #endregion

    void Start()
    {
        SetupAIElements();

        view = GetComponentInChildren<FieldOfView>();
        rend = view.GetComponent<Renderer>();

        view.VisionRange = _AI_Setup.parameters.visionRange;
        view.ViewAngle = _AI_Setup.parameters.visionAngle * 2;

        Debug.Log(view.ViewAngle);

        fovMeshMaterial = new Material(shader)
        {
            enableInstancing = true
        };

        rend.material = fovMeshMaterial;

        fovMeshMaterial.SetColor("_Color", normalColor);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        #region Color Change

        if (SeeYou)
        {
            if (_AI_Setup.IsBreakingTheLaw)
            {
                if (fovMeshMaterial.GetColor("_Color") != chaseColor)
                {
                    fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, chaseColor, lerpSpeed));
                }
            }
            if (!_AI_Setup.IsBreakingTheLaw)
            {
                if (fovMeshMaterial.GetColor("_Color") != investigateColor)
                {
                    fovMeshMaterial.SetColor("_Color", Color.Lerp(fovMeshMaterial.color, investigateColor, lerpSpeed));
                }
            }

        }
        else
        {
            if (LookingForYou || _AI_Setup.GlobalLookingForPlayer && isCamera
                || _AI_Setup.GlobalLookingForPlayer && isGuard
                || _AI_Setup.CalledToSearchPlayer && isGuard)
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

        #endregion

        #region Vision Range
        if (isCamera || isGuard)
        {
            if (_AI_Setup.IsPlayerCrouching)
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, _AI_Setup.CrouchedVisionRange, lerpSpeed);
            }
            else
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, _AI_Setup.VisionRange, lerpSpeed);
            }
        }
        else
        {
            if (_AI_Setup.IsPlayerCrouching)
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, _AI_Setup.VisionRange, lerpSpeed);
            }
            else
            {
                view.CurrentVisionRange = Mathf.Lerp(view.CurrentVisionRange, 0, lerpSpeed);
            }
        }
        #endregion

    }

    void SetupAIElements()
    {
        _AI_Setup = GetComponent<AI_Setup>();

        shader = _AI_Setup.parameters.shader;
        fovMeshMaterial = _AI_Setup.parameters.fovMeshMaterial;
        normalColor = _AI_Setup.parameters.normalColor;
        investigateColor = _AI_Setup.parameters.investigateColor;
        chaseColor = _AI_Setup.parameters.chaseColor;
        lerpSpeed = _AI_Setup.parameters.lerpSpeed;
        isCamera = _AI_Setup.parameters.isCamera;
        isGuard = _AI_Setup.parameters.isGuard;
    }
}



