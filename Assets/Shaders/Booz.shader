Shader "Custom/Gradient" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimSize("Rim Size", Range(0, 5)) = 2
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
					float fillHeight : TEXCOORD1;
					float3 viewDir : COLOR;
					float3 normal : COLOR2;
				};

				uniform float _FillAmount;

				v2f vert(appdata_t v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
					o.fillHeight = worldPos.y - _FillAmount * 1.6 + 0.1;

					o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
					o.normal = v.normal;
					return o;
				}

				uniform sampler2D _MainTex;
				uniform half4 _Color;
				uniform half4 _RimColor;
				uniform float _RimSize;
				uniform half4 _TopColor;

				half4 frag(v2f i, fixed facing : VFACE) : COLOR
				{
					float dotProduct = 1 - pow(dot(i.normal, i.viewDir), + _RimSize);
					half4 rim = smoothstep(0.6, 1, dotProduct) * _RimColor;

					half4 fillCol = _Color + rim;
					half4 col = facing > 0 ? fillCol : _TopColor;
					col *= tex2D(_MainTex, i.uv);

					col.a = step(0.7, -i.fillHeight);
					
					return col;
				}
			ENDCG
			}
	}
}