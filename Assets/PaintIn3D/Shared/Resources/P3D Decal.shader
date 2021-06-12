Shader "Hidden/Paint in 3D/Decal"
{
	Properties
	{
		_ReplaceColor("Replace Color", Color) = (1, 1, 1, 1)
		_ReplaceTexture("Replace Texture", 2D) = "white" {}

		_Texture("Texture", 2D) = "white" {}
		_Shape("Shape", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Opacity("Opacity", Float) = 1
		_Hardness("Hardness", Float) = 1
		_NormalFront("Normal Front", Vector) = (1, 0, 0)
		_NormalBack("Normal Back", Vector) = (1, 0, 0)

		_Channel("Channel", Vector) = (1, 0, 0, 0)
		_Direction("Direction", Vector) = (1, 0, 0)
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
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile __ P3D_A // 0-1
				#pragma multi_compile __ P3D_B // 0-2
				#pragma multi_compile __ P3D_C // 0-4
				#pragma multi_compile __ P3D_D // 0-8
				#define BLEND_MODE_INDEX (P3D_A * 1 + P3D_B * 2 + P3D_C * 4 + P3D_D * 8)

				sampler2D _Buffer;
				float4    _Channel;
				float4x4  _Matrix;
				float4    _ReplaceColor;
				sampler2D _ReplaceTexture;

				float3    _Direction;
				sampler2D _Texture;
				sampler2D _Shape;
				float4    _Color;
				float     _Opacity;
				float     _Hardness;
				float2    _NormalFront;
				float2    _NormalBack;

				#include "UnityCG.cginc"
				#include "BlendModes.cginc"

				struct a2v
				{
					float4 vertex    : POSITION;
					float3 normal    : NORMAL;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					float2 texcoord2 : TEXCOORD2;
					float2 texcoord3 : TEXCOORD3;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float3 position : TEXCOORD1;
					float2 coord    : TEXCOORD2;
					float  normal   : TEXCOORD3;
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				void Vert(a2v i, out v2f o)
				{
					float2 texcoord    = i.texcoord0 * _Channel.x + i.texcoord1 * _Channel.y + i.texcoord2 * _Channel.z + i.texcoord3 * _Channel.w;
					float4 worldPos    = mul(unity_ObjectToWorld, i.vertex);
					float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, i.normal));

					o.vertex   = float4(texcoord.xy * 2.0f - 1.0f, 0.5f, 1.0f);
					o.texcoord = texcoord;
					o.position = mul(_Matrix, worldPos).xyz;
					o.coord    = o.position * 0.5f + 0.5f;
					o.normal   = dot(worldNormal, _Direction);
#if UNITY_UV_STARTS_AT_TOP
					o.vertex.y = -o.vertex.y;
#endif
				}

				void Frag(v2f i, out f2g o)
				{
					float4 color = tex2D(_Texture, i.coord) * _Color;
					float3 box   = saturate(abs(i.position));

					box.xy = pow(box.xy, 1000.0f); // Make edges with high hardness
					box.z  = pow(box.z, _Hardness); // Make depth with custom hardness

					// Inverted strength
					float strength = 1.0f - max(box.x, max(box.y, box.z));
					float fade     = 1.0f;

					fade -= saturate((i.normal - _NormalFront.x) * _NormalFront.y);
					fade += saturate((i.normal - _NormalBack.x) * _NormalBack.y);

					// Invert to multiplier
					strength *= fade;
#if BLEND_MODE_INDEX == 7 // REPLACE (Shape)
					strength *= tex2D(_Shape, i.coord).a;
#endif
					o.color = Blend(color, strength * _Opacity, _Buffer, i.texcoord);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader