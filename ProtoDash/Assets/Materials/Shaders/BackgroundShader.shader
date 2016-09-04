// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable

Shader "Unlit/BackgroundShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (0.3, .00, .50, 1.)
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
			float4 _MainColor;
			
			//FRAGMENT
			// float4x4 _CameraToWorld;
#define LEVEL_NUMBER 10.

#define SF 15.

#define bsin(x) (sin(x) * .5 + .5)


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

			void applyMountain(inout float4 col, float height, float xCoord, float4 mColor)
			{
				float m = mountain(xCoord);
				col = lerp(col, mColor, step(height, m));
			}


			fixed4 frag (v2f_img  i) : SV_Target
			{
				float2 uv = (i.uv - _CurrentPosition.xy*.9 );
				uv.y += 20000.;
				uv *= 2.;
				
				uv.x += floor(uv.y / 1.2) * 3.;
				uv.y = uv.y % 1.25;

				float4 col = _MainColor;
				applyMountain(col, uv.y			, uv.x			,	_MainColor * .8);
				applyMountain(col, uv.y + .25	, uv.x + 7.6	,	_MainColor * .7);
				applyMountain(col, uv.y + .5	, uv.x + 62.2	,	_MainColor * .6);
				applyMountain(col, uv.y + .75	, uv.x + 1413.7	,	_MainColor);

				return col;
			}
			ENDCG
		}
	}
}
