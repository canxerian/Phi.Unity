using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class FibonacciOrbitingRenderer : MonoBehaviour
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

    [SerializeField]
    private ComputeShader computeShader;

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
    private ComputeBuffer positionsBuffer;
    private Vector3[] positions;
    private Vector2[] uvs;

    static int nPointsShaderId = Shader.PropertyToID("_nPoints");
    static int timeShaderId = Shader.PropertyToID("_Time");
    static int positionsShaderId = Shader.PropertyToID("_Positions");

    [SerializeField]
    private Transform boundTest;

    private void OnEnable()
    {
        Initialise();
    }

    private void OnDisable()
    {
        computeBuffer.Release();
        computeBuffer = null;
    }

    private void Initialise()
    {
        if (computeBuffer != null)
        {
            computeBuffer.Release();
        }

        if (positionsBuffer != null)
        {
            positionsBuffer.Release();
        }

        Utils.PointsOnSphere(nPoints, out positions, out uvs);
        bounds = new Bounds(Vector3.zero, Vector3.one * radius);       // Bounds center affects position (for some reason!). So we set it to zero, as opposed to current position 
        block = new MaterialPropertyBlock();

        instanceProperties = new InstanceProperties[nPoints];
        computeBuffer = new ComputeBuffer(nPoints, InstanceProperties.Size());

        positionsBuffer = new ComputeBuffer(nPoints, 3 * 4);

        // for (int i = 0; i < nPoints; i++)
        // {
        //     Vector3 pointPosition = positions[i] * radius;
        //     instanceProperties[i] = new InstanceProperties()
        //     {
        //         mat = Matrix4x4.TRS(pointPosition, Quaternion.LookRotation(positions[i]), Vector3.one * scale),
        //         uv = uvs[i],
        //     };
        // }
    }

    private void Update()
    {
        if (computeBuffer == null || transform.hasChanged)
        {
            Initialise();
        }

        UpdateComputeShader();

        computeBuffer.SetData(instanceProperties);
        material.SetBuffer("_Properties", computeBuffer);
        material.SetMatrix("_Object2WorldMat", transform.localToWorldMatrix);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, nPoints, block, ShadowCastingMode.Off, false, gameObject.layer, null, LightProbeUsage.BlendProbes);
    }

    private void UpdateComputeShader()
    {
        computeShader.SetInt(nPointsShaderId, nPoints);
        computeShader.SetFloat(timeShaderId, Time.time);
        computeShader.SetBuffer(0, positionsShaderId, positionsBuffer);
    }

    private void OnValidate()
    {
        Initialise();
    }
}
