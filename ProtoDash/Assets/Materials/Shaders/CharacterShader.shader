// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CharacterShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ThresholdValue1("Threshold Value 1",Range(0,1)) = .75
		_ThresholdColor1("Threshold Color 1", Color) = (1.,1.,1.,1.)
		_ThresholdValue2("Threshold Value 2",Range(0,1)) = .75
		_ThresholdColor2("Threshold Color 2", Color) = (1.,1.,1.,1.)
		_ThresholdValue3("Threshold Value 3",Range(0,1)) = .75
		_ThresholdColor3("Threshold Color 3", Color) = (1.,1.,1.,1.)
		_Color("Color", Color) = (1.,1.,1.,1.)
	_Progression("Progression", Range(0,1)) = .5

	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag alpha
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _Color;
			float4	_ThresholdColor1;
			float	_ThresholdValue1;
			float4	_ThresholdColor2;
			float	_ThresholdValue2;
			float4	_ThresholdColor3;
			float	_ThresholdValue3;
			float _Progression;
#define B .4


			fixed4 frag (v2f i) : SV_Target
			{

				float d = distance(i.uv, float2(.5, .5));
				
				float f = 1. - smoothstep(.49, .51, d);
				f -= 1. - smoothstep(B, B + .02, d);
				float p = (_Progression * B);
				f += 1. - smoothstep(p, p + .02, d);

				float4 col = _ThresholdColor1;
				col = lerp(col, _ThresholdColor2, step(_ThresholdValue1, _Progression));
				col = lerp(col, _ThresholdColor3, step(_ThresholdValue2, _Progression));
				col = lerp(col, _Color, step(_ThresholdValue3, _Progression));
				col *= f;

				return col;
			}
			ENDCG
		}
	}
}
