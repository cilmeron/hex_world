Shader "Universal Render Pipeline/Custom/HighlightShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1, 1, 0, 1)
        _HighlightPoint ("Highlight Point", Vector) = (0, 0, 0, 0)
        _HighlightRadius ("Highlight Radius", Range(0, 10)) = 1
    }

    SubShader {
        Tags {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            #pragma target 3.0
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : TEXCOORD1;
            };
            
            fixed4 _HighlightColor;
            float4 _HighlightPoint;
            float _HighlightRadius;
            
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float dist = distance(i.pos.xyz, _HighlightPoint.xyz);
                float falloff = 1.0 - smoothstep(_HighlightRadius - 1.0, _HighlightRadius, dist);
                fixed4 highlightColor = _HighlightColor * falloff;
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor + highlightColor;
                return finalColor;
            }
            ENDHLSL
        }
    }
    CustomEditor "ShaderGraphURPTests.SampleShaderGraphMaterialInspector"
}
