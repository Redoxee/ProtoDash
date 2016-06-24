Shader "Unlit/BackgroundShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CurrentPosition("Current Position", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True" }
		LOD 100
		Cull Off
		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float2 _CurrentPosition;
			
			//FRAGMENT
			float4x4 _CameraToWorld;
#define LEVEL_NUMBER 10.

#define SF 15.

#define bsin(x) (sin(x) * .5 + .5)

#define NB_COLORS 4

			float4 mColors[NB_COLORS];
			void init()
			{
				mColors[0] = float4(0.3, .00, .50, 1.)*.9;
				mColors[1] = float4(0.3, .01, .50, 1.)*.8;
				mColors[2] = float4(0.3, .00, .49, 1.)*.7;
				mColors[3] = float4(0.3, .00, .49, 1.)*.6;
			}

			float fbm(float x)
			{
				float r = bsin(x * 0001.9) * 0.25000;
				r += bsin(x * 002.54) * 0.25000;
				r += bsin(x * 0006.5) * 0.17000;

				r *= .7;
				r += .7;

				return floor(r * SF) / SF;
			}

			float mountain(float x)
			{
				float d = SF;
				float f1 = fbm(floor(x * SF) / SF);
				float f2 = fbm((floor(x * SF) + 1.) / SF);
				return f1 + (f2 - f1) * frac(x*d);
			}

			fixed4 frag (v2f_img  i) : SV_Target
			{

				init();
				float2 uv = (i.uv + _CurrentPosition.xy*32.) ;
				uv.y *= .5;
				
				//return float4(uv.x, uv.y, 0., 1.);
				
				uv.x += floor(uv.y / 1.2) * 3.;
				uv.y = uv.y % 1.;

				float4 col = mColors[NB_COLORS - 1];
				for (int i = 0; i < NB_COLORS; ++i)
				{
					col = lerp(col, mColors[i], step(uv.y + float(i)*.2, mountain(uv.x + pow(float(i), 1.1) * 30.14)));
				}

				return col;
			}
			ENDCG
		}
	}
}
