Shader "Unlit/EndNodeLevel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1.,1.,1.,1.)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;

				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
#define BORDER .1
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			float4 _Color;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 center = float2(.5,.5) - i.uv;
				float d = length(center);
				float f = smoothstep(.49, .51, d);

				f = 1. - f;
				f -= 1. - smoothstep(.49 - BORDER, .51 - BORDER, d);

				float angle = atan2(center.y, center.x) + 1.57079;
				f *= smoothstep(-.1, .1, sin(angle * 10.) - .5);

				float4 col = _Color;
				col.a = f;
				return col;
			}
			ENDCG
		}
	}
}
