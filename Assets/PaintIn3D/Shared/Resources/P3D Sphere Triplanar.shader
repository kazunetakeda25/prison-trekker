Shader "Hidden/Paint in 3D/Sphere Triplanar"
{
	Properties
	{
		_ReplaceColor("Replace Color", Color) = (1, 1, 1, 1)
		_ReplaceTexture("Replace Texture", 2D) = "white" {}

		_Color("Color", Color) = (1, 1, 1, 1)
		_Opacity("Opacity", Float) = 1
		_Hardness("Hardness", Float) = 1
		_Squash("Squash", Float) = 1
		_Texture("Texture", 2D) = "white" {}
		_Strength("Strength", Float) = 1
		_Tiling("Tiling", Float) = 1

		_Channel("Channel", Vector) = (1, 0, 0, 0)
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

				sampler2D _Texture;
				float     _Strength;
				float     _Tiling;
				float4    _Color;
				float     _Opacity;
				float     _Hardness;
				float     _Squash;

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
					float3 uvw      : TEXCOORD2;
					float3 weights  : TEXCOORD3;
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				void Vert(a2v i, out v2f o)
				{
					float2 texcoord = i.texcoord0 * _Channel.x + i.texcoord1 * _Channel.y + i.texcoord2 * _Channel.z + i.texcoord3 * _Channel.w;
					float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
					o.vertex   = float4(texcoord.xy * 2.0f - 1.0f, 0.5f, 1.0f);
					o.texcoord = texcoord;
					o.position = mul(_Matrix, worldPos).xyz;
#if UNITY_UV_STARTS_AT_TOP
					o.vertex.y = -o.vertex.y;
#endif
					o.uvw = worldPos.xyz * _Tiling;
					float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, i.normal));
					o.weights = abs(worldNormal);
					o.weights /= o.weights.x + o.weights.y + o.weights.z;
				}

				void Frag(v2f i, out f2g o)
				{
					float distance;

					if (_Squash != 1.0f)
					{
						float3 positionA = float3(i.position.x, i.position.y, (i.position.z - 1.0f) * _Squash + 1.0f);
						float3 positionB = float3(i.position.x, i.position.y, (i.position.z + 1.0f) * _Squash - 1.0f);

						if (positionA.z > 0.0f) // Head
						{
							distance = length(positionA);
						}
						else if (positionB.z < 0.0f) // Tail
						{
							distance = length(positionB);
						}
						else // Body
						{
							distance = length(i.position.xy);
						}
					}
					else
					{
						distance = length(i.position);
					}

					float4 textureX = tex2D(_Texture, i.uvw.yz) * i.weights.x;
					float4 textureY = tex2D(_Texture, i.uvw.xz) * i.weights.y;
					float4 textureZ = tex2D(_Texture, i.uvw.xy) * i.weights.z;
					float4 color    = pow(textureX + textureY + textureZ, _Strength) * _Color;
					float  strength = 1.0f - pow(saturate(distance), _Hardness);

					o.color = Blend(color, strength * _Opacity, _Buffer, i.texcoord);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader