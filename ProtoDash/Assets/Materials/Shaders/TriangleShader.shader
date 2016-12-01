Shader "Unlit/Triangle"
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
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	fixed4 _Color;

#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))
#define PI 3.141592
#define TPI 6.2831

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

	float dfPolygon(float side, float2 p)
	{
		float a = atan2(p.x, p.y);
		float b = TPI / side;
		
		p.y *= 2.;
		float f = p.x + p.y;
		f = smax(f, p.x - p.y,.15);
		f = smax(f,-p.x,.15);
		//f = smin(-p.x, 0., .15);
		return f;
	}



#define Si .3

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord - float2(.5,.5);
		o.color = v.color * _Color;
		return o;
	}



	fixed4 frag(v2f i) : COLOR
	{
		float f = dfPolygon(3., i.texcoord);
		f = _Smooth(.49, f, .01);

		float4 col = i.color;
		col.a *= f;
		return col;
	}

		ENDCG
	}
	}

}
