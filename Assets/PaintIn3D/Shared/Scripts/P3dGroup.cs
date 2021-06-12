using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This struct allows you to specify a group index with a group dropdown selector.</summary>
	[System.Serializable]
	public struct P3dGroup
	{
		[SerializeField]
		private int index;

		public P3dGroup(int newIndex)
		{
			if (newIndex <= 0)
			{
				index = 0;
			}
			else if (newIndex >= 31)
			{
				index = 31;
			}
			else
			{
				index = newIndex;
			}
		}

		public static implicit operator int(P3dGroup group)
		{
			return group.index;
		}

		public static implicit operator P3dGroup(int index)
		{
			return new P3dGroup(index);
		}

		public override string ToString()
		{
			return index.ToString();
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomPropertyDrawer(typeof(P3dGroup))]
	public partial class P3dGroup_Drawer : PropertyDrawer
	{
		public static void OnGUI(Rect position, P3dWindowPaintableTexture paintableTexture)
		{
			paintableTexture.Group = Mathf.Clamp(paintableTexture.Group, 0, 31);

			var handle = new GUIContent("Group " + paintableTexture.Group, "If you're painting multiple textures at the same time, you can put them on separate groups so only one brush can paint on it.");

			if (GUI.Button(position, handle, EditorStyles.popup) == true)
			{
				var menu = new GenericMenu();

				for (var i = 0; i < 32; i++)
				{
					var index   = i;
					var content = new GUIContent("Group " + i);
					var on      = paintableTexture.Group == index;

					menu.AddItem(content, on, () => { paintableTexture.Group = index; });
				}

				menu.DropDown(position);
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var sObj = property.serializedObject;
			var sPro = property.FindPropertyRelative("index");

			if (sPro.intValue < 0 || sPro.intValue > 31)
			{
				sPro.intValue = Mathf.Clamp(sPro.intValue, 0, 31);

				sObj.ApplyModifiedProperties();
			}

			var right  = position; right.xMin += EditorGUIUtility.labelWidth;
			var handle = "Group " + sPro.intValue;

			EditorGUI.LabelField(position, label);

			if (GUI.Button(right, handle, EditorStyles.popup) == true)
			{
				var menu = new GenericMenu();

				for (var i = 0; i < 32; i++)
				{
					var index   = i;
					var content = new GUIContent("Group " + i);
					var on      = sPro.intValue == index;

					menu.AddItem(content, on, () => { sPro.intValue = index; sObj.ApplyModifiedProperties(); });
				}

				menu.DropDown(right);
			}
		}
	}
}
#endif