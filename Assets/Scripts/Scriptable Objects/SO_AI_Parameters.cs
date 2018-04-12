using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_", menuName = "AI/Parameters", order = 1)]

public class SO_AI_Parameters : ScriptableObject
{
    [Header("Vision Parameters")]
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

    [Header("AI Type")]
    public bool isCamera;
    public bool isGuard;

    [Header("AI Parameters")]

    float lookingTime;

    [Header("Navigation Parameters")]
    public float maxSpeed;

    [Range(0, 360)]
    public float turnSpeed;
}
