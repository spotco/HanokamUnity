Shader "Custom/SPSpriteDefault" {
    Properties {
        _MainTex ("_MainTex", 2D) = "white" {}
    }
    SubShader {
        Pass {
        Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		
		uniform sampler2D _MainTex;

		struct vertexInput {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};
		struct vertexOutput {
			float4 pos : POSITION;
			float2 tex : TEXCOORD0;
		};
		
		//http://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
		//http://docs.unity3d.com/462/Documentation/Manual/SL-BuiltinValues.html
		//http://wiki.unity3d.com/index.php/Shader_Code
		vertexOutput vert(vertexInput input) 
		{
			vertexOutput output;
			float2 out_tex = input.texcoord;
			output.tex = out_tex;
			output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
			return output;
		}
		float4 frag(vertexOutput input) : COLOR
		{
			float4 tex = tex2D(_MainTex, input.tex.xy);
			return tex;
		}

		ENDCG
        }
    }
}