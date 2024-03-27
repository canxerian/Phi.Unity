using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightIntensityAnimator : MonoBehaviour
{
    [SerializeField] 
    private new Light light;
    
    [SerializeField, Range(0f, 20f)]
    private float minIntensity;

    [SerializeField, Range(0f, 20f)]
    private float maxIntensity;

    [SerializeField, Range(0f, 100f)]
    private float noiseFrequency;

    private void Update()
    {
        float t = Mathf.PerlinNoise(Time.time * noiseFrequency, 0.5f);      // Value of 0..1
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }

    private void OnValidate()
    {
        minIntensity = Mathf.Min(minIntensity, maxIntensity);
        maxIntensity = Mathf.Max(minIntensity, maxIntensity);
    }
}
