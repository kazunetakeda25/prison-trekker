using UnityEngine;
using UnityEngine.Rendering;

namespace PaintIn3D
{
	/// <summary>This class caches shader information, speeding them up.</summary>
	public static class P3dShader
	{
		public static int _BaseChannel;
		public static int _BaseParallaxMap;
		public static int _BaseParallaxMap_Transform;
		public static int _Buffer;
		public static int _Channel;
		public static int _Color;
		public static int _Direction;
		public static int _DstA;
		public static int _DstRGB;
		public static int _Hardness;
		public static int _ParallaxMap;
		public static int _ParallaxMap_Transform;
		public static int _ReplaceTexture;
		public static int _ReplaceColor;
		public static int _Squash;
		public static int _KernelSize;
		public static int _MainTex;
		public static int _Matrix;
		public static int _NormalBack;
		public static int _NormalFront;
		public static int _Op;
		public static int _Opacity;
		public static int _Shape;
		public static int _SrcA;
		public static int _SrcRGB;
		public static int _Strength;
		public static int _Texture;
		public static int _Tiling;

		static P3dShader()
		{
			_BaseChannel = Shader.PropertyToID("_BaseChannel");
			_BaseParallaxMap = Shader.PropertyToID("_BaseParallaxMap");
			_BaseParallaxMap_Transform = Shader.PropertyToID("_BaseParallaxMap_Transform");
			_Buffer = Shader.PropertyToID("_Buffer");
			_Channel = Shader.PropertyToID("_Channel");
			_Color = Shader.PropertyToID("_Color");
			_Direction = Shader.PropertyToID("_Direction");
			_DstA = Shader.PropertyToID("_DstA");
			_DstRGB = Shader.PropertyToID("_DstRGB");
			_Hardness = Shader.PropertyToID("_Hardness");
			_ParallaxMap = Shader.PropertyToID("_ParallaxMap");
			_ParallaxMap_Transform = Shader.PropertyToID("_ParallaxMap_Transform");
			_ReplaceTexture = Shader.PropertyToID("_ReplaceTexture");
			_ReplaceColor = Shader.PropertyToID("_ReplaceColor");
			_Squash = Shader.PropertyToID("_Squash");
			_KernelSize = Shader.PropertyToID("_KernelSize");
			_MainTex = Shader.PropertyToID("_MainTex");
			_Matrix = Shader.PropertyToID("_Matrix");
			_NormalBack = Shader.PropertyToID("_NormalBack");
			_NormalFront = Shader.PropertyToID("_NormalFront");
			_Op = Shader.PropertyToID("_Op");
			_Opacity = Shader.PropertyToID("_Opacity");
			_Shape = Shader.PropertyToID("_Shape");
			_SrcA = Shader.PropertyToID("_SrcA");
			_SrcRGB = Shader.PropertyToID("_SrcRGB");
			_Strength = Shader.PropertyToID("_Strength");
			_Texture = Shader.PropertyToID("_Texture");
			_Tiling = Shader.PropertyToID("_Tiling");
		}

		public static Shader Load(string shaderName)
		{
			var shader = Shader.Find(shaderName);

			if (shader == null)
			{
				throw new System.Exception("Failed to find shader called: " + shaderName);
			}

			return shader;
		}

		public static Material Build(Shader shader)
		{
			var material = new Material(shader);
#if UNITY_EDITOR
			material.hideFlags = HideFlags.DontSave;
#endif
			return material;
		}

		public static void BuildBlendMode(Material material, int index)
		{
			if ((index & (1 << 0)) != 0)
			{
				material.EnableKeyword("P3D_A"); // 0-1
			}

			if ((index & (1 << 1)) != 0)
			{
				material.EnableKeyword("P3D_B"); // 0-2
			}

			if ((index & (1 << 2)) != 0)
			{
				material.EnableKeyword("P3D_C"); // 0-4
			}

			if ((index & (1 << 3)) != 0)
			{
				material.EnableKeyword("P3D_D"); // 0-8
			}
		}
	}
}