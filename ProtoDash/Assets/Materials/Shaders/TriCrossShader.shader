Shader "Unlit/Tricross"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Stretch("Stretch", Float) = .3
		_Radius("Radius",Float) = .075
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
#define PI 3.141592
#define TPI 6.2831


	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord - float2(.5,.5);
		return o;
	}

	float df(float2 position)
	{
		return min(length(position - float2(0., _Stretch)), max(position.x, step(_Stretch, position.y))) + step(position.y, .0);
	}

	fixed4 frag(v2f i) : COLOR
	{
		float2 pos = i.texcoord.xy;

		pos.x = abs(pos.x);


		float f = df(pos);


		float sa = .86602; // sa = sin(PI * 2./3.)
		float2x2 rotation = float2x2(-.5, -sa, sa, -.5);
		pos = mul(rotation, pos);

		pos.x = abs(pos.x);
		f = min(f, df(pos));


		f = _Smooth(_Radius, f, .015);


		float4 col = _Color;
		col.a *= f;
		return col;
	}

		ENDCG
	}
	}

}
