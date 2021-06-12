#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This class handles saving and loading of data for the main Paint in 3D window.</summary>
	public class P3dWindowData
	{
		public enum ColorBlendType
		{
			None,
			Replace,
			Multiply
		}

		public static P3dWindowData Instance = new P3dWindowData();

		public static bool Loaded;

		public int MaxUndoSteps = 10;

		public Color CurrentColor = Color.white;

		public ColorBlendType CurrentBlend = ColorBlendType.Multiply;

		public List<Color> FavouriteColors = new List<Color>();

		public List<P3dBrush> FavouriteBrushes = new List<P3dBrush>();

		public List<P3dBrush> FavouriteBrushesSafe
		{
			get
			{
				if (FavouriteBrushes == null) FavouriteBrushes = new List<P3dBrush>();

				for (var i = FavouriteBrushes.Count - 1; i >= 0; i--)
				{
					if (FavouriteBrushes[i] == null) FavouriteBrushes.RemoveAt(i);
				}

				return FavouriteBrushes;
			}
		}

		public List<P3dBrush> CurrentBrushes = new List<P3dBrush>();

		public List<P3dBrush> CurrentBrushesSafe
		{
			get
			{
				if (CurrentBrushes == null) CurrentBrushes = new List<P3dBrush>();

				for (var i = CurrentBrushes.Count - 1; i >= 0; i--)
				{
					if (CurrentBrushes[i] == null) CurrentBrushes.RemoveAt(i);
				}

				return CurrentBrushes;
			}
		}

		public static void Load()
		{
			if (Loaded == false)
			{
				Loaded = true;

				if (EditorPrefs.HasKey("P3dWindow") == true)
				{
					EditorJsonUtility.FromJsonOverwrite(EditorPrefs.GetString("P3dWindow"), Instance);
				}
			}
		}

		public static void Save()
		{
			EditorPrefs.SetString("P3dWindow", EditorJsonUtility.ToJson(Instance));
		}
	}
}
#endif