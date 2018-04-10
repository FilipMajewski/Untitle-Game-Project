using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_", menuName = "AI/Parameters", order = 1)]

public class SO_AI_Parameters : ScriptableObject
{
    public Shader shader;
    public Material fovMeshMaterial;

    public float visionRange;
    [Range(0, 360)]
    public float visionAngle;

    public Color normalColor;
    public Color investigateColor;
    public Color chaseColor;

    [Range(0, 1)]
    public float lerpSpeed;
    public float crouchedRadiusDivider;

    public bool isCamera;
    public bool isGuard;

}
