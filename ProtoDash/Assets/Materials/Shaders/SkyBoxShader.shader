﻿Shader "Custom/CustomSkybox"
{
	Properties
	{
		_Color("Color", Color) = (1.,1.,1.,1.)
		_Color2("Color2", Color) = (1.,0.,1.,1.)
	}
		SubShader
	{
		Tags{ "Queue" = "Geometry" }

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

#define POWER .15

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = fixed2(.5, .5) - v.uv;

		return o;
	}

	float4 _Color;
	float4 _Color2;

	fixed4 frag(v2f i) : SV_Target
	{
		float f = saturate(
			pow(
				length(i.uv),POWER
			)
		);
		return lerp(_Color,_Color2,f);
	}
		ENDCG
	}
	}
}
