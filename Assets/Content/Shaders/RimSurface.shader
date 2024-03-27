Shader "Phi/Rim" {
    Properties {
        _Colour ("Colour", Color) = (1, 1, 1, 1)
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _Metallic ("Metallic", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5

        [PerRendererData]
        _UV ("UV", Float) = 0
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Standard
        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };
        
        float4 _Colour;
        float4 _RimColor;
        float _RimPower;
        float _Metallic;
        float _Smoothness;
        float2 _UV;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = _Colour.rgb;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    } 
    Fallback "Diffuse"
}