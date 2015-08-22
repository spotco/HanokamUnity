//http://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
//http://docs.unity3d.com/462/Documentation/Manual/SL-BuiltinValues.html
//http://wiki.unity3d.com/index.php/Shader_Code
//http://wiki.unity3d.com/index.php/VegetationTwoPass

Shader "Custom/SPSpriteDefault" {
    Properties {
        _MainTex ("_MainTex", 2D) = "white" {}
        _Color ("_Color", Color) = (1.0,1.0,1.0,1.0)
    }
    SubShader {
        Tags
        { 
            "Queue"="Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _Color;

			struct vertexInput {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : POSITION;
				float2 tex : TEXCOORD0;
			};
			
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
				return tex * _Color;
			}

			ENDCG
        }
    }
}