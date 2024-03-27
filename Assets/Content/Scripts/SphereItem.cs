using UnityEngine;

/// <summary>
/// Component attached to sphere item prefab
/// </summary>
public class SphereItem : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public new Renderer renderer;
    public new HingeJoint hingeJoint;

    public Vector3 originalPosition;
    public Vector2 uv;

    public void Initialise(Vector3 pos, Vector2 _uv, Rigidbody parent)
    {
        originalPosition = pos;
        uv = _uv;
        hingeJoint.connectedBody = parent;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetVector("_UV", uv);
        renderer.SetPropertyBlock(block);
    }
}
