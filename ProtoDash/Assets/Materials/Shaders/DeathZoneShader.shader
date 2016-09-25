﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Custom/DeathZone" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SecondColor("Second Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog

#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 worldPos : POSITIONT;
				float2 uv : TEXCOORD0;
				
			};

			fixed4 _Color;
			fixed4 _SecondColor;

#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))
#define PI 3.141592
#define TPI 6.2831


			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float time = _Time.x * 50.;
				float x = sin(i.worldPos.x * PI - i.worldPos.y * PI / 2. + time);
				x = _Smooth(x,0,.05) * sin(time * .5);
				fixed4 col = lerp(_Color, _SecondColor, x);
				
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
			ENDCG
		}
	}
}
