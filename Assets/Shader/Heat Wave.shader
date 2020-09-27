//Shader "Custom/Distort"
//{
//	Properties
//	{
//		_MainTex("Noise", 2D) = "white" {}
//		_StrengthFilter("Strength Filter", 2D) = "white" {}
//		_Strength("Distort Strength", float) = 1.0
//		_Speed("Distort Speed", float) = 1.0
//	}
//
//	SubShader
//	{
//		Tags
//		{
//			"Queue" = "Transparent"
//			"DisableBatching" = "True"
//		}
//		
//		// Grab the screen behind the object into _BackgroundTexture
//		GrabPass
//		{
//			"_BackgroundTexture"
//		}
//
//		// Render the object with the texture generated above, and invert the colors
//		Pass
//		{
//			ZTest Always
//
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma target 3.0 
//			#include "UnityCG.cginc"
//
//			// Properties
//			sampler2D	_MainTex;
//			sampler2D	_StrengthFilter;
//			sampler2D	_BackgroundTexture;
//			float		_Speed;
//			float		_Strength;
//
//			struct vertexInput
//			{
//				float4 vertex : POSITION;
//				float2 texcoord : TEXCOORD0;
//			};
//			
//			struct vertexOutput
//			{
//				float4 pos : SV_POSITION;
//				float4 grabPos : TEXCOORD0;
//				float2 texcoord : TEXCOORD1;
//			};
//
//			// Random function mapping uv coordinate to [0, 1]
//			float random(float2 uv)
//			{
//				return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453123);
//			}
//
//			vertexOutput vert(vertexInput input)
//			{
//				vertexOutput output;
//
//				// billboard to camera
//				output.pos = UnityObjectToClipPos(input.vertex);
//
//				// use ComputeGrabScreenPos function from UnityCG.cginc
//				// to get the correct texture coordinate
//				output.grabPos = ComputeGrabScreenPos(output.pos);
//
//				// save texcoord
//				output.texcoord = input.texcoord;
//				
//				return output;
//			}
//
//			float4 frag(vertexOutput input) : COLOR
//			{
//				//return float4(1,1,1,1); // billboard test
//
//				// distort based on noise & strength filter
//				float noise = tex2D(_MainTex, input.texcoord).rgb;
//				float filt = tex2D(_StrengthFilter, input.texcoord).rgb;
//				input.grabPos.x += cos(noise * _Time.x *_Speed) * filt * _Strength;
//				input.grabPos.y += sin(noise * _Time.x *_Speed) * filt * _Strength;
//				return tex2Dproj(_BackgroundTexture, input.grabPos);
//			}
//			ENDCG
//		}
//	}
//}



Shader "Custom/Distort"
{
	Properties
	{
		_MainTex("Noise", 2D) = "white" {}
		_CameraOpaqueTexture("Camera Opaque Texture", 2D) = "white" {}
		_StrengthFilter("Strength Filter", 2D) = "white" {}
		_Strength("Distort Strength", float) = 1.0
		_Speed("Distort Speed", float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"DisableBatching" = "True"
		}

		// Grab the screen behind the object into _BackgroundTexture
		/*GrabPass
		{
			"_CameraOpaqueTexture"
		}*/
		// Render the object with the texture generated above, and invert the colors
		Pass
		{
			ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0 
			#include "UnityCG.cginc"
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			// Properties
			sampler2D	_MainTex;
			sampler2D	_StrengthFilter;
			sampler2D	_CameraOpaqueTexture;
			float		_Speed;
			float		_Strength;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
				float2 texcoord : TEXCOORD1;
			};

			// Random function mapping uv coordinate to [0, 1]
			float random(float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453123);
			}

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// billboard to camera
				output.pos = UnityObjectToClipPos(input.vertex);

				// use ComputeGrabScreenPos function from UnityCG.cginc
				// to get the correct texture coordinate
				output.grabPos = ComputeGrabScreenPos(output.pos);

				// save texcoord
				output.texcoord = input.texcoord;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				//return float4(1,1,1,1); // billboard test

				// distort based on noise & strength filter
				float noise = tex2D(_MainTex, input.texcoord).rgb;
				float filt = tex2D(_StrengthFilter, input.texcoord).rgb;
				input.grabPos.x += cos(noise * _Time.x *_Speed) * filt * _Strength;
				input.grabPos.y += sin(noise * _Time.x *_Speed) * filt * _Strength;
				return tex2Dproj(_CameraOpaqueTexture, input.grabPos);
			}
			ENDCG
		}
	}
}