Shader "Custom/CompanionGauge" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FillColor("Fill Color", Color) = (.9,.9,.9,1.)
		_ThresholdValue("Threshold Value",Range(0,1)) = .75
		_BorderColor("Border Color",Color) = (1.,1.,0.,1.)
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
#define BORDER_THIKNESS .05
			float4	_FillColor;
			float	_ThresholdValue;

			float4	_BorderColor;
			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = floor(i.uv * NB_SECTIONS) / NB_SECTIONS;
				float f = ((_GaugeProgression - uv.y) *NB_SECTIONS - uv.x)*NB_SECTIONS;
				f = clamp(f, 0., 1.);

				fixed4 col = float4(_FillColor.rgb,f);

				return col;
			}
			ENDCG
		}
	}
}
