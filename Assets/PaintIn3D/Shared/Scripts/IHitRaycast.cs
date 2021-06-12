using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This interface allows you to make components that rely on raycast information. This is useful because raycasts contain UV data, allowing direct texture painting.
	/// NOTE: Only non-convex MeshColliders can store UV data.</summary>
	public interface IHitRaycast
	{
		void HandleHitRaycast(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, RaycastHit hit, float pressure);
	}
}