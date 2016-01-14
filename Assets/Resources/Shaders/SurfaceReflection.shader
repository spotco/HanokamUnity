Shader "Custom/SurfaceReflection"
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
		_alpha_sub ("_alpha_sub", Float) = 0
		
		_y_mult_1 ("_y_mult_1", Float) = 100
		_y_mult_2 ("_y_mult_2", Float) = 300
		_wave_ampl_1 ("_wave_ampl_1", Float) = 0.05
		_wave_ampl_2 ("_wave_ampl_2", Float) = 0.0075
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
			fixed _alpha_sub;
			fixed _y_mult_1;
			fixed _y_mult_2;
			fixed _wave_ampl_1;
			fixed _wave_ampl_2;

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
				float yval = 1-IN.fade_alpha;
				wave_texcoord.x = wave_texcoord.x + (_wave_ampl_1 * sin(_Time[1] * 1.0 + wave_texcoord.y * _y_mult_1 * yval) * yval) + (_wave_ampl_2 * cos(_Time[1] * 5.0 + wave_texcoord.y * _y_mult_2 * yval) * yval);
				half4 color = tex2D(_MainTex, wave_texcoord) * IN.color *float4(0.9,1.2,1.4,clamp(IN.fade_alpha-_alpha_sub,0,1.0)) + float4(0.02,0.02,0.02,0.0);
				clip (color.a - 0.01);
				return color;
			}
		ENDCG
		}
	}
}
