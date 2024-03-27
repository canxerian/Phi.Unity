using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Sphere))]
[CanEditMultipleObjects]
public class SphereEditor : Editor
{
    private void CreateInstances()
    {
        Cleanup();

        Sphere sphere = target as Sphere;

        Utils.PointsOnSphere(sphere.resolution, out Vector3[] positions, out Vector2[] uvs);
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 position = positions[i] * sphere.radius;
            Vector2 uv = uvs[i];

            SphereItem si = Instantiate<SphereItem>(sphere.prefab, sphere.transform);

            si.Initialise(position, uv, sphere.rigidbody);

            si.transform.localPosition = position;
            si.transform.localScale = Vector3.one * sphere.itemScale;
            si.transform.up = position;

            sphere.sphereItems.Add(si);
        }
    }

    private void Cleanup()
    {
        Sphere sphere = target as Sphere;
        for (int i = sphere.transform.childCount; i > 0; --i)
        {
            DestroyImmediate(sphere.transform.GetChild(0).gameObject);
        }
        sphere.sphereItems.Clear();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            if (!Application.isPlaying)
            {
                CreateInstances();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
