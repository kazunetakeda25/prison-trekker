#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This class handles <b>Painting</b> tab for the main Paint in 3D window.</summary>
	public partial class P3dWindow : P3dEditorWindow
	{
		private static bool expandFavouriteColors;

		private static bool expandFavouriteBrushes;

		private static bool expandUncategorizedBrushes;

		private static List<string> Expanded = new List<string>();

		private static List<P3dBrush> Locked = new List<P3dBrush>();

		private static List<P3dBrush> tempFavourites = new List<P3dBrush>();

		private static List<P3dBrush> cachedBrushes;

		private void DrawTab2()
		{
			if (cachedBrushes == null || GUILayout.Button("Refresh Brushes") == true)
			{
				cachedBrushes = AssetDatabase.FindAssets("t:GameObject").Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)).GetComponent<P3dBrush>()).Where(b => b != null).ToList();
			}

			cachedBrushes.RemoveAll(b => b == null);

			var categories = cachedBrushes.Select(b => b.Category).Where(c => string.IsNullOrEmpty(c) == false).Distinct();

			if (CanPaint() == true)
			{
				EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup(CanUndo() == false);
						if (GUILayout.Button("Undo") == true)
						{
							Undo();
						}
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(CanWrite() == false);
						P3dHelper.BeginColor(NeedsWrite() == true ? Color.green : GUI.color);
							if (GUILayout.Button(new GUIContent("Write", "If you're editing imported textures (e.g. .png), then they must be written to apply changes.")) == true)
							{
								Write();
							}
						P3dHelper.EndColor();
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(CanRedo() == false);
						if (GUILayout.Button("Redo") == true)
						{
							Redo();
						}
					EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();

				if (P3dWindowData.Instance.CurrentBrushesSafe.Count == 0)
				{
					EditorGUILayout.HelpBox("Before you can paint, you need to select a brush you want to paint with.", MessageType.Info);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("Before you can paint, you need to lock at least one texture.", MessageType.Info);
			}

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Brush Color", EditorStyles.boldLabel);
			DrawCurrentColor();

			EditorGUILayout.Separator();
			
			DrawFavouriteColors();
			DrawFavouriteBrushes();
			DrawUncategorizedBrushes(cachedBrushes);

			foreach (var category in categories)
			{
				DrawBrushes(cachedBrushes, category);
			}
		}

		private GUIContent blendContentA = new GUIContent("Original", "This will use the original brush colors, ignoring your current color selection.");
		private GUIContent blendContentB = new GUIContent("Replace", "This will replace the brush colors with your current color selection.");
		private GUIContent blendContentC = new GUIContent("Multiply", "This will multiply the brush colors with your current color selection.");

		private void DrawCurrentColor()
		{
			var rectT   = P3dHelper.Reserve();
			var rect    = P3dHelper.Reserve();
			var rectA   = GetRect(rect, rect.xMin + 20, rect.xMax);
			var rectS   = GetRect(rect, rect.xMin, rect.xMin + 18);
			var starred = P3dWindowData.Instance.FavouriteColors.Contains(P3dWindowData.Instance.CurrentColor);

			P3dHelper.BeginColor(Color.gray, P3dWindowData.Instance.CurrentBlend != P3dWindowData.ColorBlendType.None);
				if (GUI.Button(GetRect(rectT, rectT.xMin + rectT.width / 3 * 0, rectT.xMin + rectT.width / 3 * 1), blendContentA, EditorStyles.miniButtonLeft) == true)
				{
					P3dWindowData.Instance.CurrentBlend = P3dWindowData.ColorBlendType.None;
				}
			P3dHelper.EndColor();

			P3dHelper.BeginColor(Color.gray, P3dWindowData.Instance.CurrentBlend != P3dWindowData.ColorBlendType.Replace);
				if (GUI.Button(GetRect(rectT, rectT.xMin + rectT.width / 3 * 1, rectT.xMin + rectT.width / 3 * 2), blendContentB, EditorStyles.miniButtonMid) == true)
				{
					P3dWindowData.Instance.CurrentBlend = P3dWindowData.ColorBlendType.Replace;
				}
			P3dHelper.EndColor();

			P3dHelper.BeginColor(Color.gray, P3dWindowData.Instance.CurrentBlend != P3dWindowData.ColorBlendType.Multiply);
				if (GUI.Button(GetRect(rectT, rectT.xMin + rectT.width / 3 * 2, rectT.xMin + rectT.width / 3 * 3), blendContentC, EditorStyles.miniButtonRight) == true)
				{
					P3dWindowData.Instance.CurrentBlend = P3dWindowData.ColorBlendType.Multiply;
				}
			P3dHelper.EndColor();

			P3dWindowData.Instance.CurrentColor = EditorGUI.ColorField(rectA, GUIContent.none, P3dWindowData.Instance.CurrentColor);

			if (DrawStarButton(rectS, starred) == true)
			{
				if (starred == true)
				{
					P3dWindowData.Instance.FavouriteColors.Remove(P3dWindowData.Instance.CurrentColor);
				}
				else
				{
					P3dWindowData.Instance.FavouriteColors.Add(P3dWindowData.Instance.CurrentColor);
				}
			}
		}

		private void DrawFavouriteColors()
		{
			if (GUILayout.Button("Favourite Colors", expandFavouriteColors == true ? EditorStyles.toolbarDropDown : EditorStyles.toolbarButton) == true)
			{
				expandFavouriteColors = !expandFavouriteColors;
			}

			if (expandFavouriteColors == true)
			{
				var count = 0;

				for (var i = 0; i < P3dWindowData.Instance.FavouriteColors.Count; i++)
				{
					var color = P3dWindowData.Instance.FavouriteColors[i]; count++;
					var rect  = P3dHelper.Reserve();

					if (GUI.Button(rect, GUIContent.none) == true)
					{
						P3dWindowData.Instance.CurrentColor = color;
					}

					rect.xMin += 3;
					rect.xMax -= 3;
					rect.yMin += 3;
					rect.yMax -= 3;
					
					P3dHelper.BeginColor(color);
						GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
					P3dHelper.EndColor();
				}

				if (count == 0)
				{
					EditorGUILayout.HelpBox("You have no favourite colors!", MessageType.Info);
				}
			}
		}

		private void DrawFavouriteBrushes()
		{
			if (GUILayout.Button("Favourite Brushes", expandFavouriteBrushes == true ? EditorStyles.toolbarDropDown : EditorStyles.toolbarButton) == true)
			{
				expandFavouriteBrushes = !expandFavouriteBrushes;
			}

			if (expandFavouriteBrushes == true)
			{
				var count = 0;

				tempFavourites.Clear();
				tempFavourites.AddRange(P3dWindowData.Instance.FavouriteBrushesSafe);

				foreach (var favourite in tempFavourites)
				{
					DrawBrush(favourite); count++;
				}

				if (count == 0)
				{
					EditorGUILayout.HelpBox("You have no favourite brushes!", MessageType.Info);
				}
			}
		}

		private void DrawUncategorizedBrushes(IEnumerable<P3dBrush> brushes)
		{
			if (GUILayout.Button("Uncategorized Brushes", expandUncategorizedBrushes == true ? EditorStyles.toolbarDropDown : EditorStyles.toolbarButton) == true)
			{
				expandUncategorizedBrushes = !expandUncategorizedBrushes;
			}

			if (expandUncategorizedBrushes == true)
			{
				var count = 0;

				foreach (var brush in brushes)
				{
					if (string.IsNullOrEmpty(brush.Category) == true)
					{
						DrawBrush(brush); count++;
					}
				}

				if (count == 0)
				{
					EditorGUILayout.HelpBox("You have no uncategorized brushes!", MessageType.Info);
				}
			}
		}

		private void DrawBrushes(IEnumerable<P3dBrush> brushes, string category)
		{
			if (GUILayout.Button(category + " Brushes", Expanded.Contains(category) == true ? EditorStyles.toolbarDropDown : EditorStyles.toolbarButton) == true)
			{
				if (Expanded.Contains(category) == true) Expanded.Remove(category); else Expanded.Add(category);
			}

			if (Expanded.Contains(category) == true)
			{
				foreach (var brush in brushes)
				{
					if (brush.Category == category)
					{
						DrawBrush(brush);
					}
				}
			}
		}

		private static GUIStyle buttonL;
		private static GUIStyle buttonS;

		private Rect GetRect(Rect rect, float min, float max)
		{
			rect.xMin = min;
			rect.xMax = max;

			return rect;
		}

		private void DrawBrush(P3dBrush brush)
		{
			var rect   = P3dHelper.Reserve();
			var rectS  = GetRect(rect, rect.xMin, rect.xMin + 18);
			var rectA  = GetRect(rect, rect.xMin + 20, rect.xMax - 102);
			var rectB  = GetRect(rect, rect.xMax - 100, rect.xMax - 40);
			var rectC  = GetRect(rect, rect.xMax - 40, rect.xMax);
			var active = P3dWindowData.Instance.CurrentBrushesSafe.Contains(brush);
			var locked = Locked.Contains(brush);

			if (buttonL == null)
			{
				buttonL = new GUIStyle(EditorStyles.helpBox);
				//buttonL.fontSize = 15;
				buttonL.clipping = TextClipping.Overflow;
			}

			EditorGUI.BeginDisabledGroup(locked == true);
				P3dHelper.BeginColor(active == true ? Color.green : Color.white);
					if (GUI.Button(rectA, brush.name, buttonL) == true)
					{
						if (Event.current.control == true || Event.current.shift == true)
						{
							if (active == true)
							{
								P3dWindowData.Instance.CurrentBrushesSafe.Remove(brush);
							}
							else
							{
								P3dWindowData.Instance.CurrentBrushesSafe.Add(brush);
							}
						}
						else
						{
							var count = P3dWindowData.Instance.CurrentBrushesSafe.RemoveAll(b => Locked.Contains(b) == false);

							if (active == false || count > 1)
							{
								P3dWindowData.Instance.CurrentBrushesSafe.Add(brush);
							}
						}
					}
				P3dHelper.EndColor();
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(active == false);
				P3dHelper.BeginColor(locked == true ? Color.green : Color.white);
					if (GUI.Button(rectB, locked == true ? "Unlock" : "Lock", EditorStyles.miniButtonLeft) == true)
					{
						if (locked == true)
						{
							Locked.Remove(brush);
						}
						else
						{
							Locked.Add(brush);
						}
					}
				P3dHelper.EndColor();
			EditorGUI.EndDisabledGroup();

			if (GUI.Button(rectC, new GUIContent("Find", "Select the brush in the project?"), EditorStyles.miniButtonRight) == true)
			{
				Selection.activeObject = brush;

				EditorGUIUtility.PingObject(brush);
			}

			var starred = P3dWindowData.Instance.FavouriteBrushesSafe.Contains(brush) == true;

			if (DrawStarButton(rectS, starred) == true)
			{
				if (starred == true)
				{
					P3dWindowData.Instance.FavouriteBrushesSafe.Remove(brush);
				}
				else
				{
					P3dWindowData.Instance.FavouriteBrushesSafe.Add(brush);
				}
			}
		}

		private bool DrawStarButton(Rect rect, bool starred)
		{
			var favouriteContent = new GUIContent(starred == true ? "★" : "✰", "Add/Remove this from the favourites list?");

			if (buttonS == null)
			{
				buttonS = new GUIStyle(EditorStyles.miniButton);
				buttonS.fontSize = 15;
				buttonS.clipping = TextClipping.Overflow;
			}

			P3dHelper.BeginColor(starred == true ? Color.yellow : Color.white);
				var clicked = GUI.Button(rect, favouriteContent, buttonS);
			P3dHelper.EndColor();

			return clicked;
		}

		private bool CanPaint()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanPaint() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanUndo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanUndo() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanWrite()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanWrite() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool NeedsWrite()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].NeedsWrite() == true)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanRedo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				if (paintables[i].CanRedo() == true)
				{
					return true;
				}
			}

			return false;
		}

		private void Undo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				paintables[i].Undo();
			}
		}

		private void Write()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				paintables[i].Write();
			}
		}

		private void Redo()
		{
			for (var i = paintables.Count - 1; i >= 0; i--)
			{
				paintables[i].Redo();
			}
		}
	}
}
#endif