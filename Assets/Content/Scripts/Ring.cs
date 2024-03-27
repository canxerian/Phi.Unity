using UnityEditor;
using UnityEngine;

public class Ring : MonoBehaviour
{
  [SerializeField] private GameObject prefab;

  [Range(1f, 100f)]
  [SerializeField] private int resolution;

  [Range(0f, 0.5f)]
  [SerializeField] private float radius;

  [Range(0.1f, 1f)]
  [SerializeField] private float scale;
}
