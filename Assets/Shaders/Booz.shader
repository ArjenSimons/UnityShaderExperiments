Shader "Custom/Gradient" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_TopColor("Top Color", Color) = (1, 1, 1, 1)
		_FillAmount("Fill Amount", Range(0, 1)) = 0.7
		_MainTex("Main Texture", 2D) = "white" {}
	}

		SubShader{
			Pass {
				Cull off
				Blend SrcAlpha OneMinusSrcAlpha
				AlphaToMask On
				CGPROGRAM

				#pragma vertex vert 
				#pragma fragment frag 
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					return o;
				}

				uniform float _FillAmount;
				uniform sampler2D _MainTex;
				uniform half4 _Color;
				uniform half4 _TopColor;

				half4 frag(v2f i, fixed facing : VFACE) : COLOR
				{
					half4 col = facing > 0 ? _Color : _TopColor;
					col *= tex2D(_MainTex, i.uv);

					col.a = step(i.uv.y, _FillAmount);
					
					return col;
				}
			ENDCG
			}
	}
}