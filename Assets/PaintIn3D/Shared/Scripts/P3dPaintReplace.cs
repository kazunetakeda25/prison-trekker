using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace paint mode, which will replace all pixels in the specified texture.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintReplace")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Replace")]
	public class P3dPaintReplace : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();
			
			public Texture Texture;
			public Color   Color;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material cachedMaterial;

			static Command()
			{
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Replace");
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Texture = Texture;
				command.Color   = Color;

				return command;
			}

			public override void Apply()
			{
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetColor(P3dShader._Color, Color);
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

			public void SetMaterial(Texture texture, Color color)
			{
				Blend    = P3dBlendMode.ReplaceCustom(color, texture);
				Material = cachedMaterial;
				Texture  = texture;
				Color    = color;
				Radius   = float.PositiveInfinity;
			}
		}

		/// <summary>Only the P3dPaintableTextures whose groups are within this mask will be eligible for painting.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>The texture that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		public static void Blit(RenderTexture renderTexture, Texture texture, Color color)
		{
			Command.Instance.SetMaterial(texture, color);

			Command.Instance.Apply();

			renderTexture.DiscardContents();

			P3dHelper.Blit(renderTexture, Command.Instance.Material);
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
						var finalTexture = texture;

						P3dPaintableManager.BuildModifiers(gameObject);
						P3dPaintableManager.ModifyColor(pressure, ref finalColor);
						P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

						Command.Instance.SetMaterial(finalTexture, finalColor);

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
	[CustomEditor(typeof(P3dPaintReplace))]
	public class P3dPaintReplace_Editor : P3dEditor<P3dPaintReplace>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Groups == 0));
				Draw("groups", "Only the P3dPaintableTextures whose groups are within this mask will be eligible for painting.");
			EndError();

			Separator();

			Draw("texture", "The texture that will be painted.");
			Draw("color", "The color of the paint.");
		}
	}
}
#endif