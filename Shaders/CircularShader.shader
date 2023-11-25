Shader "Universal Render Pipeline/Custom/CircularShader" 
{
    Properties 
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 0.5
        _BorderSize("Border Size", Float) = 0.1
        _ShouldDisplayBorderOnly("Display Border Only?", Range(0,1)) = 0
        _WorldPos("World Position", Vector) = (0,0,0)
        _Offset("Offset", Vector) = (0,0,0)
        _AlphaThreshold("Alpha Threshold", Range(0,1)) = 0.5
    }
    SubShader 
    {
        Tags
        { 
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual
        ColorMask RGB

        Pass 
        {
            Name "CustomCircularPass"

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInstancing.hlsl"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : POSITION;
                float4 worldPos : TEXCOORD1;
                float2 texcoord  : TEXCOORD0;
            };

            float4 _Color;
            float _Radius;
            float _BorderSize;
            float _ShouldDisplayBorderOnly;
            float4 _WorldPos;
            float4 _Offset;
            float _AlphaThreshold;
            float4 _MainTex_ST;
            sampler2D _MainTex;

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 adjustedPos = float2(IN.worldPos.x - _WorldPos.x + _Offset.x, IN.worldPos.z - _WorldPos.z + _Offset.z);
                float X = 0.5 - adjustedPos.x, Y = 0.5 - adjustedPos.y;
                float uvDist = X * X + Y * Y;
                float cenDist = _Radius * _Radius - uvDist;

                float borderAlpha = 0;

                // Check if we should display the border only
                if (_ShouldDisplayBorderOnly > 0.0)
                {
                    float innerRadius = _Radius - _BorderSize;
                    float borderDist = _BorderSize - abs(cenDist);
                    borderAlpha = saturate(borderDist / _BorderSize);
                }
                else
                {
                    borderAlpha = saturate(cenDist / _Radius);
                }

                // Apply alpha threshold to discard pixels below the threshold
                if (borderAlpha < _AlphaThreshold)
                {
                    discard;
                }

                return _Color * borderAlpha;
            }
            ENDCG
        }
    } 
    Fallback "Diffuse"
}
