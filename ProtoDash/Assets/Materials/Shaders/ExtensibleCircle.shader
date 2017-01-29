Shader "Unlit/ExtensibleMaterial" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Ratio("Ratio", Float) = 1.
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			Stencil{
				Comp always
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
				float2 uv : TEXCOORD0;
				half invRatio:Any;
			};

			fixed4 _Color;
			float _Ratio;
			//float invRatio;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;// -float2(.5f, .5f);
				//o.uv.x /= _Ratio;
				o.invRatio = 1.f / _Ratio - .5f;
				return o;
			}

#define _Smooth(p,r,s) smoothstep(-s, s, p-(r))

			fixed4 frag(v2f i) : COLOR
			{
				float2 uv = i.uv;
				uv.x /= _Ratio;
				uv.x = min(uv.x,.5) * step(i.uv.x,.5) + (max(i.invRatio, uv.x) + (.5 - i.invRatio )) * step(.5,i.uv.x);
				float d = distance(uv, float2(.5, .5));
				float4 col = _Color;
				col.a *= 1.-_Smooth(d, .49, .01);
				return col;
			}
			ENDCG
		}
	}
}
