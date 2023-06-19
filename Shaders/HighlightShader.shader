Shader "Custom/HighlightShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1, 1, 0, 1)
        _HighlightPoint ("Highlight Point", Vector) = (0, 0, 0, 0)
        _HighlightRadius ("Highlight Radius", Range(0, 10)) = 1
    }
    
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _HighlightColor;
            float4 _HighlightPoint;
            float _HighlightRadius;
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                float dist = distance(i.worldPos, _HighlightPoint.xyz);
                float falloff = 1.0 - smoothstep(_HighlightRadius - 1.0, _HighlightRadius, dist);
                fixed4 highlightColor = _HighlightColor * falloff;
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor + highlightColor;
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            
            ENDCG
        }
    }
}
