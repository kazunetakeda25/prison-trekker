using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to debug hit points. A hit point can be found using a companion component like: P3dDragRaycast, P3dOnCollision, P3dOnParticleCollision.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintDebug")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Paint Debug")]
	public class P3dPaintDebug : MonoBehaviour, IHit, IHitPoint, IHitLine
	{
		/// <summary>The color of the debug.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The duration of the debug.</summary>
		public float Duration { set { duration = value; } get { return duration; } } [SerializeField] private float duration = 0.05f;

		/// <summary>The size of the debug.</summary>
		public float Size { set { size = value; } get { return size; } } [SerializeField] private float size = 0.05f;

		public void HandleHitPoint(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure)
		{
			var cornerA = worldPosition + worldRotation * new Vector3(-size, -size);
			var cornerB = worldPosition + worldRotation * new Vector3(-size,  size);
			var cornerC = worldPosition + worldRotation * new Vector3( size,  size);
			var cornerD = worldPosition + worldRotation * new Vector3( size, -size);
			var tint    = color;

			if (preview == true)
			{
				tint.a *= 0.5f;
			}

			tint.a *= pressure * 0.75f + 0.25f;

			Debug.DrawLine(cornerA, cornerB, tint, duration);
			Debug.DrawLine(cornerB, cornerC, tint, duration);
			Debug.DrawLine(cornerC, cornerD, tint, duration);
			Debug.DrawLine(cornerD, cornerA, tint, duration);
			Debug.DrawLine(worldPosition, worldPosition + worldRotation * Vector3.forward * size, tint, duration);
		}

		public void HandleHitLine(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB)
		{
			var tint = color;

			if (preview == true)
			{
				tint.a *= 0.5f;
			}

			tint.a *= pressureA * 0.75f + 0.25f;

			Debug.DrawLine(worldPositionA, worldPositionB, tint, duration);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintDebug))]
	public class P3dPaintDebug_Editor : P3dEditor<P3dPaintDebug>
	{
		protected override void OnInspector()
		{
			Draw("color", "The color of the debug.");
			BeginError(Any(t => t.Duration <= 0.0f));
				Draw("duration", "The duration of the debug.");
			EndError();
			BeginError(Any(t => t.Size <= 0.0f));
				Draw("size", "The size of the debug.");
			EndError();
		}
	}
}
#endif