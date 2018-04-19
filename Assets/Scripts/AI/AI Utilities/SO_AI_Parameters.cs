using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_", menuName = "AI/Parameters", order = 1)]

public class SO_AI_Parameters : ScriptableObject
{
    [Header("Vision Parameters")]
    public Shader shader;
    [HideInInspector]
    public Material fovMeshMaterial;

    public float visionRange;
    public float crouchedVisionRange;
    [Range(0, 180)]
    public float visionAngle;

    public Color normalColor;
    public Color investigateColor;
    public Color chaseColor;

    [Range(0, 1)]
    public float lerpSpeed;

    [Header("AI Type")]
    public bool isCamera;
    public bool isGuard;

    [Header("AI Parameters")]
    float lookingTime;

    [Header("Navigation Parameters")]
    public float chaseSpeed;
    public float normalSpeed;

    [Range(0, 360)]
    public float turnSpeed;
    public float lookingRadius;
}
