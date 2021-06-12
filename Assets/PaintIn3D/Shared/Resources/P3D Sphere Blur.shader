Shader "Hidden/Paint in 3D/Sphere Blur"
{
	Properties
	{
		_Hardness("Hardness", Float) = 1
		_Opacity("Opacity", Float) = 1
		_Squash("Squash", Float) = 1
		_KernelSize("Kernel Size", Float) = 0.01

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

				sampler2D _Buffer;
				float4    _Channel;
				float4x4  _Matrix;

				float _Opacity;
				float _Hardness;
				float _Squash;
				float _KernelSize;

				struct a2v
				{
					float4 vertex    : POSITION;
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

					float4 a       = tex2D(_Buffer, i.texcoord + float2(-_KernelSize, 0.0f));
					float4 b       = tex2D(_Buffer, i.texcoord + float2(_KernelSize, 0.0f));
					float4 c       = tex2D(_Buffer, i.texcoord + float2(0.0f, -_KernelSize));
					float4 d       = tex2D(_Buffer, i.texcoord + float2(0.0f, _KernelSize));
					float4 abcd    = (a + b + c + d) * 0.25f;
					float strength = 1.0f - pow(saturate(distance), _Hardness);

					o.color = lerp(tex2D(_Buffer, i.texcoord), abcd, _Opacity * strength);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader