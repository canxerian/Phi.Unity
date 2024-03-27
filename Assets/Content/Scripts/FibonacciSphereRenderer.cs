using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class FibonacciSphereRenderer : MonoBehaviour
{
    private struct InstanceProperties
    {
        public Matrix4x4 mat;
        public Vector2 uv;

        public static int Size()
        {
            int matrixSize = sizeof(float) * 4 * 4;
            int uvSize = sizeof(float) * 2;
            return matrixSize + uvSize;
        }
    }

    [SerializeField]
    private Mesh mesh;

    [SerializeField]
    private Material material;

    [SerializeField, Range(1, 50000)]
    private int nPoints = 1000;

    [SerializeField, Range(0.1f, 2f)]
    private float radius = 1f;

    [SerializeField, Range(0.001f, 0.1f)]
    private float scale = 0.05f;

    private Bounds bounds;
    private MaterialPropertyBlock block;
    private InstanceProperties[] instanceProperties;
    private ComputeBuffer computeBuffer;
    private Vector3[] positions;
    private Vector2[] uvs;

    private void OnEnable()
    {
        Initialise();
    }

    private void Initialise()
    {
        if (computeBuffer != null)
        {
            computeBuffer.Dispose();
        }

        Utils.PointsOnSphere(nPoints, out positions, out uvs);
        bounds = new Bounds(transform.position, Vector3.one * radius);
        block = new MaterialPropertyBlock();

        instanceProperties = new InstanceProperties[nPoints];
        computeBuffer = new ComputeBuffer(nPoints, InstanceProperties.Size());

        for (int i = 0; i < nPoints; i++)
        {
            Vector3 pointPosition = positions[i] * radius;
            instanceProperties[i] = new InstanceProperties()
            {
                mat = Matrix4x4.TRS(pointPosition, Quaternion.LookRotation(positions[i]), Vector3.one * scale),
                uv = uvs[i],
            };
        }
    }

    private void Update()
    {
        if (computeBuffer == null)
        {
            Initialise();
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out MixedRealityPose pose))
        {
            float planetRadius = 1.5f;
            for (int i = 0; i < nPoints; i++)
            {
                Vector3 pointPosWorld = transform.TransformPoint(instanceProperties[i].mat.GetColumn(3));
                Vector3 originalPointWorld = transform.TransformPoint(positions[i]);

                float distance = Vector3.Distance(originalPointWorld, pose.Position);

                if (distance < planetRadius)
                {
                    pointPosWorld = Vector3.Lerp(pointPosWorld, pose.Position, Time.deltaTime);

                    // Update matrix
                    instanceProperties[i].mat = Matrix4x4.TRS(transform.InverseTransformPoint(pointPosWorld), Quaternion.LookRotation(positions[i]), Vector3.one * scale);
                }
            }
        }

        computeBuffer.SetData(instanceProperties);
        material.SetBuffer("_Properties", computeBuffer);
        material.SetMatrix("_Object2WorldMat", transform.localToWorldMatrix);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, nPoints, block, ShadowCastingMode.Off, false, gameObject.layer, null, LightProbeUsage.BlendProbes);
    }

    private void OnValidate()
    {
        Initialise();
    }
}
