// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Texture" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
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

			sampler2D	_MainTex;
			float4		_MainTex_ST;	// Tiling size and offset
					
			struct v2f 
			{ 
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD;
			};
		
			v2f vert (appdata_base v)
			{ 
				v2f o; 
				o.pos = UnityObjectToClipPos (v.vertex);

				o.texcoord.xy = v.texcoord.xy;

				return o; 
			} 

			float4 frag (v2f i) : COLOR 	
			{  
				float4 texColor = tex2D(_MainTex, 
					_MainTex_ST.xy * i.texcoord.xy + _MainTex_ST.zw);
				
				return texColor;
			}		
		
			ENDCG
		}
	} 
	
	FallBack "Diffuse"
}
