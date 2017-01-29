// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Custom/BicolorUIShader" {
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_SecondColor("Second Color", Color) = (1,1,1,1)
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
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 worldPos : POSITIONT;
				half2 texcoord  : TEXCOORD0;
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
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.texcoord = v.uv;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : COLOR
			{
				float time = _Time.x * 60.;
				float x = sin(
					(i.worldPos.x - i.worldPos.y / 2.) * PI * 2.5
					+ time);
				x = _Smooth(x, 0, .025);// *sin(time * .5);
				float a = tex2D(_MainTex, i.texcoord).a;
				fixed4 col = lerp(_Color, _SecondColor, x);
				col.a = a;
				return col;
			}
			ENDCG
		}
	}
}
