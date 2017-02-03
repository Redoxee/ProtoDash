Shader "Unlit/Cross"
	{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Stretch("Stretch", Float) = .3
		_Radius("Radius",Float) = .09
	}

	SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Stencil{
				Comp always
			}
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
	float _Stretch;
	float _Radius;
	
	#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))
	float sdSegment(in float2 p, in float2 a, in float2 b)
	{
		float2 pa = p - a, ba = b - a;
		float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
		return length(pa - ba*h);
	}


	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord - float2(.5,.5);
		return o;
	}



	fixed4 frag(v2f i) : COLOR
	{
		float f = sdSegment(abs(i.texcoord),float2(0,0),float2(_Stretch,_Stretch));

		f = _Smooth(_Radius, f, .03);

		float4 col = _Color;
		col.a *= f;
		return col;
	}

		ENDCG
	}
	}

}
