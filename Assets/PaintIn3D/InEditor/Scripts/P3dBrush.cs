using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be added to a prefab, allowing it to be selected from the  Paint in 3D editor window.</summary>
	public class P3dBrush : MonoBehaviour, IModify, IModifyColor
	{
		/// <summary>This allows you to organize this brush into category with this name.</summary>
		public string Category { set { category = value; } get { return category; } } [SerializeField] private string category;

		/// <summary>This allows you to set the distance between each paint point in pixels.
		/// 0 = Once per frame.</summary>
		public float DragStep { set { dragStep = value; } get { return dragStep; } } [SerializeField] private float dragStep = 1;

		public Vector2 LastMousePosition { set { lastMousePosition = value; } get { return lastMousePosition; } } [SerializeField] private Vector2 lastMousePosition;

		public void ModifyColor(float pressure, ref Color color)
		{
#if UNITY_EDITOR
			switch (P3dWindowData.Instance.CurrentBlend)
			{
				case P3dWindowData.ColorBlendType.Replace:
				{
					color = P3dWindowData.Instance.CurrentColor;
				}
				break;

				case P3dWindowData.ColorBlendType.Multiply:
				{
					color *= P3dWindowData.Instance.CurrentColor;
				}
				break;
			}
#endif
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dBrush))]
	public class P3dBrush_Editor : P3dEditor<P3dBrush>
	{
		protected override void OnInspector()
		{
			Draw("category", "This allows you to organize this brush into category with this name.");
			Draw("dragStep", "This allows you to set the distance between each paint point in pixels.\n0 = Once per frame.");
		}
	}
}
#endif