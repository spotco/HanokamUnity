﻿Shader "Custom/UnderWaterViewSurface"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float fade_alpha : TEXCOORD1;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.fade_alpha = 1-IN.texcoord.y;
				
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 wave_texcoord = IN.texcoord;
				//float yval = IN.fade_alpha;
				//wave_texcoord.x = wave_texcoord.x + (_wave_ampl_1 * sin(_Time[1] * 1.0 + wave_texcoord.y * _y_mult_1 * yval) * yval) + (_wave_ampl_2 * cos(_Time[1] * 5.0 + wave_texcoord.y * _y_mult_2 * yval) * yval);
				float yval = 1-IN.fade_alpha;
				float inv_texcoord_y = 1-wave_texcoord.y;
				wave_texcoord.x = wave_texcoord.x + (0.01 * sin(_Time[1] + inv_texcoord_y * 100) * yval) * yval;
				
				half4 color = tex2D(_MainTex, wave_texcoord) * IN.color *float4(0.9,1.0,1.05,clamp(IN.fade_alpha+0.15,0,1.0)) + float4(0.02,0.02,0.02,0.0);
				
				return color;
			}
		ENDCG
		}
	}
}
