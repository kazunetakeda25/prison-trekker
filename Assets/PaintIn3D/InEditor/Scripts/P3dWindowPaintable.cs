#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This class handles a locked object for the main Paint in 3D window.</summary>
	public class P3dWindowPaintable
	{
		public GameObject Root;

		private Mesh bakedMesh;

		private Mesh lastMesh;

		private Matrix4x4 lastMatrix;

		//private Bounds lastBounds;

		public List<P3dWindowPaintableTexture> PaintableTextures = new List<P3dWindowPaintableTexture>();

		public P3dWindowPaintable(GameObject newRoot)
		{
			Root = newRoot;
		}

		public bool CanPaint()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				if (PaintableTextures[i].Locked == true)
				{
					return true;
				}
			}

			return false;
		}

		public bool CanUndo()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				if (PaintableTextures[i].CanUndo() == true)
				{
					return true;
				}
			}

			return false;
		}

		public bool CanWrite()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				if (PaintableTextures[i].CanWrite() == true)
				{
					return true;
				}
			}

			return false;
		}

		public bool NeedsWrite()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				if (PaintableTextures[i].NeedsWrite() == true)
				{
					return true;
				}
			}

			return false;
		}

		public bool CanRedo()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				if (PaintableTextures[i].CanRedo() == true)
				{
					return true;
				}
			}

			return false;
		}

		public void Undo()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				PaintableTextures[i].Undo();
			}
		}

		public void Write()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				PaintableTextures[i].Write();
			}
		}

		public void Redo()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				PaintableTextures[i].Redo();
			}
		}

		public bool Raycast(Ray ray, ref RaycastHit hit)
		{
			var skinnedMeshRenderer = Root.GetComponent<SkinnedMeshRenderer>();

			if (skinnedMeshRenderer != null)
			{
				if (skinnedMeshRenderer.sharedMesh != null)
				{
					if (bakedMesh == null)
					{
						bakedMesh = new Mesh();
					}

					var scaling    = P3dHelper.Reciprocal3(Root.transform.lossyScale);
					var localScale = Root.transform.localScale;

					Root.transform.localScale = Vector3.one;

					skinnedMeshRenderer.BakeMesh(bakedMesh);

					Root.transform.localScale = localScale;

					lastMesh   = bakedMesh;
					lastMatrix = Root.transform.localToWorldMatrix;
					//lastBounds = skinnedMeshRenderer.bounds;

					var matrix = lastMatrix * Matrix4x4.Scale(scaling);

					if (P3dWindowIntersect.IntersectRayMesh(ray, bakedMesh, matrix, out hit) == true)
					{
						return true;
					}
				}
			}
			else
			{
				var meshRenderer = Root.GetComponent<MeshRenderer>();

				if (meshRenderer != null)
				{
					var meshFilter = Root.GetComponent<MeshFilter>();

					if (meshFilter != null)
					{
						var mesh = meshFilter.sharedMesh;

						if (mesh != null)
						{
							lastMesh   = mesh;
							lastMatrix = Root.transform.localToWorldMatrix;
							//lastBounds = meshRenderer.bounds;

							if (P3dWindowIntersect.IntersectRayMesh(ray, mesh, lastMatrix, out hit) == true)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public void Paint(P3dCommand command, List<P3dWindowPaintable> paintables)
		{
			var commandMaterial = command.Material;

			//if (bounds.Intersects(lastBounds) == true)
			{
				for (var i = PaintableTextures.Count - 1; i >= 0; i--)
				{
					var paintableTexture = PaintableTextures[i];
					var renderTexture    = paintableTexture.PreparePaint(); // Prepare the paint regardless, to sync undo states

					if (P3dHelper.IndexInMask(paintableTexture.Group, command.Groups) == true)
					{
						var oldActive = RenderTexture.active;
						var swap      = default(RenderTexture);

						RenderTexture.active = renderTexture;

						if (true) // TODO
						{
							swap = P3dHelper.GetRenderTexture(renderTexture.descriptor);

							P3dHelper.Blit(swap, renderTexture);

							commandMaterial.SetTexture(P3dShader._Buffer, swap);
						}

						command.Apply();

						if (command.RequireMesh == true)
						{
							for (var j = paintables.Count - 1; j >= 0; j--)
							{
								var otherP = paintables[j];

								for (var k = otherP.PaintableTextures.Count - 1; k >= 0; k--)
								{
									var otherT = otherP.PaintableTextures[k];

									if (otherT.OldTexture == paintableTexture.OldTexture)
									{
										switch (otherT.Channel)
										{
											case P3dChannel.UV : commandMaterial.SetVector(P3dShader._Channel, new Vector4(1.0f, 0.0f, 0.0f, 0.0f)); break;
											case P3dChannel.UV2: commandMaterial.SetVector(P3dShader._Channel, new Vector4(0.0f, 1.0f, 0.0f, 0.0f)); break;
											case P3dChannel.UV3: commandMaterial.SetVector(P3dShader._Channel, new Vector4(0.0f, 0.0f, 1.0f, 0.0f)); break;
											case P3dChannel.UV4: commandMaterial.SetVector(P3dShader._Channel, new Vector4(0.0f, 0.0f, 0.0f, 1.0f)); break;
										}

										commandMaterial.SetPass(0);

										Graphics.DrawMeshNow(otherP.lastMesh, otherP.lastMatrix, otherT.MaterialIndex);
									}
								}
							}
						}
						else
						{
							Graphics.Blit(default(Texture), renderTexture, commandMaterial);
						}

						RenderTexture.active = oldActive;

						if (swap != null)
						{
							P3dHelper.ReleaseRenderTexture(swap);
						}
					}
				}
			}
		}

		public void Apply()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				PaintableTextures[i].Apply();
			}
		}

		public void Revert()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				PaintableTextures[i].Revert();
			}
		}

		public void Unlock()
		{
			for (var i = PaintableTextures.Count - 1; i >= 0; i--)
			{
				PaintableTextures[i].Unlock();
			}
		}
	}
}
#endif