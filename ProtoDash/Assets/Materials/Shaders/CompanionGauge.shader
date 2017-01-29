Shader "Custom/CompanionGauge" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FillColor("Fill Color", Color) = (.9,.9,.9,1.)
		_ThresholdValue1("Threshold Value 1",Range(0,1)) = .75
		_ThresholdColor1("Threshold Color 1", Color) = (1.,1.,1.,1.)
		_ThresholdValue2("Threshold Value 2",Range(0,1)) = .75
		_ThresholdColor2("Threshold Color 2", Color) = (1.,1.,1.,1.)

		_GaugeProgression("Gauge Progression", Range(0,1)) = 1

	}
	SubShader
	{
		Tags{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True" }

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]

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

			float _GaugeProgression;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

#define NB_SECTIONS 15.
			float4	_FillColor;
			float4	_ThresholdColor1;
			float	_ThresholdValue1;
			float4	_ThresholdColor2;
			float	_ThresholdValue2;

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = floor(i.uv * NB_SECTIONS) / NB_SECTIONS;

				float4 col = _FillColor;
				col = lerp(col, _ThresholdColor1, step(_ThresholdValue1, _GaugeProgression));
				col = lerp(col, _ThresholdColor2, step(_ThresholdValue2, _GaugeProgression));

				float f = ((_GaugeProgression - uv.y) *NB_SECTIONS - uv.x)*NB_SECTIONS;
				f = clamp(f, 0., 1.);

				col.a *= f;

				return col;
			}
			ENDCG
		}
	}
}
