Shader "Unlit/Cross"
	{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	}

		SubShader{
		Tags{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}
		LOD 100

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
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

#define Si .3

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord - float2(.5,.5);
		return o;
	}



	fixed4 frag(v2f i) : COLOR
	{
		float f = sdSegment(i.texcoord,float2(-Si,-Si),float2(Si,Si));
		f = smin(f , sdSegment(i.texcoord, float2(-Si,Si), float2(Si,-Si)), .05);
		f = _Smooth(.09, f, .004);

		float4 col = _Color;
		col.a *= f;
		return col;
	}

		ENDCG
	}
	}

}
