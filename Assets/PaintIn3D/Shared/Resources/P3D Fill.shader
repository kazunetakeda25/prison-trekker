Shader "Hidden/Paint in 3D/Fill"
{
	Properties
	{
		_ReplaceColor("Replace Color", Color) = (1, 1, 1, 1)
		_ReplaceTexture("Replace Texture", 2D) = "white" {}

		_Texture("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Opacity("Opacity", Float) = 1
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Transparent"
			"RenderType"      = "Transparent"
			"IgnoreProjector" = "True"
			"Paint in 3D"     = "True"
		}
		Pass
		{
			Blend One Zero
			Cull Off
			Lighting Off
			ZWrite Off

			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment Frag
				#pragma multi_compile __ P3D_A // 0-1
				#pragma multi_compile __ P3D_B // 0-2
				#pragma multi_compile __ P3D_C // 0-4
				#pragma multi_compile __ P3D_D // 0-8
				#define BLEND_MODE_INDEX (P3D_A * 1 + P3D_B * 2 + P3D_C * 4 + P3D_D * 8)

				sampler2D _Buffer;
				float4    _ReplaceColor;
				sampler2D _ReplaceTexture;

				sampler2D _Texture;
				float4    _Color;
				float     _Opacity;

				#include "UnityCG.cginc"
				#include "BlendModes.cginc"

				struct f2g
				{
					float4 color : COLOR;
				};

				void Frag(v2f_img i, out f2g o)
				{
					float4 color = tex2D(_Texture, i.uv) * _Color;

					//o.color = Blend(color, _Opacity, _Buffer, i.uv);
					o.color = BlendMinimum(color, 1.0f, _Buffer, i.uv, _Opacity);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader