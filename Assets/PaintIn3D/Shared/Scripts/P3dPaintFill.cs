using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace paint mode, which will modify all pixels in the specified texture in the same way.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintFill")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Fill")]
	public class P3dPaintFill : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();

			public Texture Texture;
			public Color   Color;
			public float   Opacity;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material[] cachedMaterials;

			static Command()
			{
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Fill");
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Texture = Texture;
				command.Color   = Color;
				command.Opacity = Opacity;

				return command;
			}

			public override void Apply()
			{
				Blend.Apply(Material);

				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
			}

			public override bool RequireMesh
			{
				get
				{
					return false;
				}
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture texture, Color color, float opacity)
			{
				Material = cachedMaterials[blendMode];
				Texture  = texture;
				Color    = color;
				Opacity  = opacity;
			}
		}

		public enum RotationType
		{
			World,
			Normal
		}

		/// <summary>Only the P3dPaintableTextures whose groups are within this mask will be eligible for painting.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>This component will paint using this blending mode.
		/// NOTE: See <b>P3dBlendMode</b> documentation for more information.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		/// <summary>The color of the paint.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.</summary>
		public float OpacityPressure { set { opacityPressure = value; } get { return opacityPressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacityPressure;

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		public static RenderTexture Blit(ref RenderTexture renderTexture, P3dBlendMode blendMode, Texture texture, Color color, float opacity)
		{
			Command.Instance.SetMaterial(blendMode, texture, color, opacity);

			Command.Instance.Apply();

			var swap = P3dHelper.GetRenderTexture(renderTexture.descriptor);

			Command.Instance.Material.SetTexture(P3dShader._Buffer, renderTexture);

			P3dHelper.Blit(swap, Command.Instance.Material);

			P3dHelper.ReleaseRenderTexture(renderTexture);

			renderTexture = swap;

			return swap;
		}

		public void HandleHitPoint(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			if (collider != null)
			{
				var model = collider.GetComponent<P3dModel>();

				if (model != null)
				{
					var paintableTextures = P3dPaintableTexture.Filter(model, groups);

					if (paintableTextures.Count > 0)
					{
						var finalColor   = color;
						var finalOpacity = opacity;
						var finalTexture = texture;

						P3dPaintableManager.BuildModifiers(gameObject);
						P3dPaintableManager.ModifyColor(pressure, ref finalColor);
						P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
						P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

						Command.Instance.SetMaterial(blendMode, finalTexture, finalColor, opacity);

						for (var i = paintableTextures.Count - 1; i >= 0; i--)
						{
							var paintableTexture = paintableTextures[i];

							P3dPaintableManager.Submit(Command.Instance, model, paintableTexture, preview);
						}
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintFill))]
	public class P3dPaintFill_Editor : P3dEditor<P3dPaintFill>
	{
		private bool expandOpacity;

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Groups == 0));
				Draw("groups", "Only the P3dPaintableTextures whose groups are within this mask will be eligible for painting.");
			EndError();

			Separator();

			Draw("blendMode", "This component will paint using this blending mode.\n\nNOTE: See P3dBlendMode documentation for more information.");
			Draw("texture", "The texture of the paint.");
			Draw("color", "The color of the paint.");
			DrawExpand(ref expandOpacity, "opacity", "The opacity of the brush.");
			if (expandOpacity == true || Any(t => t.OpacityPressure != 0.0f))
			{
				BeginIndent();
					Draw("opacityPressure", "If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.", "Pressure");
				EndIndent();
			}
		}
	}
}
#endif