#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	/// <summary>This class handles <b>Extras</b> tab for the main Paint in 3D window.</summary>
	public partial class P3dWindow : P3dEditorWindow
	{
		private void DrawTab3()
		{
			var data = P3dWindowData.Instance;

			data.MaxUndoSteps = EditorGUILayout.IntField("Max Undo Steps", data.MaxUndoSteps);

			if (GUILayout.Button("Save Settings Now") == true)
			{
				P3dWindowData.Save();
			}
		}
	}
}
#endif