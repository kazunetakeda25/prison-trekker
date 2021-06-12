using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This is the base class for all components that repeat paint commands (e.g. mirroring).</summary>
	public abstract class P3dTransform : P3dLinkedBehaviour<P3dTransform>
	{
		public abstract Matrix4x4 Repeat(Matrix4x4 matrix);
	}
}