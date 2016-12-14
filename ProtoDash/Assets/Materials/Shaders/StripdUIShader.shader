Shader "Custom/StripeUI" {
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_SecondColor("Second Color", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

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
		ColorMask[_ColorMask]

		Pass{
			Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}
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

			// Parameters
#define FREQUENCE .025
#define TILT -.02
#define SPEED 4.
#define THIKNESS .04
#define SMOOTHNESS .001
			//



#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))
#define time  _Time.x * SPEED
#define _thikness THIKNESS * .5


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

				float stripes = frac(i.worldPos.x * FREQUENCE + i.worldPos.y * TILT + time); // frequence tilt and animation
				stripes = _Smooth(_thikness, abs(stripes - .5), SMOOTHNESS); // boldness
				
				float a = tex2D(_MainTex, i.texcoord).a;
				fixed4 col = lerp(_Color, _SecondColor, stripes);
				col.a = a;
				return col;
			}
			ENDCG
		}
	}
}
