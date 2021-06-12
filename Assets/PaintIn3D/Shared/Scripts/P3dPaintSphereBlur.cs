using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to blur a sphere at a hit point. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintSphereBlur")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Sphere Blur")]
	public class P3dPaintSphereBlur : MonoBehaviour, IHit, IHitPoint, IHitLine
	{
		public class Command : P3dCommand
		{
			public static Command Instance = new Command();

			public float Opacity;
			public float Hardness;
			public float Squash;
			public float KernelSize;

			private static Stack<Command> pool = new Stack<Command>();

			private static Material cachedMaterial;

			private static P3dBlendMode cachedBlend = new P3dBlendMode() { Index = P3dBlendMode.ALPHA_BLEND };

			static Command()
			{
				cachedMaterial = P3dPaintableManager.BuildMaterial("Hidden/Paint in 3D/Sphere Blur");
			}

			public override P3dCommand SpawnCopy()
			{
				var command = SpawnCopy(pool);

				command.Opacity    = Opacity;
				command.Hardness   = Hardness;
				command.Squash     = Squash;
				command.KernelSize = KernelSize;

				return command;
			}

			public override void Apply()
			{
				Material.SetMatrix(P3dShader._Matrix, Matrix.inverse);
				Material.SetFloat(P3dShader._Opacity, Opacity);
				Material.SetFloat(P3dShader._Hardness, Hardness);
				Material.SetFloat(P3dShader._Squash, Squash);
				Material.SetFloat(P3dShader._KernelSize, KernelSize);
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

			public void SetLocation(Vector3 positionA, Vector3 positionB, float radius)
			{
				var direction = positionB - positionA;

				if (direction.sqrMagnitude > 0.0f && radius > 0.0f)
				{
					var middle   = (positionA + positionB) * 0.5f;
					var dist     = direction.magnitude * 0.5f;
					var length   = dist + radius;
					var rotation = dist > radius * 0.01f ? Quaternion.LookRotation(direction) : Quaternion.identity;

					SetLocation(middle, new Vector3(radius, radius, length), rotation);

					Squash = length / radius;
				}
				else
				{
					SetLocation(positionA, radius);
				}
			}

			public void SetLocation(Vector3 position, float radius)
			{
				var matrix = Matrix4x4.identity;

				matrix.m00 = matrix.m11 = matrix.m22 = radius;
				matrix.m03 = position.x;
				matrix.m13 = position.y;
				matrix.m23 = position.z;

				Matrix = matrix;
				Squash   = 1.0f;
				Position = position;
				Radius   = radius;
			}

			public void SetLocation(Vector3 position, Vector3 radius, Quaternion rotation)
			{
				Matrix   = Matrix4x4.TRS(position, rotation, radius);
				Squash   = 1.0f;
				Position = position;
				Radius   = Mathf.Max(radius.x, Mathf.Max(radius.y, radius.z));
			}

			public void SetMaterial(float hardness, float opacity, float kernelSize)
			{
				Blend      = cachedBlend;
				Material   = cachedMaterial;
				Hardness   = hardness;
				Opacity    = opacity;
				KernelSize = kernelSize;
			}
		}

		public enum RotationType
		{
			World,
			Normal
		}

		/// <summary>The layers you want this paint to apply to.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>The layers you want this paint to apply to.</summary>
		public P3dGroupMask Groups { set { groups = value; } get { return groups; } } [SerializeField] private P3dGroupMask groups = -1;

		/// <summary>If you only want to paint one specific model/paintable, rather than all of them in the scene, then specify it here.</summary>
		public P3dModel TargetModel { set { targetModel = value; } get { return targetModel; } } [SerializeField] private P3dModel targetModel;

		/// <summary>If you only want to paint one specific texture, rather than all of them in the scene, then specify it here.</summary>
		public P3dPaintableTexture TargetTexture { set { targetTexture = value; } get { return targetTexture; } } [SerializeField] private P3dPaintableTexture targetTexture;

		/// <summary>If you want to override the scale of the sphere to paint an ellipse, then set the scale here.</summary>
		public Vector3 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector3 scale = Vector3.one;

		/// <summary>This allows you to control how the ellipse is rotated.</summary>
		public RotationType RotateTo { set { rotateTo = value; } get { return rotateTo; } } [SerializeField] private RotationType rotateTo;

		/// <summary>The radius of the paint brush.</summary>
		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 0.1f;

		/// <summary>If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.</summary>
		public float RadiusPressure { set { radiusPressure = value; } get { return radiusPressure; } } [SerializeField] private float radiusPressure;

		/// <summary>The hardness of the paint brush.</summary>
		public float Hardness { set { hardness = value; } get { return hardness; } } [SerializeField] private float hardness = 1.0f;

		/// <summary>If you want the hardness to increase with finger pressure, this allows you to set how much added hardness is given at maximum pressure.</summary>
		public float HardnessPressure { set { hardnessPressure = value; } get { return hardnessPressure; } } [SerializeField] private float hardnessPressure;

		/// <summary>The opacity of the brush.</summary>
		public float Opacity { set { opacity = value; } get { return opacity; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacity = 1.0f;

		/// <summary>If you want the opacity to increase with finger pressure, this allows you to set how much added opacity is given at maximum pressure.</summary>
		public float OpacityPressure { set { opacityPressure = value; } get { return opacityPressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float opacityPressure;

		/// <summary>The blur kernel size.</summary>
		public float KernelSize { set { kernelSize = value; } get { return kernelSize; } } [SerializeField] private float kernelSize = 0.001f;

		/// <summary>If you want the kernel size to increase with finger pressure, this allows you to set how much added kernel size is given at maximum pressure.</summary>
		public float KernelSizePressure { set { kernelSizePressure = value; } get { return kernelSizePressure; } } [SerializeField] private float kernelSizePressure;

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void IncrementOpacity(float delta)
		{
			opacity = Mathf.Clamp01(opacity + delta);
		}

		/// <summary>This method multiplies the radius by the specified value.</summary>
		public void MultiplyRadius(float multiplier)
		{
			radius *= multiplier;
		}

		/// <summary>This method multiplies the scale by the specified value.</summary>
		public void MultiplyScale(float multiplier)
		{
			scale *= multiplier;
		}

		/// <summary>This allows you to paint all pixels within a sphere at the specified point.</summary>
		public void HandleHitPoint(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			var finalOpacity  = opacity + (1.0f - opacity) * opacityPressure * pressure;
			var finalRadius   = radius + radiusPressure * pressure;
			var finalHardness = hardness + hardnessPressure * pressure;

			P3dPaintableManager.BuildModifiers(gameObject);
			P3dPaintableManager.ModifyOpacity(pressure, ref finalOpacity);
			P3dPaintableManager.ModifyRadius(pressure, ref finalRadius);
			P3dPaintableManager.ModifyHardness(pressure, ref finalHardness);

			if (scale == Vector3.one) // Sphere?
			{
				Command.Instance.SetLocation(worldPosition, finalRadius);
			}
			else // Elipse?
			{
				Command.Instance.SetLocation(worldPosition, scale * finalRadius, rotateTo == RotationType.World ? worldRotation : Quaternion.identity);
			}

			Command.Instance.SetMaterial(finalHardness, finalOpacity, kernelSize);

			P3dPaintableManager.SubmitAll(Command.Instance, preview, layers, groups, targetModel, targetTexture, repeaters, commands);
		}

		/// <summary>This allows you to paint all pixels within a capsule between the specified points.</summary>
		public void HandleHitLine(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB)
		{
			var finalOpacityA  = opacity + (1.0f - opacity) * opacityPressure * pressureA;
			var finalRadiusA   = radius + radiusPressure * pressureA;
			var finalHardnessA = hardness + hardnessPressure * pressureA;
			//var finalOpacityB  = opacity + (1.0f - opacity) * opacityPressure * pressureB;
			//var finalRadiusB   = radius + radiusPressure * pressureB;
			//var finalHardnessB = hardness + hardnessPressure * pressureB;

			Command.Instance.SetLocation(worldPositionA, worldPositionB, finalRadiusA);

			Command.Instance.SetMaterial(finalHardnessA, finalOpacityA, kernelSize);

			P3dPaintableManager.SubmitAll(Command.Instance, preview, layers, groups, targetModel, targetTexture, repeaters, commands);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintSphereBlur))]
	public class P3dPaintSphereBlur_Editor : P3dEditor<P3dPaintSphereBlur>
	{
		private bool expandLayers;
		private bool expandGroups;
		private bool expandScale;
		private bool expandRadius;
		private bool expandHardness;
		private bool expandKernel;

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

			DrawExpand(ref expandScale, "scale", "If you want to override the scale of the sphere to paint an ellipse, then set the scale here.");
			if (expandScale == true || Any(t => t.Scale != Vector3.one))
			{
				BeginIndent();
					Draw("rotateTo", "This allows you to control how the ellipse is rotated.");
				EndIndent();
			}
			BeginError(Any(t => t.Radius <= 0.0f));
				DrawExpand(ref expandRadius, "radius", "The radius of the paint brush.");
			EndError();
			if (expandRadius == true || Any(t => t.RadiusPressure != 0.0f))
			{
				BeginIndent();
					Draw("radiusPressure", "If you want the radius to increase with finger pressure, this allows you to set how much added radius is given at maximum pressure.", "Pressure");
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
			DrawExpand(ref expandKernel, "kernelSize", "The blur kernel size.");
			if (expandKernel == true || Any(t => t.KernelSizePressure != 0.0f))
			{
				BeginIndent();
					Draw("kernelSizePressure", "If you want the kernel size to increase with finger pressure, this allows you to set how much added kernel size is given at maximum pressure.", "Pressure");
				EndIndent();
			}
		}
	}
}
#endif