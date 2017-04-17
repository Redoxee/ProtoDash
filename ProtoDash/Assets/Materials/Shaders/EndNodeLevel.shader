// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/EndNodeLevel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1.,1.,1.,1.)
		_AnimationTime("Animation Time", Float) = 0.
		_ClosureFactor("Closure Factor", Float) = 0.
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

#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))
#define PI 3.141592
#define TPI 6.2831

#define STRIPES 4.
#define COL 3.
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			float4 _Color;
			float _AnimationTime = 0.f;
			float _ClosureFactor = 0.f;


			// from http://iquilezles.org/www/articles/smin/smin.htm
			// polynomial smooth min (k = 0.1);
			float smin(float a, float b, float k)
			{
				float h = clamp(0.5 + 0.5*(b - a) / k, 0.0, 1.0);
				return lerp(b, a, h) - k*h*(1.0 - h);
			}

			float smax(float a, float b, float k)
			{
				return (-smin(-a, -b, k));
			}

			float df(float2 pos)
			{
				float2 q = abs(pos);
				return smax(smax((q.x * 0.866025 + pos.y*0.5), q.y, .05), smax((q.x * 0.866025 - pos.y*0.5), q.y, .05), .05);
			}


/*
			float df(float2 pos)
			{
				float2 q = abs(pos);
				return max((q.x * 0.866025 + q.y*0.5), q.y);
			}*/

			fixed4 frag (v2f i) : SV_Target
			{
				float2 position = float2(.5, .5) - i.uv;

				float dist = 1. - df(position *2.3);

				float stripID = ceil(dist * STRIPES);
				float v = (fmod(stripID , 2.) * 2. - 1.);

				float shape = _Smooth(sin(dist * TPI * STRIPES), .1 - clamp(_ClosureFactor * 3. - 2.,0.,1.) * 1.2,.05);
				float angle = (atan2(position.y,position.x));
				shape *= _Smooth(sin(angle * COL + _AnimationTime * stripID * v),0.4 - clamp(_ClosureFactor * 1.5 ,0.,1.) * 1.5,.01);
				shape *= _Smooth(dist,0.,.01);

				float4 col = _Color;
				col.a = shape;
				return col;
			}
			ENDCG
		}
	}
}
