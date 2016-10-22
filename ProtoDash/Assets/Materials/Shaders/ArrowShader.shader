Shader "Unlit/Arrow" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		//BlendOp Min

		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			fixed4 _Color;


#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))
			float sdSegment(in float2 p, in float2 a, in float2 b)
			{
				float2 pa = p - a, ba = b - a;
				float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
				return length(pa - ba*h);
			}
			// from http://iquilezles.org/www/articles/smin/smin.htm
			// polynomial smooth min (k = 0.1);
			float smin(float a, float b, float k)
			{
				float h = clamp(0.5 + 0.5*(b - a) / k, 0.0, 1.0);
				return lerp(b, a, h) - k*h*(1.0 - h);
			}


			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv - float2(.5f,.5f);
				return o;
			}


#define Si .39
#define Bx .25
#define By .3

			fixed4 frag(v2f i) : COLOR
			{
				float4 col = _Color;
				i.uv.y = abs(i.uv.y);
				float f = sdSegment(i.uv,float2(-Si,.0f),float2(Si,.0f));
				f = smin(f , sdSegment(i.uv, float2(Bx,By), float2(Si, .0f)), .01);
				f = _Smooth(.06, f, .006);
				col.a *= f;
				return col;
			}
			ENDCG
		}
	}
}
