Shader "Custom/S_IntersectionHighlight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
Blend SrcAlpha OneMinusSrcAlpha
Cull Off

CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

struct Input {
    float2 uv_MainTex;
    float3 worldPos;
};

half _OutlineWidth;
fixed4 _OutlineColor;

void surf (Input IN, inout SurfaceOutputStandard o) {
    // Calculate the distance from the fragment to the edge of the object
    half d = fwidth(length(IN.worldPos));

    // Check if the distance is within the outline width
    half alpha = saturate(d - _OutlineWidth);

    // Output the outline color with the calculated alpha
    o.Emission = _OutlineColor.rgb * alpha;
}
ENDCG

    }
    FallBack "Diffuse"
}
