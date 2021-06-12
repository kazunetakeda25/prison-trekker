using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to randomize the painting hardness of the attached component (e.g. P3dPaintDecal).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyHardnessRandom")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Hardness Random")]
	public class P3dModifyHardnessRandom : MonoBehaviour, IModify, IModifyHardness
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>This is the minimum random hardness that will be picked.</summary>
		public float Min { set { min = value; } get { return min; } } [SerializeField] private float min = 0.6666f;

		/// <summary>This is the maximum random hardness that will be picked.</summary>
		public float Max { set { max = value; } get { return max; } } [SerializeField] private float max = 1.5f;

		/// <summary>The way the picked hardness value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyHardness(float pressure, ref float hardness)
		{
			var pickedHardness = Random.Range(min, max);

			switch (blend)
			{
				case BlendType.Replace:
				{
					hardness = pickedHardness;
				}
				break;

				case BlendType.Multiply:
				{
					hardness *= pickedHardness;
				}
				break;

				case BlendType.Increment:
				{
					hardness += pickedHardness;
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
	[CustomEditor(typeof(P3dModifyHardnessRandom))]
	public class P3dModifyHardnessRandom_Editor : P3dEditor<P3dModifyHardnessRandom>
	{
		protected override void OnInspector()
		{
			Draw("min", "This is the minimum random hardness that will be picked.");
			Draw("max", "This is the maximum random hardness that will be picked.");
			Draw("blend", "The way the picked hardness value will be blended with the current one.");
		}
	}
}
#endif