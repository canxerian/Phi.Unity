using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FibonacciSphereRenderer))]
public class FibonacciSphereRendererEditor : Editor
{
    private MaterialEditor materialEditor;

    private void OnEnable()
    {
        FibonacciSphereRenderer fSphereRenderer = (FibonacciSphereRenderer)target;
        SerializedProperty materialProp = serializedObject.FindProperty("material");

        materialEditor = (MaterialEditor)CreateEditor(materialProp.objectReferenceValue);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (materialEditor != null)
        {
            materialEditor.DrawHeader();
            materialEditor.OnInspectorGUI();
        }
    }
}
