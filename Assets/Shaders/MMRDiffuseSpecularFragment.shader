// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/MMRDiffuseSpecularFragment" 
{
	Properties 
	{
		_LightPosition ("Light Position", Vector) = (-4.0, 7.0, -7.0, 0.0)
		_DiffuseColor ("Diffuse Color", Color) = (1,0,0)
		_SpecularColor ("Specular Color", Color) = (1,1,1)
		_AmbientColor ("Ambient Color", Color) = (0,1,0)
		_EmissiveColor ("Emissive Color", Color) = (0,0,0)
		_LightColor ("Light Color", Color) = (1,1,1)
		_Ambient ("Ambient Intensity", Range(0.0, 1.0)) = 0.0
		_Shininess ("Specular Shininess", Range(1, 255)) = 15
		
	}

	SubShader 
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag 
		
			#include "UnityCG.cginc"

			float4 _LightPosition;
			float3 _DiffuseColor;
			float3 _SpecularColor;
			float3 _AmbientColor;
			float3 _EmissiveColor;
			float3 _LightColor;
			float _Ambient;
			float _Shininess;
					
			struct v2f 
			{ 
				float4 pos   : SV_POSITION;
				float3 opos  : TEXCOORD0;
				float3 onorm : TEXCOORD1;
			};
		
			v2f vert (appdata_base v)
			{ 
				v2f o; 
				o.pos   = UnityObjectToClipPos(v.vertex); 
				o.opos  = v.vertex.xyz;
				o.onorm = v.normal;
				return o;
			} 
		
			half4 frag (v2f i) : COLOR 	
			{ 			
				float3 P = i.opos;
				float3 N = normalize(i.onorm);
				float3 L = normalize(_LightPosition - P);
				float3 V = normalize(mul((float3x3)unity_WorldToObject, _WorldSpaceCameraPos) - P);
				float3 H = normalize(L + V);
				
  				float3 emissive = _EmissiveColor;
				float3 ambient  = _AmbientColor * _Ambient;
  			
				float diffuseCoff = max(dot(N, L), 0);
				float3 diffuse = _DiffuseColor * _LightColor * diffuseCoff;
				
				float specularCoff = pow(max(dot(N, H), 0), _Shininess);
  				if (diffuseCoff <= 0) specularCoff = 0;
  				float3 specular = _SpecularColor * _LightColor * specularCoff;

				return half4 (emissive + ambient + diffuse + specular, 1); 
			}
		
			ENDCG
		}
	}
	 
	FallBack "Diffuse"
}

