Shader "Unlit/SimpleHexagon"
{
	Properties
	{
		_Color("Color", Color) = (1.,1.,1.,1.)
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv - float2(.5, .5);

				return o;
			}

			float4 _Color;


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


			fixed4 frag (v2f i) : SV_Target
			{
				float2 position = i.uv;

				float dist = df(position );

				float shape = 1. -  _Smooth(dist, .44, .005);
				//shape *= _Smooth(dist,0.5,.01);

				float4 col = _Color;
				col.a *= shape;
				return col;
			}
			ENDCG
		}
	}
}
