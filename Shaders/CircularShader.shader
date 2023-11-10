Shader "Custom/CircularShader" 
{
	Properties 
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Radius("Radius", Float) = 0.5
		_BorderSize("Border Thickness", Float) = 0.1
		_ShouldDisplayBorderOnly("Display Border Only?", Range(0,1)) = 0
		_WorldPos("World Position", Vector) = (0,0,0)
		_Offset("Offset", Vector) = (0,0,0)
		_AlphaThreshold("Alpha Threshold", Range(0,1)) = 0.5
	}
	SubShader 
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater [_AlphaThreshold]

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			float4 _Color;
			float _Radius;
			float _BorderSize;
			float _ShouldDisplayBorderOnly;
			float4 _WorldPos;
			float4 _Offset;
			float _AlphaThreshold;
			sampler2D _MainTex;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float2 texcoord  : TEXCOORD0;
			};

			v2f vert (appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
				OUT.texcoord = IN.texcoord;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

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
	Fallback "Transparent/VertexLit"
}

