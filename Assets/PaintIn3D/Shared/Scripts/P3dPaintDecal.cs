using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to paint a decal at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintDecal")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Decal")]
	public class P3dPaintDecal : MonoBehaviour, IHit, IHitPoint
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();

			public Vector3 Direction;
			public Color   Color;
			public float   Opacity;
			public float   Hardness;
			public Texture Texture;
			public Texture Shape;
			public Vector2 NormalFront;
			public Vector2 NormalBack;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material[] cachedMaterials;

			static Command()
			{
				cachedMaterials = P3dPaintableManager.BuildMaterialsBlendModes("Hidden/Paint in 3D/Decal");
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Direction   = Direction;
				command.Color       = Color;
				command.Opacity     = Opacity;
				command.Hardness    = Hardness;
				command.Texture     = Texture;
				command.Shape       = Shape;
				command.NormalFront = NormalFront;
				command.NormalBack  = NormalBack;

				return command;
			}

			public override void Apply()
			{
				Blend.Apply(Material);

				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetVector(P3dShader._Direction, Direction);
				Material.SetColor(P3dShader._Color, Color);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetFloat(P3dShader._Hardness, Hardness);
				Material.SetTexture(P3dShader._Texture, Texture);
				Material.SetTexture(P3dShader._Shape, Shape);
				Material.SetVector(P3dShader._NormalFront, NormalFront);
				Material.SetVector(P3dShader._NormalBack, NormalBack);
			}

			public override bool RequireMesh
			{
				get
				{
					return true;
				}
			}

			public override void Pool()
			{
				pool.Push(this);
			}

			public void SetLocation(Vector3 worldPosition, Quaternion worldRotation, Vector2 scale, float radius, Texture decal, float depth)
			{
				var worldSize = new Vector3(scale.x * radius, scale.y * radius, depth);

				if (decal != null)
				{
					if (decal.width > decal.height)
					{
						worldSize.y *= decal.height / (float)decal.width;
					}
					else
					{
						worldSize.x *= decal.width / (float)decal.height;
					}
				}

				Matrix    = Matrix4x4.TRS(worldPosition, worldRotation, worldSize);
				Position  = worldPosition;
				Radius    = (worldSize * 0.5f).magnitude;
				Direction = worldRotation * Vector3.forward;
			}

			public void SetMaterial(P3dBlendMode blendMode, Texture decal, float hardness, float normalBack, float normalFront, float normalFade, Color color, float opacity, Texture shape)
			{
				Blend    = blendMode;
				Material = cachedMaterials[blendMode];
				Color    = color;
				Opacity  = opacity;
				Hardness = hardness;
				Texture  = decal;
				Shape    = shape;

				var pointA = normalFront - 1.0f - normalFade;
				var pointB = normalFront - 1.0f;
				var pointC = 1.0f - normalBack;
				var pointD = 1.0f - normalBack + normalFade;

				NormalFront = new Vector2(pointA, P3dHelper.Reciprocal(pointB - pointA));
				NormalBack  = new Vector2(pointC, P3dHelper.Reciprocal(pointD - pointC));
			}

			public override void SetLocation(Matrix4x4 matrix)
			{
				base.SetLocation(matrix);

				Direction = matrix.MultiplyVector(Vector3.forward).normalized;
			}
		}

		/// <summary>Only the P3dModel/P3dPaintable GameObjects whose layers are within this mask will be eligible for painting.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>If this is set, then only the specified P3dModel/P3dPaintable will be painted, regardless of the layer setting.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>Only the P3dPaintableTextures whose groups are within this mask will be eligible for painting.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>If this is set, then only the specified P3dPaintableTexture will be painted, regardless of the layer or group setting.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

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

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.</summary>
		public float RadiusPressure { set { radiusPressure = value; } get { return radiusPressure; } } [SerializeField] private float radiusPressure;

		/// <summary>This allows you to control how near+far from the hit point the decal will appear in world space.
		/// If you're painting a flat surface head-on then you can use a low value here, but if you're painting something complex then you may need to set this higher.</summary>
		public float Depth { set { depth = value; } get { return depth; } } [SerializeField] private float depth = 0.1f;

		/// <summary>If you want the depth to increase with finger pressure, this allows you to set how much added depth is given at maximum pressure.</summary>
		public float DepthPressure { set { depthPressure = value; } get { return depthPressure; } } [SerializeField] private float depthPressure;

		/// <summary>This allows you to control the sharpness of the near+far depth cut-off point.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 3.0f;

		/// <summary>If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.</summary>
		public float HardnessPressure { set { hardnessPressure = value; } get { return hardnessPressure; } } [SerializeField] private float hardnessPressure;

		/// <summary>This allows you to control how much the paint can wrap around the front of surfaces.
		/// For example, if you want paint to wrap around curved surfaces then set this to a higher value.
		/// NOTE: If you set this to 0 then paint will not be applied to front facing surfaces.</summary>
		public float NormalFront { set { normalFront = value; } get { return normalFront; } } [Range(0.0f, 1.0f)] [SerializeField] private float normalFront = 0.2f;

		/// <summary>This works just like <b>Normal Front</b>, except for back facing surfaces.
		/// NOTE: If you set this to 0 then paint will not be applied to back facing surfaces.</summary>
		public float NormalBack { set { normalBack = value; } get { return normalBack; } } [Range(0.0f, 1.0f)] [SerializeField] private float normalBack;

		/// <summary>This allows you to control the smoothness of the normal cut-off point.</summary>
		public float NormalFade { set { normalFade = value; } get { return normalFade; } } [Range(0.001f, 0.25f)] [SerializeField] private float normalFade = 0.01f;

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

		/// <summary>This method multiplies the hardness by the specified value.</summary>
		public void MultiplyHardness(float multiplier)
		{
			hardness *= multiplier;
		}

		public void HandleHitPoint(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			var finalOpacity  = opacity + (1.0f - opacity) * opacityPressure * pressure;
			var finalRadius   = radius + radiusPressure * pressure;
			var finalDepth    = depth + depthPressure * pressure;
			var finalHardness = hardness + hardnessPressure * pressure;
			var finalColor    = color;
			var finalAngle    = angle;
			var finalTexture  = texture;

			P3dPaintableManager.BuildModifiers(gameObject);
			P3dPaintableManager.ModifyColor(pressure, ref finalColor);
			P3dPaintableManager.ModifyAngle(pressure, ref finalAngle);
			P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
			P3dPaintableManager.ModifyHardness(pressure, ref finalHardness);
			P3dPaintableManager.ModifyRadius(pressure, ref finalRadius);
			P3dPaintableManager.ModifyTexture(pressure, ref finalTexture);

			worldRotation = worldRotation * Quaternion.Euler(0.0f, 0.0f, finalAngle);

			Command.Instance.SetLocation(worldPosition, worldRotation, scale, finalRadius, finalTexture, finalDepth);

			Command.Instance.SetMaterial(blendMode, texture, finalHardness, normalBack, normalFront, normalFade, finalColor, finalOpacity, shape);

			P3dPaintableManager.SubmitAll(Command.Instance, preview, layers, groups, targetModel, targetTexture, repeaters, commands);
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = Matrix4x4.Translate(transform.position);

			Gizmos.DrawWireCube(Vector3.zero, new Vector3(radius, radius, depth));
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintDecal))]
	public class P3dPaintDecal_Editor : P3dEditor<P3dPaintDecal>
	{
		private bool expandLayers;
		private bool expandGroups;
		private bool expandOpacity;
		private bool expandRadius;
		private bool expandDepth;
		private bool expandHardness;

		protected override void OnInspector()
		{
			BeginError(Any(t => t.Layers == 0 && t.TargetModel == null));
				DrawExpand(ref expandLayers, "layers", "Only the P3dModel/P3dPaintable GameObjects whose layers are within this mask will be eligible for painting.");
			EndError();
			if (expandLayers == true || Any(t => t.TargetModel != null))
			{
				BeginIndent();
					Draw("targetModel", "If this is set, then only the specified P3dModel/P3dPaintable will be painted, regardless of the layer setting.");
				EndIndent();
			}
			BeginError(Any(t => t.Groups == 0 && t.TargetTexture == null));
				DrawExpand(ref expandGroups, "groups", "Only the P3dPaintableTextures whose groups are within this mask will be eligible for painting.");
			EndError();
			if (expandGroups == true || Any(t => t.TargetTexture != null))
			{
				BeginIndent();
					Draw("targetTexture", "If this is set, then only the specified P3dPaintableTexture will be painted, regardless of the layer or group setting.");
				EndIndent();
			}

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
			Draw("scale", "This allows you to control the mirroring and aspect ratio of the decal.\n\n1, 1 = No scaling.\n-1, 1 = Horizontal Flip.");
			BeginError(Any(t => t.Radius <= 0.0f));
				DrawExpand(ref expandRadius, "radius", "The radius of the paint brush.");
			EndError();
			if (expandRadius == true || Any(t => t.RadiusPressure != 0.0f))
			{
				BeginIndent();
					Draw("radiusPressure", "If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.", "Pressure");
				EndIndent();
			}
			BeginError(Any(t => t.Depth <= 0.0f));
				DrawExpand(ref expandDepth, "depth", "This allows you to control how near+far from the hit point the decal will appear in world space. If you're painting a flat surface head-on then you can use a low value here, but if you're painting something complex then you may need to set this higher.");
			EndError();
			if (expandDepth == true || Any(t => t.DepthPressure != 0.0f))
			{
				BeginIndent();
					Draw("depthPressure", "If you want the depth to increase with finger pressure, this allows you to set how much added depth is given at maximum pressure.", "Pressure");
				EndIndent();
			}
			BeginError(Any(t => t.Hardness <= 0.0f));
				DrawExpand(ref expandHardness, "hardness", "This allows you to control the sharpness of the near+far depth cut-off point.");
			EndError();
			if (expandHardness == true || Any(t => t.HardnessPressure != 0.0f))
			{
				BeginIndent();
					Draw("hardnessPressure", "If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.", "Pressure");
				EndIndent();
			}

			Separator();

			Draw("normalFront", "This allows you to control how much the paint can wrap around the front of surfaces (e.g. if you want paint to wrap around curved surfaces then set this to a higher value).\n\nNOTE: If you set this to 0 then paint will not be applied to front facing surfaces.");
			Draw("normalBack", "This works just like Normal Front, except for back facing surfaces.\n\nNOTE: If you set this to 0 then paint will not be applied to back facing surfaces.");
			Draw("normalFade", "This allows you to control the smoothness of the depth cut-off point.");
		}
	}
}
#endif