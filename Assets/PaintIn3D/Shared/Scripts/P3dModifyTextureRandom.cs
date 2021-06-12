using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to randomize the painting texture of the attached component (e.g. P3dPaintDecal).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModifyTextureRandom")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Modify/Modify Texture Random")]
	public class P3dModifyTextureRandom : MonoBehaviour, IModify, IModifyTexture
	{
		/// <summary>A random texture will be picked from this list.</summary>
		public List<Texture> Textures { get { if (textures == null) textures = new List<Texture>(); return textures; } } [SerializeField] private List<Texture> textures;

		public void ModifyTexture(float pressure, ref Texture texture)
		{
			if (textures != null && textures.Count > 0)
			{
				var pickedIndex = Random.Range(0, textures.Count);

				texture = textures[pickedIndex];
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModifyTextureRandom))]
	public class P3dModifyTextureRandom_Editor : P3dEditor<P3dModifyTextureRandom>
	{
		protected override void OnInspector()
		{
			Draw("textures", "A random texture will be picked from this list.");
		}
	}
}
#endif