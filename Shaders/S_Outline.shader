Shader "Custom/S_Outline" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0, 1)) = 0.5
        _GlowThreshold ("Glow Threshold", Range(0, 1)) = 0.8
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}

        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0
        #include "UnityCG.cginc"

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;
        float4 _Color;
        float4 _GlowColor;
        float _GlowIntensity;
        float _GlowThreshold;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Sample the main texture
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate the luminance of the color
            float luminance = dot(c.rgb, float3(0.2126, 0.7152, 0.0722));

            // Add glow to the material if the luminance is above the threshold
            if (luminance > _GlowThreshold) {
                o.Emission = _GlowColor.rgb * _GlowIntensity;
            }

            // Set the output color to the material color
            o.Albedo = _Color.rgb;

            // Set the output alpha to the material alpha
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
