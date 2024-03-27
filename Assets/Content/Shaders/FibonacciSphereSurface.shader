// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// https://bitbucket.org/catlikecodingunitytutorials/basics-05-compute-shaders/src/master/Assets/Point/

Shader "Phi/FibonacciSphereSurface" {
    Properties {
        [MainColor]
        _Colour ("Colour", Color) = (1, 1, 1, 1)
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _Metallic ("Metallic", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _NoiseTex ("Noise", 2D) = "black" {}
        _NoiseTex2 ("Noise 2", 2D) = "black" {}
        _NoiseMix("Noise Mix", Range(0, 1)) = 0.5 
        _NoiseAmount ("Noise Amount", Range(0, 10)) = 0.5
        _NoiseSpeed ("Noise Speed", Range(0, 10)) = 0.5
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        
        #pragma surface surf Standard vertex:vert
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

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
            float2 sphereUV;
        };
        
        float4 _Colour;
        float4 _RimColor;
        float _RimPower;
        float _Metallic;
        float _Smoothness;
        sampler2D _NoiseTex;
        sampler2D _NoiseTex2;
        float _NoiseMix;
        float _NoiseAmount;
        float _NoiseSpeed;

        // Passed to this shader in FibonacciSphereRenderer.cs
        float4x4 _Object2WorldMat;
        #ifdef SHADER_API_D3D11
            StructuredBuffer<InstanceProperties> _Properties;
        #endif

        void vert (inout app v, out Input o) {
            #ifdef SHADER_API_D3D11
                float2 uv = _Properties[v.instanceID].uv;
                float4 localPos = mul(_Properties[v.instanceID].mat, float4(v.vertex.xyz, 1.0));
                
                float4 noiseUV = float4(uv.x + _Time.x * _NoiseSpeed, uv.y, 0, 0);     // xy = uv, w = LOD
                float noise1 = tex2Dlod(_NoiseTex, noiseUV).r * _NoiseAmount;  
                float noise2 = tex2Dlod(_NoiseTex2, noiseUV).r * _NoiseAmount;  
                float noise = lerp(noise1, noise2, _NoiseMix);
                
                localPos.xyz += localPos.xyz * noise;

                UNITY_INITIALIZE_OUTPUT(Input, o);
                o.sphereUV = uv;
                // v.vertex = mul(_Object2WorldMat, float4(localPos.xyz, 1.0));
                v.vertex = localPos;
                
                // unity_ObjectToWorld = _Object2WorldMat;
                // v.vertex = UnityObjectToClipPos(localPos);
                // v.vertex = mul(UNITY_MATRIX_VP, mul(_Object2WorldMat, float4(localPos, 1.0)));
                // v.vertex =  mul(_Object2WorldMat, float4(localPos.xyz, 1.0));
                // v.vertex = mul(UNITY_MATRIX_VP, v.vertex);
                // v.vertex.xyz = localPos.xyz;
            #endif
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = _Colour;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    } 
    Fallback "Diffuse"
}