using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to randomize the painting opacity of the attached component (e.g. P3dPaintDecal).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyOpacityRandom")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Opacity Random")]
	public class P3dModifyOpacityRandom : MonoBehaviour, IModify, IModifyOpacity
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>This is the minimum random opacity that will be picked.</summary>
		public float Min { set { min = value; } get { return min; } } [SerializeField] private float min = 0.6666f;

		/// <summary>This is the maximum random opacity that will be picked.</summary>
		public float Max { set { max = value; } get { return max; } } [SerializeField] private float max = 1.5f;

		/// <summary>The way the picked opacity value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyOpacity(float pressure, ref float opacity)
		{
			var pickedOpacity = Random.Range(min, max);

			switch (blend)
			{
				case BlendType.Replace:
				{
					opacity = pickedOpacity;
				}
				break;

				case BlendType.Multiply:
				{
					opacity *= pickedOpacity;
				}
				break;

				case BlendType.Increment:
				{
					opacity += pickedOpacity;
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
	[CustomEditor(typeof(P3dModifyOpacityRandom))]
	public class P3dModifyOpacityRandom_Editor : P3dEditor<P3dModifyOpacityRandom>
	{
		protected override void OnInspector()
		{
			Draw("min", "This is the minimum random opacity that will be picked.");
			Draw("max", "This is the maximum random opacity that will be picked.");
			Draw("blend", "The way the picked opacity value will be blended with the current one.");
		}
	}
}
#endif