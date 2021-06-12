using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This interface allows you to make components that can paint 3D points with a specified orientation.</summary>
	public interface IHitPoint
	{
		void HandleHitPoint(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Collider collider, Vector3 worldPosition, Quaternion worldRotation, float pressure);
	}
}