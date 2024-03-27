using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public SphereItem prefab;

    [Header("Editor Parameters")]
    [Range(1, 1000)]
    public int resolution;

    [Range(0.01f, 2f)]
    public float radius;

    [Range(0.01f, 1f)]
    public float itemScale;

    [Header("Runtime Parameters")]
    [Range(-2f, 2f)]
    public float pulseAmplitude = .5f;

    [Range(0.01f, 1f)]
    public float pulseSpeed = .5f;

    [Header("Other")]
    public new Rigidbody rigidbody;
    public List<SphereItem> sphereItems;

    private void FixedUpdate()
    {
        for (int i = 0; i < sphereItems.Count; i++)
        {
            SphereItem si = sphereItems[i];

            float t = i / (float)sphereItems.Count;
            float offset = Mathf.PerlinNoise(Time.time * pulseSpeed + si.uv.x, si.uv.y) * pulseAmplitude;

            Vector3 newPos = si.originalPosition + (si.originalPosition.normalized * offset);
            newPos = transform.TransformPoint(newPos);

            si.rigidbody.MovePosition(newPos);
        }
    }
}
