// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/TwoSidedTexture" 
{
	Properties 
	{
		_FrontTex ("Front (RGB)", 2D) = "white" {}
		_BackTex  ("Back (RGB)", 2D) = "red" {}
	}

	SubShader 
	{
		Pass
		{
			Cull Off

			CGPROGRAM
		
			#pragma vertex vert 
			#pragma fragment frag 
		
			#include "UnityCG.cginc"

			sampler2D	_FrontTex;
			float4		_FrontTex_ST;
			sampler2D	_BackTex; 
			float4		_BackTex_ST;
			
			struct v2f 
			{ 
				float4 pos			: SV_POSITION;
				float2 texcoord0	: TEXCOORD0;
				float2 texcoord1	: TEXCOORD1;
				float3 opos			: TEXCOORD2;
				float3 onorm		: TEXCOORD3;
			};
					
			v2f vert (appdata_base v)
			{ 
				v2f o; 
				o.pos = UnityObjectToClipPos (v.vertex);
				o.texcoord0.xy = TRANSFORM_TEX(v.texcoord, _FrontTex);
				o.texcoord1.xy = TRANSFORM_TEX(v.texcoord, _BackTex);

				o.opos  = v.vertex.xyz;
				o.onorm = v.normal;

				return o; 
			} 

			float4 frag (v2f i) : COLOR 	
			{  
				float4 texColor;
				float3 P = i.opos;
				float3 N = normalize(i.onorm);
				float3 V = normalize(mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz - P);

				float isFront = dot(N, V);

				if (isFront >= 0.0f)
					texColor = tex2D(_FrontTex, i.texcoord0.xy);
				else
					texColor = tex2D(_BackTex, i.texcoord1.xy);
				
				return texColor;
			}		
		
			ENDCG
		}
	} 
	
	FallBack "Diffuse"
}