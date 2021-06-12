using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This interface allows you to make components that can paint lines defined by two points.</summary>
	public interface IHitLine
	{
		void HandleHitLine(List<P3dCommand> commands, List<P3dTransform> repeaters, bool preview, Vector3 worldPositionA, Vector3 worldPositionB, float pressureA, float pressureB);
	}
}