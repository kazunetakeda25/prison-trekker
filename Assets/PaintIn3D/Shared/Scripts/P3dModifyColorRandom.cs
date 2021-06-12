using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to randomize the painting color of the attached component (e.g. P3dPaintDecal).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyColorRandom")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Color Random")]
	public class P3dModifyColorRandom : MonoBehaviour, IModify, IModifyColor
	{
		public enum BlendType
		{
			Replace,
			Multiply,
			Increment
		}

		/// <summary>This is the gradient containing all the possible colors. A color will be randomly picked from this.</summary>
		public Gradient Gradient { get { if (gradient == null) gradient = new Gradient(); return gradient; } } [SerializeField] private Gradient gradient;

		/// <summary>The way the picked color value will be blended with the current one.</summary>
		public BlendType Blend { set { blend = value; } get { return blend; } } [SerializeField] private BlendType blend;

		public void ModifyColor(float pressure, ref Color color)
		{
			if (gradient != null)
			{
				var pickedColor = gradient.Evaluate(Random.value);

				switch (blend)
				{
					case BlendType.Replace:
					{
						color = pickedColor;
					}
					break;

					case BlendType.Multiply:
					{
						color *= pickedColor;
					}
					break;

					case BlendType.Increment:
					{
						color += pickedColor;
					}
					break;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyColorRandom))]
	public class P3dModifyColorRandom_Editor : P3dEditor<P3dModifyColorRandom>
	{
		protected override void OnInspector()
		{
			Draw("gradient", "This is the gradient containing all the possible colors. A color will be randomly picked from this.");
			Draw("blend", "The way the picked color value will be blended with the current one.");
		}
	}
}
#endif