using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to randomize the painting angle of the attached component (e.g. P3dPaintDecal).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyAngleRandom")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Angle Random")]
	public class P3dModifyAngleRandom : MonoBehaviour, IModify, IModifyAngle
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>This is the minimum random angle that will be picked.</summary>
		public float Min { set { min = value; } get { return min; } } [SerializeField] private float min = -180.0f;

		/// <summary>This is the maximum random angle that will be picked.</summary>
		public float Max { set { max = value; } get { return max; } } [SerializeField] private float max = 180.0f;

		/// <summary>The way the picked angle value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyAngle(float pressure, ref float angle)
		{
			var pickedAngle = Random.Range(min, max);

			switch (blend)
			{
				case BlendType.Replace:
				{
					angle = pickedAngle;
				}
				break;

				case BlendType.Multiply:
				{
					angle *= pickedAngle;
				}
				break;

				case BlendType.Increment:
				{
					angle += pickedAngle;
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
	[CustomEditor(typeof(P3dModifyAngleRandom))]
	public class P3dModifyAngleRandom_Editor : P3dEditor<P3dModifyAngleRandom>
	{
		protected override void OnInspector()
		{
			Draw("min", "This is the minimum random angle that will be picked.");
			Draw("max", "This is the maximum random angle that will be picked.");
			Draw("blend", "The way the picked angle value will be blended with the current one.");
		}
	}
}
#endif