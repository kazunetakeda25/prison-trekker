using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a decal at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintDirectDecal")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Direct Decal")]
	public class P3dPaintDirectDecal : MonoBehaviour, IHit, IHitRaycast
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();

			public Color    Color;
			public float    Opacity;
			public Texture  Texture;
			public Texture  Shape;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material[] cachedMaterials;

			static Command()
			{
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Direct Decal");
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Color   = Color;
				command.Opacity = Opacity;
				command.Texture = Texture;
				command.Shape   = Shape;

				return command;
			}

			public override void Apply()
			{
				Blend.Apply(Material);

				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetTexture(P3dShader._Shape, Shape);
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

			public void SetLocation(Texture canvas, Vector2 coord, float angle, Vector2 scale, float radius, Texture decal)
			{
				var size = new Vector3(scale.x * radius, scale.y * radius, 1.0f);

				if (decal != null)
				{
					if (decal.width > decal.height)
					{
						size.y *= decal.height / (float)decal.width;
					}
					else
					{
						size.x *= decal.width / (float)decal.height;
					}
				}

				Matrix = Matrix4x4.TRS(coord, Quaternion.Euler(0.0f, 0.0f, angle), new Vector3(size.x, size.y, 1.0f));

				if (canvas != null)
				{
					var matrixB = Matrix4x4.Scale(new Vector3(1.0f / canvas.width, 1.0f / canvas.height, 1.0f));

					coord.x *= canvas.width;
					coord.y *= canvas.height;

					Matrix = Matrix4x4.TRS(coord, Quaternion.Euler(0.0f, 0.0f, angle), new Vector3(size.x, size.y, 1.0f));

					Matrix = matrixB * Matrix;
				}
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture decal, Color color, float opacity, Texture shape)
			{
				Blend    = blendMode;
				Material = cachedMaterials[blendMode];
				Color    = color;
				Opacity  = opacity;
				Texture  = decal;
				Shape    = shape;
			}
		}

		/// <summary>The groups you want this paint to apply to.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>This component will paint using this blending mode.
		/// NOTE: See <b>P3dBlendMode</b> documentation for more information.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode = P3dBlendMode.AlphaBlend;

		/// <summary>The decal that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The shape of the decal when using Replace blending.</summary>
		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.</summary>
		public float OpacityPressure { set { opacityPressure = value; } get { return opacityPressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacityPressure;

		/// <summary>The angle of the decal in degrees.</summary>
		public float Angle { set { angle = value; } get { return angle; } } [Range(-180.0f, 180.0f)] [SerializeField] private float angle;

		/// <summary>This allows you to control the mirroring and aspect ratio of the decal.
		/// 1, 1 = No scaling.
		/// -1, 1 = Horizontal Flip.</summary>
		public Vector2 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector2 scale = Vector2.one;

		/// <summary>The radius of the paint brush in pixels.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 32.0f;

		/// <summary>If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.</summary>
		public float RadiusPressure { set { radiusPressure = value; } get { return radiusPressure; } } [SerializeField] private float radiusPressure;

		/// <summary>This method will invert the scale.x value.</summary>
		[ContextMenu("Flip Horizontal")]
		public void FlipHorizontal()
		{
			scale.x = -scale.x;
		}

		/// <summary>This method will invert the scale.y value.</summary>
		[ContextMenu("Flip Vertical")]
		public void FlipVertical()
		{
			scale.y = -scale.y;
		}

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		/// <summary>This method increments the angle by the specified amount of degrees, and wraps it to the -180..180 range.</summary>
		public void IncrementAngle(float degrees)
		{
			angle = Mathf.Repeat(angle + 180.0f + degrees, 360.0f) - 180.0f;
		}

		/// <summary>This method multiplies the scale by the specified value.</summary>
		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		public void HandleHitRaycast(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, RaycastHit hit, float pressure)
		{
			var model = hit.collider.GetComponent<P3dModel>();

			if (model != null)
			{
				var paintableTextures = P3dPaintableTexture.Filter(model, groups);
				var finalOpacity      = opacity + (1.0f - opacity) * opacityPressure * pressure;
				var finalRadius       = radius + radiusPressure * pressure;
				var finalColor        = color;
				var finalAngle        = angle;
				var finalTexture      = texture;

				P3dPaintableManager.BuildModifiers(gameObject);
				P3dPaintableManager.ModifyColor(pressure, ref finalColor);
				P3dPaintableManager.ModifyAngle(pressure, ref finalAngle);
				P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
				P3dPaintableManager.ModifyRadius(pressure, ref finalRadius);
				P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

				Command.Instance.SetMaterial(blendMode, finalTexture, finalColor, finalOpacity, shape);

				for (var i = paintableTextures.Count - 1; i >= 0; i--)
				{
					var paintableTexture = paintableTextures[i];
					var coord            = default(Vector2);

					switch (paintableTexture.Channel)
					{
						case P3dChannel.UV:
						{
							coord = hit.textureCoord;
						}
						break;

						case P3dChannel.UV2:
						{
							coord = hit.textureCoord2;
						}
						break;
					}

					Command.Instance.SetLocation(paintableTexture.Current, coord, finalAngle, scale, finalRadius, texture);

					P3dPaintableManager.Submit(Command.Instance, model, paintableTexture, preview);
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = Matrix4x4.Translate(transform.position);

			Gizmos.DrawWireCube(Vector3.zero, new Vector3(radius, radius, 0.0f));
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintDirectDecal))]
	public class P3dPaintDirectDecal_Editor : P3dEditor<P3dPaintDirectDecal>
	{
		private bool expandOpacity;
		private bool expandRadius;

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Groups == 0));
				Draw("groups", "The groups you want this paint to apply to.");
			EndError();

			Separator();

			Draw("blendMode", "This component will paint using this blending mode.\n\nNOTE: See P3dBlendMode documentation for more information.");
			BeginError(Any(t => t.Texture == null));
				Draw("texture", "The decal that will be painted.");
			EndError();
			if (Any(t => t.BlendMode == P3dBlendMode.REPLACE))
			{
				BeginError(Any(t => t.Shape == null));
					Draw("shape", "The shape of the decal when using Replace blending.");
				EndError();
			}
			Draw("color", "The color of the paint.");
			DrawExpand(ref expandOpacity, "opacity", "The opacity of the brush.");
			if (expandOpacity == true || Any(t => t.OpacityPressure != 0.0f))
			{
				BeginIndent();
					Draw("opacityPressure", "If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.", "Pressure");
				EndIndent();
			}

			Separator();

			Draw("angle", "The angle of the decal in degrees.");
			Draw("scale", "This allows you to control the mirroring and aspect ratio of the decal.\n1, 1 = No scaling.\n-1, 1 = Horizontal Flip.");
			BeginError(Any(t => t.Radius <= 0.0f));
				DrawExpand(ref expandRadius, "radius", "The radius of the paint brush.");
			EndError();
			if (expandRadius == true || Any(t => t.RadiusPressure != 0.0f))
			{
				BeginIndent();
					Draw("radiusPressure", "If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.", "Pressure");
				EndIndent();
			}
		}
	}
}
#endif