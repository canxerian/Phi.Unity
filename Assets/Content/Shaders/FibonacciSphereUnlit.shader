// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// https://bitbucket.org/catlikecodingunitytutorials/basics-05-compute-shaders/src/master/Assets/Point/

Shader "Phi/FibonacciSphereUnlit" 
{
    Properties 
    {
        _Colour ("Colour", Color) = (1, 1, 1, 1)
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _Metallic ("Metallic", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _NoiseTex ("Noise", 2D) = "black" {}
        _NoiseAmount ("Noise Amount", Range(0, 10)) = 0.5
        _NoiseSpeed ("Noise Speed", Range(0, 1)) = 0.5
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #pragma target 5.0
            
            #include "UnityCG.cginc"

            // Passed to this shader in FibonacciSphereRenderer.cs
            struct InstanceProperties {
                float4x4 mat;
                float2 uv;
            };

            struct app {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 texcoord3 : TEXCOORD3;
                fixed4 color : COLOR;
                uint instanceID: SV_InstanceID;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };
            
            float4 _Colour;
            float4 _RimColor;
            float _RimPower;
            float _Metallic;
            float _Smoothness;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _NoiseAmount;
            float _NoiseSpeed;

            // Passed to this shader in FibonacciSphereRenderer.cs
            float4x4 _Object2WorldMat;
            StructuredBuffer<InstanceProperties> _Properties;

            v2f vert (app v) {
                float2 uv = _Properties[v.instanceID].uv;
                float4x4 localMat = _Properties[v.instanceID].mat;
                float4 localPos = mul(localMat, v.vertex);
                
                float t = _Time.x * _NoiseSpeed;
                float4 noiseUV = float4(uv.x + t, uv.y + t * 0.7, 0, 0);     // xy = uv, w = LOD
                float noise = tex2Dlod(_NoiseTex, noiseUV).r * _NoiseAmount;  
                localPos.xyz += localPos.xyz * noise;

                v2f o;
                unity_ObjectToWorld = _Object2WorldMat;
                o.vertex = UnityObjectToClipPos(localPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 1;
                return col;
            }

            ENDCG
        } 

    }
    Fallback "Diffuse"
}