using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component raycasts between two points, and fires hit events when the ray hits something.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitBetween")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Between")]
	public class P3dHitBetween : P3dHitConnector
	{
		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		public enum NormalType
		{
			HitNormal,
			RayDirection
		}

		/// <summary>The start point of the raycast.</summary>
		public Transform PointA { set { pointA = value; } get { return pointA; } } [SerializeField] private Transform pointA;

		/// <summary>The end point of the raycast.</summary>
		public Transform PointB { set { pointB = value; } get { return pointB; } } [SerializeField] private Transform pointB;

		/// <summary>The end point of the raycast.</summary>
		public float Fraction { get { return fraction; } } [SerializeField] private float fraction = 1.0f;

		/// <summary>The layers you want the raycast to hit.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = Physics.DefaultRaycastLayers;

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation;

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>Which normal should the hit point rotation be based on?</summary>
		public NormalType Normal { set { normal = value; } get { return normal; } } [SerializeField] private NormalType normal;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The time in seconds between each raycast.
		/// 0 = every frame
		/// -1 = manual only</summary>
		public float Interval { set { interval = value; } get { return interval; } } [UnityEngine.Serialization.FormerlySerializedAs("delay")] [SerializeField] private float interval = 0.05f;

		/// <summary>This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.</summary>
		public float Pressure { set { pressure = value; } get { return pressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float pressure = 1.0f;

		/// <summary>Should the applied paint be applied as a preview?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>If you want to display something at the hit point (e.g. particles), you can specify the Transform here.</summary>
		public Transform Point { set { point = value; } get { return point; } } [SerializeField] private Transform point;

		/// <summary>If you want to draw a line between the start point and the his point then you can set the line here.</summary>
		public LineRenderer Line { set { line = value; } get { return line; } } [SerializeField] private LineRenderer line;

		[System.NonSerialized]
		private float current;

		[ContextMenu("Raycast Now")]
		public void RaycastNow()
		{
			if (pointA != null && pointB != null)
			{
				var vector      = pointB.position - pointA.position;
				var maxDistance = vector.magnitude;
				var ray         = new Ray(pointA.position, vector);
				var hit         = default(RaycastHit);

				if (Physics.Raycast(ray, out hit, maxDistance, layers) == true)
				{
					var finalUp       = orientation == OrientationType.CameraUp ? P3dHelper.GetCameraUp(_camera) : Vector3.up;
					var finalPosition = hit.point + hit.normal * offset;
					var finalNormal   = normal == NormalType.HitNormal ? hit.normal : -ray.direction;
					var finalRotation = Quaternion.LookRotation(finalNormal, finalUp);

					hitCache.InvokeRaycast(gameObject, null, null, preview, hit, pressure);

					DispatchHits(preview, hit.collider, finalPosition, finalRotation, pressure, this);

					fraction = (hit.distance + offset) / maxDistance;

					UpdatePointAndLine();
				}
				else
				{
					BreakHits(this);

					fraction = 1.0f;

					UpdatePointAndLine();
				}
			}
		}

		private void UpdatePointAndLine()
		{
			if (pointA != null && pointB != null)
			{
				var a = pointA.position;
				var b = pointB.position;
				var m = Vector3.Lerp(a, b, fraction);

				if (point != null)
				{
					point.position = m;
				}

				if (line != null)
				{
					line.positionCount = 2;

					line.SetPosition(0, a);
					line.SetPosition(1, m);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (point != null && pointB != null)
			{
				point.position = pointB.position;
			}
		}

		protected virtual void LateUpdate()
		{
			UpdatePointAndLine();
		}

		protected virtual void FixedUpdate()
		{
			if (interval >= 0.0f)
			{
				current += Time.fixedDeltaTime;

				if (current >= interval)
				{
					current = 0.0f;

					RaycastNow();
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitBetween))]
	public class P3dHitBetween_Editor : P3dHitConnector_Editor<P3dHitBetween>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PointA == null));
				Draw("pointA", "The start point of the raycast.");
			EndError();
			BeginError(Any(t => t.PointB == null));
				Draw("pointB", "The end point of the raycast.");
			EndError();
			BeginError(Any(t => t.Layers == 0));
				Draw("layers", "The layers you want the raycast to hit.");
			EndError();
			Draw("orientation", "How should the hit point be oriented?\nNone = It will be treated as a point with no rotation.\nWorldUp = It will be rotated to the normal, where the up vector is world up.\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			BeginIndent();
				if (Any(t => t.Orientation == P3dHitBetween.OrientationType.CameraUp))
				{
					Draw("_camera", "Orient to a specific camera?\nNone = MainCamera.");
				}
			EndIndent();
			Draw("normal", "Which normal should the hit point rotation be based on?");
			Draw("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			Draw("interval", "The time in seconds between each raycast.\n0 = every frame\n-1 = manual only");
			Draw("pressure", "This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.");
			Draw("preview", "Should the applied paint be applied as a preview?");

			base.OnInspector();

			Separator();

			Draw("point", "If you want to display something at the hit point (e.g. particles), you can specify the Transform here.");
			Draw("line", "If you want to draw a line between the start point and the his point then you can set the line here");

			Separator();

			Target.HitCache.Inspector(Target.gameObject, true, true, Target.ConnectHits);
		}
	}
}
#endif