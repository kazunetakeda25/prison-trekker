using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component grabs paint hits and connected hits, mirrors the data, then re-broadcasts it.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTransformMirror")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Transform/Transform Mirror")]
	public class P3dTransformMirror : P3dTransform
	{
		/// <summary>When a decal is mirrored it will appear backwards, should it be flipped back around?</summary>
		public bool Flip { set { flip = value; } get { return flip; } } [SerializeField] private bool flip;

		public override Matrix4x4 Repeat(Matrix4x4 matrix)
		{
			var tp = Matrix4x4.Translate(transform.position);
			var rp = Matrix4x4.Rotate(transform.rotation);
			var ti = Matrix4x4.Translate(-transform.position);
			var ri = Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation));
			var s = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, -1.0f));

			var plane = tp * rp * s * ri * ti;

			if (flip == true)
			{
				matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 180.0f, 0.0f), new Vector3(1.0f, 1.0f, -1.0f));
			}

			return plane * matrix;
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			for (var i = 1; i <= 10; i++)
			{
				Gizmos.DrawWireCube(Vector3.zero, new Vector3(i, i, 0.0f));
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dTransformMirror))]
	public class P3dTransformMirror_Editor : P3dEditor<P3dTransformMirror>
	{
		protected override void OnInspector()
		{
			Draw("flip", "When a decal is mirrored it will appear backwards, should it be flipped back around?");
		}
	}
}
#endif