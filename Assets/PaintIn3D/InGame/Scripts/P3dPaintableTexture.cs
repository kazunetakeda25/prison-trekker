using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to make one texture on the attached Renderer paintable.
	/// NOTE: If the texture or texture slot you want to paint is part of a shared material (e.g. prefab material), then I recommend you add the P3dMaterialCloner component to make it unique.</summary>
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(P3dPaintable))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintableTexture")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable Texture")]
	public class P3dPaintableTexture : P3dLinkedBehaviour<P3dPaintableTexture>
	{
		public enum StateType
		{
			None,
			FullTextureCopy,
			LocalCommandCopy
		}

		public enum MipType
		{
			Auto,
			ForceOn,
			ForceOff
		}

		[System.Serializable] public class PaintableTextureEvent : UnityEvent<P3dPaintableTexture> {}

		/// <summary>The material index and shader texture slot name that this component will paint.</summary>
		public P3dSlot Slot { set { slot = value; } get { return slot; } } [SerializeField] private P3dSlot slot = new P3dSlot(0, "_MainTex");

		/// <summary>The UV channel this texture is mapped to.</summary>
		public P3dChannel Channel { set { channel = value; } get { return channel; } } [SerializeField] private P3dChannel channel;

		/// <summary>The group you want to associate this texture with. You only need to set this if you are painting multiple types of textures at the same time (e.g. 0 = albedo, 1 = illumination).</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		/// <summary>This allows you to set how this texture's state is stored. This allows you to perform undo and redo operations.
		/// FullTextureCopy = A full copy of your texture will be copied for each state. This allows you to quickly undo and redo, and works with animated skinned meshes, but it uses up a lot of texture memory.
		/// LocalCommandCopy = Each paint command will be stored in local space for each state. This allows you to perform unlimited undo and redo states with minimal memory usage, because the object will be repainted from scratch. However, performance will depend on how many states must be redrawn.</summary>
		public StateType State { set { state = value; } get { return state; } } [SerializeField] private StateType state;

		/// <summary>The amount of times this texture can have its paint operations undone.</summary>
		public int StateLimit { set { stateLimit = value; } get { return stateLimit; } } [SerializeField] private int stateLimit;

		/// <summary>If you want this texture to automatically save/load, then you can set the unique save name for it here. Keep in mind this setting won't work properly with prefab spawning since all clones will share the same SaveName.</summary>
		public string SaveName { set { saveName = value; } get { return saveName; } } [SerializeField] private string saveName;

		/// <summary>Some shaders require specific shader keywords to be enabled when adding new textures. If there is no texture in your selected slot then you may need to set this keyword.</summary>
		public string ShaderKeyword { set { shaderKeyword = value; } get { return shaderKeyword; } } [SerializeField] private string shaderKeyword;

		/// <summary>The format of the created texture.</summary>
		public RenderTextureFormat Format { set { format = value; } get { return format; } } [SerializeField] private RenderTextureFormat format;

		/// <summary>The mip maps of the created texture.
		/// Auto = On or Off based on the <b>Texture</b> mip map count.
		/// ForceOn = Always enabled.
		/// ForceOff = Always disabled.</summary>
		public MipType MipMaps { set { mipMaps = value; } get { return mipMaps; } } [SerializeField] private MipType mipMaps;

		/// <summary>The base width of the created texture.</summary>
		public int Width { set { width = value; } get { return width; } } [SerializeField] private int width = 512;

		/// <summary>The base height of the created texture.</summary>
		public int Height { set { height = value; } get { return height; } } [SerializeField] private int height = 512;

		/// <summary>The base color of the created texture.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The base texture of the created texture.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>This event is called after a paint command has been added to this texture. These commands will be executed at the end of the frame.</summary>
		public event System.Action<P3dCommand> OnAddCommand;

		/// <summary>This event is called after this texture has been painted, allowing you to perform actions like counting the pixels after modification.
		/// Bool = Preview painting.</summary>
		public event System.Action<bool> OnModified;

		[System.NonSerialized]
		private P3dPaintable cachedPaintable;

		[System.NonSerialized]
		private bool cachedPaintableSet;

		[SerializeField]
		private bool activated;

		[SerializeField]
		private RenderTexture current;

		[SerializeField]
		private RenderTexture preview;

		[SerializeField]
		private bool previewSet;

		[System.NonSerialized]
		private List<P3dPaintableState> paintableStates = new List<P3dPaintableState>();

		[System.NonSerialized]
		private int stateIndex;

		[System.NonSerialized]
		private P3dPaintable paintable;

		[System.NonSerialized]
		private bool paintableSet;

		[System.NonSerialized]
		private Material material;

		[System.NonSerialized]
		private bool materialSet;

		[System.NonSerialized]
		private List<P3dCommand> paintCommands = new List<P3dCommand>();

		[System.NonSerialized]
		private List<P3dCommand> previewCommands = new List<P3dCommand>();

		[System.NonSerialized]
		private List<P3dCommand> localCommands = new List<P3dCommand>();

		[System.NonSerialized]
		private static List<P3dPaintableTexture> tempPaintableTextures = new List<P3dPaintableTexture>();

		/// <summary>This lets you know if this texture is activated and ready for painting. Activation is controlled by the associated P3dPaintable component.</summary>
		public bool Activated
		{
			get
			{
				return activated;
			}
		}

		/// <summary>This lets you know if there is at least one undo state this texture can be undone into.</summary>
		public bool CanUndo
		{
			get
			{
				return state != StateType.None && stateIndex > 0;
			}
		}

		/// <summary>This lets you know if there is at least one redo state this texture can be redone into.</summary>
		public bool CanRedo
		{
			get
			{
				return state != StateType.None && stateIndex < paintableStates.Count - 1;
			}
		}

		/// <summary>This quickly gives you the P3dPaintable component associated with this paintable texture.</summary>
		public P3dPaintable CachedPaintable
		{
			get
			{
				if (cachedPaintableSet == false)
				{
					cachedPaintable    = GetComponent<P3dPaintable>();
					cachedPaintableSet = true;
				}

				return cachedPaintable;
			}
		}

		/// <summary>This gives you the current state of this paintabe texture.
		/// NOTE: This will only exist if your texture is activated.
		/// NOTE: This is a RenderTexture, so you can't directly read it. Use the GetReadableTexture() method if you need to.
		/// NOTE: This doesn't include any preview painting information, access the Preview property if you need to.</summary>
		public RenderTexture Current
		{
			set
			{
				if (materialSet == true)
				{
					current = value;

					material.SetTexture(slot.Name, current);
				}
			}

			get
			{
				return current;
			}
		}

		/// <summary>This gives you the current state of this paintabe texture including any preview painting information.</summary>
		public RenderTexture Preview
		{
			get
			{
				return preview;
			}
		}

		/// <summary>This allows you to get a list of all paintable textures on a P3dModel/P3dPaintable within the specified group mask.</summary>
		public static List<P3dPaintableTexture> Filter(P3dModel model, int groupMask)
		{
			tempPaintableTextures.Clear();

			if (model.Paintable != null)
			{
				var paintableTextures = model.Paintable.PaintableTextures;

				for (var i = paintableTextures.Count - 1; i >= 0; i--)
				{
					var paintableTexture = paintableTextures[i];

					if (P3dHelper.IndexInMask(paintableTexture.group, groupMask) == true)
					{
						tempPaintableTextures.Add(paintableTexture);
					}
				}
			}

			return tempPaintableTextures;
		}

		/// <summary>This will clear all undo/redo texture states.</summary>
		[ContextMenu("Clear States")]
		public void ClearStates()
		{
			if (paintableStates != null)
			{
				for (var i = paintableStates.Count - 1; i >= 0; i--)
				{
					paintableStates[i].Pool();
				}

				paintableStates.Clear();

				stateIndex = 0;
			}
		}

		/// <summary>This will store a texture state so that it can later be undone. This should be called before you perform texture modifications.</summary>
		[ContextMenu("Store State")]
		public void StoreState()
		{
			if (activated == true)
			{
				// If this is the latest state, then don't store or trim future
				if (stateIndex != paintableStates.Count - 1)
				{
					TrimFuture();

					AddState();
				}

				if (state == StateType.FullTextureCopy)
				{
					TrimPast();
				}

				stateIndex = paintableStates.Count;
			}
		}

		/// <summary>This will revert the texture to a previous state, if you have an undo state stored.</summary>
		[ContextMenu("Undo")]
		public void Undo()
		{
			if (CanUndo == true)
			{
				// If we're undoing for the first time, store the current state so we can redo back to it
				if (stateIndex == paintableStates.Count)
				{
					AddState();
				}

				ClearCommands();

				stateIndex -= 1;

				switch (state)
				{
					case StateType.FullTextureCopy:
					{
						var paintableState = paintableStates[stateIndex];

						P3dHelper.Blit(current, paintableState.Texture);
					}
					break;

					case StateType.LocalCommandCopy:
					{
						RebuildFromCommands();
					}
					break;
				}

				NotifyOnModified(false);
			}
		}

		/// <summary>This will restore a previously undone texture state, if you've performed an undo.</summary>
		[ContextMenu("Redo")]
		public void Redo()
		{
			if (CanRedo == true)
			{
				ClearCommands();

				stateIndex += 1;

				switch (state)
				{
					case StateType.FullTextureCopy:
					{
						var paintableState = paintableStates[stateIndex];

						P3dHelper.Blit(current, paintableState.Texture);

						NotifyOnModified(false);
					}
					break;

					case StateType.LocalCommandCopy:
					{
						RebuildFromCommands();
					}
					break;
				}
			}
		}

		private void RebuildFromCommands()
		{
			P3dPaintReplace.Blit(current, texture, color);
			
			var localToWorldMatrix = transform.localToWorldMatrix;

			for (var i = 0; i <= stateIndex; i++)
			{
				var paintableState = paintableStates[i];
				var commandCount   = paintableState.Commands.Count;

				for (var j = 0; j < commandCount; j++)
				{
					var worldCommand = paintableState.Commands[j].SpawnCopy();

					worldCommand.SetLocation(localToWorldMatrix * worldCommand.Matrix);

					AddCommand(worldCommand);
				}
			}

			ExecuteCommands(false);

			NotifyOnModified(false);
		}

		private void AddState()
		{
			var paintableState = P3dPaintableState.Pop();

			switch (state)
			{
				case StateType.FullTextureCopy:
				{
					paintableState.Write(current);
				}
				break;

				case StateType.LocalCommandCopy:
				{
					paintableState.Write(localCommands);

					localCommands.Clear();
				}
				break;
			}

			paintableStates.Add(paintableState);
		}

		private void TrimFuture()
		{
			for (var i = paintableStates.Count - 1; i >= stateIndex; i--)
			{
				paintableStates[i].Pool();

				paintableStates.RemoveAt(i);
			}
		}

		private void TrimPast()
		{
			for (var i = paintableStates.Count - stateLimit - 1; i >= 0; i--)
			{
				paintableStates[i].Pool();

				paintableStates.RemoveAt(i);
			}
		}

		/// <summary>You should call this after painting this paintable texture.</summary>
		public void NotifyOnModified(bool preview)
		{
			if (OnModified != null)
			{
				OnModified.Invoke(preview);
			}
		}
		
		/// <summary>If you need to read pixels from this paintable texture then this will return a Texture2D you can read.
		/// NOTE: A new texture is allocated each time you call this, so you must manually delete it when finished.</summary>
		public Texture2D GetReadableCopy()
		{
			return P3dHelper.GetReadableCopy(current);
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			Clear(texture, color);
		}

		/// <summary>This allows you to clear all pixels in this paintable texture with the specified color.</summary>
		public void Clear(Color color)
		{
			Clear(texture, color);
		}

		/// <summary>This allows you to clear all pixels in this paintable texture with the specified color and texture.</summary>
		public void Clear(Texture texture, Color color)
		{
			if (activated == true)
			{
				P3dPaintReplace.Blit(current, texture, color);
			}
		}

		/// <summary>This will automatically save the current texture state with the current SaveName.</summary>
		[ContextMenu("Save")]
		public void Save()
		{
			Save(saveName);
		}

		/// <summary>This will save the current texture state with the specified save name.</summary>
		public void Save(string saveName)
		{
			if (activated == true)
			{
				var readableTexture = P3dHelper.GetReadableCopy(current);

				P3dHelper.SavePngTextureData(readableTexture.EncodeToPNG(), saveName);

				P3dHelper.Destroy(readableTexture);
			}
		}

		/// <summary>This will automatically load the current texture state with the current SaveName.</summary>
		[ContextMenu("Load")]
		public void Load()
		{
			Load(saveName);
		}

		/// <summary>This will load the current texture state with the specified save name.</summary>
		public void Load(string saveName)
		{
			if (activated == true && current != null)
			{
				var tempTexture = default(Texture2D);

				if (P3dHelper.TryLoadTexture(saveName, ref tempTexture) == true)
				{
					P3dHelper.Blit(current, tempTexture);

					P3dHelper.Destroy(tempTexture);
				}
			}
		}

		/// <summary>If you last painted using preview painting and you want to hide the preview painting, you can call this method to force the texture to go back to its current state.</summary>
		public void HidePreview()
		{
			if (activated == true && current != null && material != null)
			{
				material.SetTexture(slot.Name, current);
			}
		}

		/// <summary>This automatically calls HidePreview on all active and enabled paintable textures.</summary>
		public static void HideAllPreviews()
		{
			var instance = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				instance.HidePreview();

				instance = instance.NextInstance;
			}
		}

		/// <summary>This will automatically clear save data with the current SaveName.</summary>
		[ContextMenu("Clear Save")]
		public void ClearSave()
		{
			P3dHelper.ClearTexture(saveName);
		}

		/// <summary>This will clear save data with the specified save name.</summary>
		public void ClearSave(string saveName)
		{
			P3dHelper.ClearTexture(saveName);
		}

		/// <summary>If you modified the slot material index, then call this to update the cached material.</summary>
		[ContextMenu("Update Material")]
		public void UpdateMaterial()
		{
			material    = P3dHelper.GetMaterial(gameObject, slot.Index);
			materialSet = true;
		}

		/// <summary>If the current slot has a texture, this allows you to copy the width and height from it.</summary>
		[ContextMenu("Copy Size")]
		public void CopySize()
		{
			var texture = Slot.FindTexture(gameObject);

			if (texture != null)
			{
				width  = texture.width;
				height = texture.height;
			}
		}

		/// <summary>This allows you to manually activate this paintable texture.
		/// NOTE: This will automatically be called by the associated P3dPaintable component when it activates.</summary>
		[ContextMenu("Activate")]
		public void Activate()
		{
			if (activated == false)
			{
				UpdateMaterial();

				if (material != null)
				{
					var finalWidth   = width;
					var finalHeight  = height;
					var finalTexture = material.GetTexture(slot.Name);

					CachedPaintable.ScaleSize(ref finalWidth, ref finalHeight);

					if (texture != null)
					{
						finalTexture = texture;
					}

					if (string.IsNullOrEmpty(shaderKeyword) == false)
					{
						material.EnableKeyword(shaderKeyword);
					}

					var desc = new RenderTextureDescriptor(width, height, format, 0);

					desc.autoGenerateMips = false;

					if (mipMaps == MipType.ForceOn)
					{
						desc.useMipMap = true;
					}
					else if (mipMaps == MipType.Auto && P3dHelper.HasMipMaps(finalTexture) == true)
					{
						desc.useMipMap = true;
					}

					current = P3dHelper.GetRenderTexture(desc);

					P3dPaintReplace.Blit(current, finalTexture, color);

					if (current.useMipMap == true) current.GenerateMips();

					material.SetTexture(slot.Name, current);

					activated = true;

					if (string.IsNullOrEmpty(saveName) == false)
					{
						Load();
					}

					NotifyOnModified(false);
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (paintableSet == false)
			{
				paintable    = GetComponent<P3dPaintable>();
				paintableSet = true;
			}

			paintable.Register(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			
			paintable.Unregister(this);
		}

		protected virtual void OnDestroy()
		{
			if (activated == true)
			{
				if (string.IsNullOrEmpty(saveName) == false)
				{
					Save();
				}

				P3dHelper.ReleaseRenderTexture(current);
				P3dHelper.ReleaseRenderTexture(preview);

				ClearStates();
			}
		}

		/// <summary>This will add a paint command to this texture's paint stack. The paint stack will be executed at the end of the current frame.</summary>
		public void AddCommand(P3dCommand command)
		{
			if (command.Preview == true)
			{
				previewCommands.Add(command);
			}
			else
			{
				paintCommands.Add(command);

				if (state == StateType.LocalCommandCopy && command.Preview == false)
				{
					var localCommand = command.SpawnCopyLocal(transform);

					localCommands.Add(localCommand);
				}
			}

			if (OnAddCommand != null)
			{
				OnAddCommand(command);
			}
		}

		/// <summary>This lets you know if there are paint commands in this paintable texture's paint stack.</summary>
		public bool CommandsPending
		{
			get
			{
				return paintCommands.Count + previewCommands.Count > 0;
			}
		}

		public void ClearCommands()
		{
			for (var i = previewCommands.Count - 1; i >= 0; i--)
			{
				previewCommands[i].Pool();
			}

			previewCommands.Clear();

			for (var i = paintCommands.Count - 1; i >= 0; i--)
			{
				paintCommands[i].Pool();
			}

			paintCommands.Clear();

			for (var i = localCommands.Count - 1; i >= 0; i--)
			{
				localCommands[i].Pool();
			}

			localCommands.Clear();
		}

		/// <summary>This allows you to manually execute all commands in the paint stack.
		/// This is useful if you need to modify the state of your object before the end of the frame.</summary>
		public void ExecuteCommands(bool sendNotifications)
		{
			if (activated == true)
			{
				var hidePreview = true;

				if (CommandsPending == true)
				{
					var oldActive      = RenderTexture.active;
					var swap           = P3dHelper.GetRenderTexture(current.descriptor);
					var preparedMesh   = default(Mesh);
					var preparedMatrix = default(Matrix4x4);

					swap.DiscardContents();

					Graphics.Blit(current, swap);

					// Paint
					if (paintCommands.Count > 0)
					{
						ExecuteCommands(paintCommands, sendNotifications, ref current, ref swap, ref preparedMesh, ref preparedMatrix);

						if (current.useMipMap == true) current.GenerateMips();
					}

					// Preview
					if (previewCommands.Count > 0)
					{
						if (previewSet == false)
						{
							preview    = P3dHelper.GetRenderTexture(current.descriptor);
							previewSet = true;
						}

						hidePreview = false;

						preview.DiscardContents();

						Graphics.Blit(current, preview);

						ExecuteCommands(previewCommands, sendNotifications, ref preview, ref swap, ref preparedMesh, ref preparedMatrix);

						if (preview.useMipMap == true) preview.GenerateMips();
					}

					P3dHelper.ReleaseRenderTexture(swap);

					RenderTexture.active = oldActive;
				}

				if (hidePreview == true && previewSet == true)
				{
					P3dHelper.ReleaseRenderTexture(preview);

					preview    = null;
					previewSet = false;
				}

				if (materialSet == false)
				{
					UpdateMaterial();
				}

				material.SetTexture(slot.Name, previewSet == true ? preview : current);
			}
		}

		private void ExecuteCommands(List<P3dCommand> commands, bool sendNotifications, ref RenderTexture main, ref RenderTexture swap, ref Mesh preparedMesh, ref Matrix4x4 preparedMatrix)
		{
			RenderTexture.active = main;

			for (var i = 0; i < commands.Count; i++)
			{
				var command         = commands[i];
				var commandMaterial = command.Material;

				RenderTexture.active = swap;

				// Switch main and swap
				var temp = swap; swap = main; main = temp;

				commandMaterial.SetTexture(P3dShader._Buffer, swap);

				command.Apply();

				if (command.RequireMesh == true)
				{
					command.Model.GetPrepared(ref preparedMesh, ref preparedMatrix);

					switch (channel)
					{
						case P3dChannel.UV : commandMaterial.SetVector(P3dShader._Channel, new Vector4(1.0f, 0.0f, 0.0f, 0.0f)); break;
						case P3dChannel.UV2: commandMaterial.SetVector(P3dShader._Channel, new Vector4(0.0f, 1.0f, 0.0f, 0.0f)); break;
						case P3dChannel.UV3: commandMaterial.SetVector(P3dShader._Channel, new Vector4(0.0f, 0.0f, 1.0f, 0.0f)); break;
						case P3dChannel.UV4: commandMaterial.SetVector(P3dShader._Channel, new Vector4(0.0f, 0.0f, 0.0f, 1.0f)); break;
					}

					commandMaterial.SetPass(0);

					Graphics.DrawMeshNow(preparedMesh, preparedMatrix, slot.Index);
				}
				else
				{

					Graphics.Blit(default(Texture), main, commandMaterial);
				}

				command.Pool();
			}

			commands.Clear();

			if (sendNotifications == true)
			{
				NotifyOnModified(commands == previewCommands);
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintableTexture))]
	public class P3dPaintableTexture_Editor : P3dEditor<P3dPaintableTexture>
	{
		private bool expandSlot;

		protected override void OnInspector()
		{
			if (Any(t => t.Activated == true))
			{
				EditorGUILayout.HelpBox("This component has been activated.", MessageType.Info);
			}

			DrawExpand(ref expandSlot, "slot", "The material index and shader texture slot name that this component will paint.");
			if (expandSlot == true)
			{
				BeginIndent();
					BeginDisabled();
						EditorGUI.ObjectField(P3dHelper.Reserve(), new GUIContent("Texture", "This is the current texture in the specified texture slot."), Target.Slot.FindTexture(Target.gameObject), typeof(Texture), false);
					EndDisabled();
				EndIndent();
			}
			Draw("channel", "The UV channel this texture is mapped to.");
			Draw("shaderKeyword", "Some shaders require specific shader keywords to be enabled when adding new textures. If there is no texture in your selected slot then you may need to set this keyword.");

			Separator();

			Draw("group", "The group you want to associate this texture with. You only need to set this if you are painting multiple types of textures at the same time (e.g. 0 = albedo, 1 = illumination).");
			Draw("state", "This allows you to set how this texture's state is stored. This allows you to perform undo and redo operations.\nFullTextureCopy = A full copy of your texture will be copied for each state. This allows you to quickly undo and redo, and works with animated skinned meshes, but it uses up a lot of texture memory.\nLocalCommandCopy = Each paint command will be stored in local space for each state. This allows you to perform unlimited undo and redo states with minimal memory usage, because the object will be repainted from scratch. However, performance will depend on how many states must be redrawn.");
			if (Any(t => t.State == P3dPaintableTexture.StateType.FullTextureCopy))
			{
				BeginIndent();
					Draw("stateLimit", "The amount of times this texture can have its paint operations undone.", "Limit");
				EndIndent();
			}
			Draw("saveName", "If you want this texture to automatically save/load, then you can set the unique save name for it here. Keep in mind this setting won't work properly with prefab spawning since all clones will share the same SaveName.");

			Separator();

			Draw("format", "The format of the created texture.");
			Draw("mipMaps", "The mip maps of the created texture.\n\nAuto = On or Off based on the <b>Texture</b> mip map count.\n\nForceOn = Always enabled.\n\nForceOff = Always disabled.");
			DrawSize();
			Draw("color", "The base color of the created texture.");
			Draw("texture", "The format of the created texture.");
		}

		private void DrawSize()
		{
			var rect  = P3dHelper.Reserve();
			var rectL = rect; rectL.width = EditorGUIUtility.labelWidth;

			EditorGUI.LabelField(rectL, new GUIContent("Size", "This allows you to control the width and height of the texture when it gets activated."));

			rect.xMin += EditorGUIUtility.labelWidth;

			var rectR = rect; rectR.xMin = rectR.xMax - 48;
			var rectW = rect; rectW.xMax -= 50; rectW.xMax -= rectW.width / 2 + 1;
			var rectH = rect; rectH.xMax -= 50; rectH.xMin += rectH.width / 2 + 1;

			EditorGUI.PropertyField(rectW, serializedObject.FindProperty("width"), GUIContent.none);
			EditorGUI.PropertyField(rectH, serializedObject.FindProperty("height"), GUIContent.none);

			BeginDisabled(All(CannotScale));
				if (GUI.Button(rectR, new GUIContent("Copy", "Copy the width and height from the current slot?"), EditorStyles.miniButton) == true)
				{
					Undo.RecordObjects(targets, "Copy Sizes");

					Each(t => t.CopySize(), true);
				}
			EndDisabled();
		}

		private bool CannotScale(P3dPaintableTexture paintableTexture)
		{
			var texture = paintableTexture.Slot.FindTexture(paintableTexture.gameObject);

			if (texture != null)
			{
				if (texture.width != paintableTexture.Width || texture.height != paintableTexture.Height)
				{
					return false;
				}
			}

			return true;
		}
	}
}
#endif