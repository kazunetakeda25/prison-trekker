using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to randomize the painting radius of the attached component (e.g. P3dPaintDecal).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyRadiusRandom")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Radius Random")]
	public class P3dModifyRadiusRandom : MonoBehaviour, IModify, IModifyRadius
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>This is the minimum random radius that will be picked.</summary>
		public float Min { set { min = value; } get { return min; } } [SerializeField] private float min = 0.6666f;

		/// <summary>This is the maximum random radius that will be picked.</summary>
		public float Max { set { max = value; } get { return max; } } [SerializeField] private float max = 1.5f;

		/// <summary>The way the picked radius value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyRadius(float pressure, ref float radius)
		{
			var pickedRadius = Random.Range(min, max);

			switch (blend)
			{
				case BlendType.Replace:
				{
					radius = pickedRadius;
				}
				break;

				case BlendType.Multiply:
				{
					radius *= pickedRadius;
				}
				break;

				case BlendType.Increment:
				{
					radius += pickedRadius;
				}
				break;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyRadiusRandom))]
	public class P3dModifyRadiusRandom_Editor : P3dEditor<P3dModifyRadiusRandom>
	{
		protected override void OnInspector()
		{
			Draw("min", "This is the minimum random radius that will be picked.");
			Draw("max", "This is the maximum random radius that will be picked.");
			Draw("blend", "The way the picked radius value will be blended with the current one.");
		}
	}
}
#endif