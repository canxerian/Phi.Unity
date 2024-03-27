using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void PointsOnSphere(int n, out Vector3[] positions, out Vector2[] uvs)
    {
        positions = new Vector3[n];
        uvs = new Vector2[n];

        // https://stackoverflow.com/a/26127012/999032
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f));   // golden angle in radians

        for (int i = 0; i < n; i++)
        {
            float y = 1 - (i / (n - 1f)) * 2;           // y goes from 1 to -1
            float r = Mathf.Sqrt(1 - y * y);            // radius at y
            float theta = phi * i;                      // golden angle increment
            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;

            // float angle = Mathf.Acos(x / z);            // Angle of the triangle formed by xz
            Vector3 position = new Vector3(x, y, z).normalized;
            Vector2 uv = new Vector2(x, y);
            uv.x = (uv.x + 1) * .5f;                    // Convert from -1, 1 to 0, 1
            uv.y = (uv.y + 1) * .5f;

            positions[i] = position;
            uvs[i] = uv;
        }
    }
}
