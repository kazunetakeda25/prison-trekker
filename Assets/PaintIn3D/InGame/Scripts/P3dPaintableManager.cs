using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.</summary>
	[DisallowMultipleComponent]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintableManager")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable Manager")]
	public class P3dPaintableManager : P3dLinkedBehaviour<P3dPaintableManager>
	{
		[System.NonSerialized]
		public static int MatrixCount;

		[System.NonSerialized]
		public static int RepeaterCount;

		[System.NonSerialized]
		private static List<Matrix4x4> tempMatrices = new List<Matrix4x4>();

		[System.NonSerialized]
		private static List<P3dTransform> tempRepeaters = new List<P3dTransform>();

		[System.NonSerialized]
		private static List<IModify> tempModifiers = new List<IModify>();

		public static Material[] BuildMaterialsBlendModes(string shaderName)
		{
			var shader    = P3dShader.Load(shaderName);
			var materials = new Material[P3dBlendMode.COUNT];
			
			for (var index = 0; index < P3dBlendMode.COUNT; index++)
			{
				var material = P3dShader.Build(shader);

				P3dShader.BuildBlendMode(material, index);

				materials[index] = material;
			}

			return materials;
		}

		public static Material BuildMaterial(string shaderName)
		{
			var shader = P3dShader.Load(shaderName);

			return P3dShader.Build(shader);
		}

		public static void BuildModifiers(GameObject gameObject)
		{
			gameObject.GetComponents(tempModifiers);
		}

		public static void ModifyColor(float pressure, ref Color color)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyColor;

				if (modifier != null)
				{
					modifier.ModifyColor(pressure, ref color);
				}
			}
		}

		public static void ModifyAngle(float pressure, ref float angle)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyAngle;

				if (modifier != null)
				{
					modifier.ModifyAngle(pressure, ref angle);
				}
			}
		}

		public static void ModifyOpacity(float pressure, ref float opacity)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyOpacity;

				if (modifier != null)
				{
					modifier.ModifyOpacity(pressure, ref opacity);
				}
			}
		}

		public static void ModifyHardness(float pressure, ref float hardness)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyHardness;

				if (modifier != null)
				{
					modifier.ModifyHardness(pressure, ref hardness);
				}
			}
		}

		public static void ModifyRadius(float pressure, ref float radius)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyRadius;

				if (modifier != null)
				{
					modifier.ModifyRadius(pressure, ref radius);
				}
			}
		}

		public static void ModifyTexture(float pressure, ref Texture texture)
		{
			for (var i = 0; i < tempModifiers.Count; i++)
			{
				var modifier = tempModifiers[i] as IModifyTexture;

				if (modifier != null)
				{
					modifier.ModifyTexture(pressure, ref texture);
				}
			}
		}

		public static void SubmitAll(P3dCommand command, bool preview, int layerMask, int groupMask, P3dModel model = null, P3dPaintableTexture paintableTexture = null, List<P3dTransform> repeaters = null, List<P3dCommand> commands = null)
		{
			command.Model   = null;
			command.Groups  = groupMask;
			command.Preview = preview;

			if (commands != null)
			{
				commands.Add(command.SpawnCopy());

				// Repeat paint?
				BuildRepeaters(command.Matrix, repeaters);

				for (var r = 0; r < RepeaterCount; r++)
				{
					for (var m = 0; m < MatrixCount; m++)
					{
						command.SetLocation(Repeat(r, m));

						commands.Add(command.SpawnCopy());
					}
				}
			}
			else
			{
				SubmitAll(command, preview, layerMask, groupMask, model, paintableTexture);

				// Repeat paint?
				BuildRepeaters(command.Matrix);

				for (var r = 0; r < RepeaterCount; r++)
				{
					for (var m = 0; m < MatrixCount; m++)
					{
						command.SetLocation(Repeat(r, m));

						SubmitAll(command, preview, layerMask, groupMask, model, paintableTexture);
					}
				}
			}
		}

		private static void SubmitAll(P3dCommand command, bool preview, int layerMask, int groupMask, P3dModel model, P3dPaintableTexture paintableTexture)
		{
			if (model != null)
			{
				if (paintableTexture != null)
				{
					Submit(command, model, paintableTexture, preview);
				}
				else
				{
					SubmitAll(command, preview, model, groupMask);
				}
			}
			else
			{
				if (paintableTexture != null)
				{
					Submit(command, paintableTexture.CachedPaintable, paintableTexture, preview);
				}
				else
				{
					SubmitAll(command, preview, layerMask, groupMask);
				}
			}
		}

		private static void SubmitAll(P3dCommand command, bool preview, int layerMask, int groupMask)
		{
			var models = P3dModel.FindOverlap(command.Position, command.Radius, layerMask);

			for (var i = models.Count - 1; i >= 0; i--)
			{
				SubmitAll(command, preview, models[i], groupMask);
			}
		}

		private static void SubmitAll(P3dCommand command, bool preview, P3dModel model, int groupMask)
		{
			var paintableTextures = P3dPaintableTexture.Filter(model, groupMask);

			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				Submit(command, model, paintableTextures[i], preview);
			}
		}

		public static void Submit(P3dCommand command, P3dModel model, P3dPaintableTexture paintableTexture, bool preview)
		{
			var copy = command.SpawnCopy();

			copy.Model   = model;
			copy.Groups  = -1;
			copy.Preview = preview;

			if (copy.Blend.Index == P3dBlendMode.REPLACE_ORIGINAL)
			{
				copy.Blend.Color   = paintableTexture.Color;
				copy.Blend.Texture = paintableTexture.Texture;
			}

			paintableTexture.AddCommand(copy);
		}

		public static void BuildRepeaters(Matrix4x4 matrix, List<P3dTransform> repeaters = null)
		{
			tempMatrices.Clear();
			tempRepeaters.Clear();

			tempMatrices.Add(matrix);

			if (repeaters != null)
			{
				for (var i = 0; i < repeaters.Count; i++)
				{
					var repeater = repeaters[i];

					if (repeater != null)
					{
						tempRepeaters.Add(repeater);
					}
				}
			}
			else
			{
				var repeater = P3dTransform.FirstInstance;

				for (var i = 0; i < P3dTransform.InstanceCount; i++)
				{
					tempRepeaters.Add(repeater);

					repeater = repeater.NextInstance;
				}
			}

			MatrixCount   = 1;
			RepeaterCount = tempRepeaters.Count;
		}

		public static Matrix4x4 Repeat(int repeaterIndex, int matrixIndex)
		{
			if (matrixIndex == 0)
			{
				MatrixCount = tempMatrices.Count;
			}

			var matrix = tempRepeaters[repeaterIndex].Repeat(tempMatrices[matrixIndex]);

			tempMatrices.Add(matrix);

			return matrix;
		}

		protected virtual void LateUpdate()
		{
			if (this == FirstInstance && P3dModel.InstanceCount > 0)
			{
				ClearAll();
				UpdateAll();
			}
			else
			{
				P3dHelper.Destroy(gameObject);
			}
		}

		private void ClearAll()
		{
			var model = P3dModel.FirstInstance;

			for (var i = 0; i < P3dModel.InstanceCount; i++)
			{
				model.Prepared = false;

				model = model.NextInstance;
			}
		}

		private void UpdateAll()
		{
			var paintableTexture = P3dPaintableTexture.FirstInstance;

			for (var i = 0; i < P3dPaintableTexture.InstanceCount; i++)
			{
				paintableTexture.ExecuteCommands(true);

				paintableTexture = paintableTexture.NextInstance;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintableManager))]
	public class P3dPaintableManager_Editor : P3dEditor<P3dPaintableManager>
	{
		[InitializeOnLoad]
		public class ExecutionOrder
		{
			static ExecutionOrder()
			{
				ForceExecutionOrder(100);
			}
		}

		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.", MessageType.Info);
		}
	}
}
#endif