Shader "Custom/ExtandableCircle"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1.,1.,1.,1.)
		_Ratio("Ratio", Float) = .5

	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag alpha
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

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.uv;
			return o;
		}

		sampler2D _MainTex;
		float4 _Color;
		float _Ratio;
#define RADIUS .49
#define SMOOTH .01

		fixed4 frag(v2f i) : SV_Target
		{
			float2 uv = i.uv;
			uv.x /= _Ratio;

			uv.x = min(uv.x,.5) * step(i.uv.x,.5) + (max(1/_Ratio - .5, uv.x ) + (1. - 1./_Ratio)) * step(.5,i.uv.x);
			float d = distance(uv, float2(.5, .5));
			float f = smoothstep(RADIUS - SMOOTH, RADIUS + SMOOTH, d);
			return float4(_Color.rgb,(1. - f) * _Color.a);
		}
			ENDCG
		}
	}
}
