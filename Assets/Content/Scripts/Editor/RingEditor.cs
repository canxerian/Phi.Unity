using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ring))]
[CanEditMultipleObjects]
public class RingEditor : Editor
{
    SerializedProperty resolution;
    SerializedProperty radius;
    SerializedProperty prefab;
    SerializedProperty scale;

    void OnEnable()
    {
        resolution = serializedObject.FindProperty("resolution");
        radius = serializedObject.FindProperty("radius");
        prefab = serializedObject.FindProperty("prefab");
        scale = serializedObject.FindProperty("scale");
    }


    private void CreateLoop()
    {
        Ring ring = (target as Ring);
        Cleanup();

        for (int i = 0; i < resolution.intValue; i++)
        {
            float t = i / (float)resolution.intValue;

            float x = Mathf.Sin(2 * Mathf.PI * t);
            float z = Mathf.Cos(2 * Mathf.PI * t);
            Vector3 pos = new Vector3(x, 0, z) * radius.floatValue;

            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab.objectReferenceValue, ring.transform);
            go.transform.localScale = Vector3.one * scale.floatValue;
            go.transform.localPosition = pos;
            go.transform.forward = pos;
        }
    }

    private void Cleanup()
    {
        Ring ring = (target as Ring);

        // UnityEditor.EditorApplication.delayCall += () =>
        // {
        for (int i = ring.transform.childCount; i > 0; --i)
            DestroyImmediate(ring.transform.GetChild(0).gameObject);
        // };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        // EditorGUILayout.PropertyField(prefab);
        // EditorGUILayout.PropertyField(resolution);
        // EditorGUILayout.PropertyField(radius);
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            CreateLoop();
        }
        serializedObject.ApplyModifiedProperties();
    }
}