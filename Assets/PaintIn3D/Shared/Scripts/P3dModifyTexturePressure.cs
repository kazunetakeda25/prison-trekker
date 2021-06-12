using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to change the painting texture of the attached component (e.g. P3dPaintDecal) based on the paint pressure.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyTexturePressure")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Texture Pressure")]
	public class P3dModifyTexturePressure : MonoBehaviour, IModify, IModifyTexture
	{
		/// <summary>The painting texture will be changed to this.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The paint pressure must be at least this value.</summary>
		public float PressureMin { set { pressureMin = value; } get { return pressureMin; } } [SerializeField] private float pressureMin = 0.5f;

		/// <summary>The paint pressure must be at most this value.</summary>
		public float PressureMax { set { pressureMax = value; } get { return pressureMax; } } [SerializeField] private float pressureMax = 1.0f;

		public void ModifyTexture(float pressure, ref Texture currentTexture)
		{
			if (pressure >= pressureMin && pressure <= pressureMax)
			{
				currentTexture = texture;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyTexturePressure))]
	public class P3dModifyTexturePressure_Editor : P3dEditor<P3dModifyTexturePressure>
	{
		protected override void OnInspector()
		{
			Draw("texture", "The painting texture will be changed to this.");
			Draw("pressureMin", "The paint pressure must be at least this value.");
			Draw("pressureMax", "The paint pressure must be at most this value.");
		}
	}
}
#endif