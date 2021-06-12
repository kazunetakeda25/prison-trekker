using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This interface allows you to create components that modify the paint color.</summary>
	public interface IModifyColor
	{
		void ModifyColor(float pressure, ref Color color);
	}
}