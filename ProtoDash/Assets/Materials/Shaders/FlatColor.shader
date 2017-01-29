Shader "Unlit/FlatColor" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
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
				//float2 uv : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}


			fixed4 frag(v2f i) : COLOR
			{
				float4 col = _Color;
				return col;
			}
			ENDCG
		}
	}
}
