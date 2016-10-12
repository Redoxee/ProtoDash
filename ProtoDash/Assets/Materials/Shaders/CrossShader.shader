Shader "Unlit/Cross"
	{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Rotation("Rotation", Float) = 0
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
	float _Rotation = 0.;

#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))

#define radius .2
#define teehNumber 4.
#define teethSize .3
#define teethSteepness .7
#define centerHoleSize .15

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord - float2(.5,.5);
		return o;
	}



	fixed4 frag(v2f i) : COLOR
	{
		float2 pos = i.texcoord.xy;

		float dist = length(pos);

		float teeth = sin(atan2(pos.y, pos.x) * teehNumber + _Rotation);
		teeth = _Smooth(teeth,0.,teethSteepness) * teethSize;

		float f = 1 - _Smooth(dist - teeth, radius,.006);

		float4 col = _Color;
		col.a = f;
		return col;
	}

		ENDCG
	}
	}

}
