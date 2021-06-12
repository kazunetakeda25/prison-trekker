namespace PaintIn3D
{
	/// <summary>This interface allows you to create components that modify the paint radius.</summary>
	public interface IModifyRadius
	{
		void ModifyRadius(float pressure, ref float radius);
	}
}